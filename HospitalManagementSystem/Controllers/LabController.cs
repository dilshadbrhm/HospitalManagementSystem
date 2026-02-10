using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain;
using HospitalManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalManagementSystem.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class LabController : Controller
    {
        private readonly ILabResultRepository _labResultRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly INotificationService _notificationService;

        public LabController(
            ILabResultRepository labResultRepository,
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository,
            UserManager<AppUser> userManager,
            INotificationService notificationService)
        {
            _labResultRepository = labResultRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            AppUser user = await _userManager.GetUserAsync(User);
            Doctor doctor = await _doctorRepository.GetByUserIdAsync(user.Id);

            if (doctor == null)
                return RedirectToAction("Index", "Home");

            IEnumerable<LabResult> labResults = await _labResultRepository.GetByDoctorIdAsync(doctor.Id);
            return View(labResults);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            IEnumerable<Patient> patients = await _patientRepository.GetAllAsync();
            ViewBag.Patients = new SelectList(patients, "Id", "FirstName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LabResult labResult, IFormFile pdfFile)
        {
            try
            {
                AppUser user = await _userManager.GetUserAsync(User);
                Doctor doctor = await _doctorRepository.GetByUserIdAsync(user.Id);

                if (doctor == null)
                {
                    TempData["Error"] = "Doctor not found";
                    return RedirectToAction("Index", "Home");
                }

                if (labResult.PatientId == 0)
                {
                    TempData["Error"] = "Patient is required";
                    IEnumerable<Patient> patients = await _patientRepository.GetAllAsync();
                    ViewBag.Patients = new SelectList(patients, "Id", "FirstName");
                    return View(labResult);
                }

                if (pdfFile != null && pdfFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "lab-results");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(pdfFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        await pdfFile.CopyToAsync(stream);
                    }

                    labResult.PdfFilePath = "/uploads/lab-results/" + uniqueFileName;
                }

                labResult.DoctorId = doctor.Id;
                labResult.UploadedBy = doctor.FirstName + " " + doctor.LastName;
                labResult.UploadedAt = DateTime.Now;
                labResult.TestDate = DateTime.Now;

                await _labResultRepository.AddAsync(labResult);

                Patient patient = await _patientRepository.GetByIdAsync(labResult.PatientId);
                if (patient != null)
                {
                    await _notificationService.SendLabResultReadyAsync(
                        patient.Email,
                        patient.UserId,
                        patient.FirstName,
                        labResult.TestName
                    );
                }

                TempData["Success"] = "Lab result created and patient notified";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                IEnumerable<Patient> patients = await _patientRepository.GetAllAsync();
                ViewBag.Patients = new SelectList(patients, "Id", "FirstName");
                return View(labResult);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            LabResult labResult = await _labResultRepository.GetByIdAsync(id);

            if (labResult == null)
                return NotFound();

            return View(labResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _labResultRepository.DeleteAsync(id);
            TempData["Success"] = "Lab result deleted";
            return RedirectToAction("Index");
        }
    }
}
