using HospitalManagement.Application.Dtos.Appointment;
using HospitalManagement.Application.Dtos.Patient;
using HospitalManagement.Application.Dtos.Prescription;
using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Security.Claims;

namespace HospitalManagementSystem.Controllers
{
    [Authorize(Roles = "Patient")]
    [Authorize(Roles = "Patient")]
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IPrescriptionService _prescriptionService;
        private readonly IPatientRepository _patientRepository;

        public PatientController(
            IPatientService patientService,
            IPrescriptionService prescriptionService,
            IPatientRepository patientRepository)
        {
            _patientService = patientService;
            _prescriptionService = prescriptionService;
            _patientRepository = patientRepository;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            string userId = GetUserId();
            PatientDashboardDto dashboard = await _patientService.GetDashboardAsync(userId);

            if (dashboard == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(dashboard);
        }

        [HttpGet]
        public async Task<IActionResult> MyAppointments()
        {
            string userId = GetUserId();
            List<AppointmentItemDto> appointments = await _patientService.GetAppointmentsAsync(userId);
            return View(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            string userId = GetUserId();
            PatientDto profile = await _patientService.GetProfileAsync(userId);

            if (profile == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(PatientDto dto, IFormFile profileImage)
        {
            ModelState.Remove("ProfilePicture");

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            if (profileImage != null && profileImage.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "image");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(profileImage.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(stream);
                }

                dto.ProfilePicture = "/assets/image/" + uniqueFileName;
            }

            string userId = GetUserId();
            bool result = await _patientService.UpdateProfileAsync(userId, dto);

            if (result)
            {
                TempData["Success"] = "Profile updated successfully";
            }
            else
            {
                TempData["Error"] = "Failed to update profile";
            }

            return RedirectToAction("Profile");
        }

        [HttpGet]
        public async Task<IActionResult> MyPrescriptions()
        {
            string userId = GetUserId();
            Patient patient = await _patientRepository.GetByUserIdAsync(userId);

            if (patient == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<PrescriptionDto> prescriptions = await _prescriptionService.GetByPatientIdAsync(patient.Id);

            return View(prescriptions);
        }

        [HttpGet]
        public async Task<IActionResult> ViewPrescription(int id)
        {
            string userId = GetUserId();
            Patient patient = await _patientRepository.GetByUserIdAsync(userId);

            if (patient == null)
            {
                return RedirectToAction("Index", "Home");
            }

            PrescriptionDto prescription = await _prescriptionService.GetByIdAsync(id);

            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);
        }

        [HttpGet]
        public async Task<IActionResult> PrintPrescription(int id)
        {
            string userId = GetUserId();
            Patient patient = await _patientRepository.GetByUserIdAsync(userId);

            if (patient == null)
            {
                return RedirectToAction("Index", "Home");
            }

            string html = await _prescriptionService.GenerateHtmlAsync(id);

            if (html == null)
            {
                return NotFound();
            }

            return Content(html, "text/html");
        }
    }
}
