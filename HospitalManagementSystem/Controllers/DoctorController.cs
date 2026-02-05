using HospitalManagement.Application.Dtos.Appointment;
using HospitalManagement.Application.Dtos.Doctor;
using HospitalManagement.Application.Dtos.Prescription;
using HospitalManagement.Application.Dtos.Timeslot;
using HospitalManagement.Application.Interfaces;
using HospitalManagement.Application.Services;
using HospitalManagement.Domain;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;
using HospitalManagement.Infrastructure.Persistence;
using HospitalManagement.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HospitalManagementSystem.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IDoctorCabinetService _cabinetService;
        private readonly IDoctorService _doctorService;
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IEmailService _emailService;
        private readonly IPrescriptionService _prescriptionService;

        public DoctorController(
            IDoctorCabinetService cabinetService,
            IDoctorService doctorService,
            IAppointmentService appointmentService,
            IDoctorRepository doctorRepository,
            IEmailService emailService,
            IPrescriptionService prescriptionService)
        {
            _cabinetService = cabinetService;
            _doctorService = doctorService;
            _appointmentService = appointmentService;
            _doctorRepository = doctorRepository;
            _emailService = emailService;
            _prescriptionService = prescriptionService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<DoctorItemDto> doctors = await _doctorService.GetAllDoctorsAsync();
            return View(doctors);
        }

        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            DoctorProfileDto doctor = await _doctorService.GetDoctorProfileAsync(id);

            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> Cabinet()
        {
            DoctorCabinetDto result = await _cabinetService.GetCabinetAsync(GetUserId());

            if (result == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(result);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> TimeTable()
        {
            List<TimeSlotDto> result = await _cabinetService.GetTimeSlotsAsync(GetUserId());
            return View(result);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public IActionResult AddTimeSlot()
        {
            return View();
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> AddTimeSlot(CreateTimeSlotDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            bool result = await _cabinetService.AddTimeSlotAsync(GetUserId(), dto);

            if (!result)
            {
                ModelState.AddModelError("", "Something went wrong");
                return View(dto);
            }

            return RedirectToAction("TimeTable");
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> EditTimeSlot(int id)
        {
            TimeSlotDto result = await _cabinetService.GetTimeSlotByIdAsync(GetUserId(), id);

            if (result == null)
            {
                return RedirectToAction("TimeTable");
            }

            return View(result);
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> EditTimeSlot(TimeSlotDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            bool result = await _cabinetService.UpdateTimeSlotAsync(GetUserId(), dto);

            if (!result)
            {
                ModelState.AddModelError("", "Something went wrong");
                return View(dto);
            }

            return RedirectToAction("TimeTable");
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> DeleteTimeSlot(int id)
        {
            await _cabinetService.DeleteTimeSlotAsync(GetUserId(), id);
            return RedirectToAction("TimeTable");
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> UpdateAppointmentStatus(int appointmentId, string status)
        {
            bool result = await _cabinetService.UpdateAppointmentStatusAsync(GetUserId(), appointmentId, status);

            if (result)
            {
                TempData["Success"] = "Status updated successfully";
            }
            else
            {
                TempData["Error"] = "Failed to update status";
            }

            return RedirectToAction("Cabinet");
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            string userId = GetUserId();
            DoctorProfileEditDto profile = await _cabinetService.GetProfileAsync(userId);

            if (profile == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(profile);
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyProfile(DoctorProfileEditDto dto, IFormFile profileImage)
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
            bool result = await _cabinetService.UpdateProfileAsync(userId, dto);

            if (result)
            {
                TempData["Success"] = "Profile updated successfully";
            }
            else
            {
                TempData["Error"] = "Failed to update profile";
            }

            return RedirectToAction("MyProfile");
        }


        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> CancelAppointment(int appointmentId, string reason)
        {
            string userId = GetUserId();
            Doctor doctor = await _doctorRepository.GetByUserIdAsync(userId);

            if (doctor == null)
            {
                TempData["Error"] = "Doctor not found";
                return RedirectToAction("Cabinet");
            }

            CancelResultDto result = await _appointmentService.CancelByDoctorAsync(appointmentId, doctor.Id, reason);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Cabinet");
            }

            if (!string.IsNullOrEmpty(result.PatientEmail))
            {
                string subject = "Your Appointment Has Been Cancelled";
                string body = "Dear " + result.PatientName + ",<br><br>" +
                              "We regret to inform you that your appointment scheduled for " +
                              result.AppointmentDate.ToString("dd MMM yyyy") + " at " +
                              result.StartTime.ToString(@"hh\:mm") + " with Dr. " + result.DoctorName +
                              " has been cancelled.<br><br>" +
                              "Reason: " + reason + "<br><br>" +
                              "Please book a new appointment at your convenience.<br><br>" +
                              "Best regards,<br>Hospital Management System";

                try
                {
                    await _emailService.SendEmailAsync(result.PatientEmail, subject, body);
                }
                catch
                {
                }
            }

            TempData["Success"] = "Appointment cancelled successfully";
            return RedirectToAction("Cabinet");
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> RescheduleAppointment(int id)
        {
            DetailsAppointmentDto appointment = await _appointmentService.GetByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            RescheduleAppointmentDto dto = new RescheduleAppointmentDto();
            dto.AppointmentId = id;
            dto.NewDate = appointment.AppointmentDate.AddDays(1);

            ViewBag.Appointment = appointment;
            ViewBag.AvailableSlots = await _appointmentService.GetAvailableSlotsAsync(appointment.DoctorId, dto.NewDate);

            return View(dto);
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RescheduleAppointment(RescheduleAppointmentDto dto)
        {
            string userId = GetUserId();
            Doctor doctor = await _doctorRepository.GetByUserIdAsync(userId);

            if (doctor == null)
            {
                TempData["Error"] = "Doctor not found";
                return RedirectToAction("Cabinet");
            }

            if (!ModelState.IsValid)
            {
                DetailsAppointmentDto appointment = await _appointmentService.GetByIdAsync(dto.AppointmentId);
                ViewBag.Appointment = appointment;
                ViewBag.AvailableSlots = await _appointmentService.GetAvailableSlotsAsync(appointment.DoctorId, dto.NewDate);
                return View(dto);
            }

            AppointmentResultDto result = await _appointmentService.RescheduleByDoctorAsync(dto, doctor.Id);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                DetailsAppointmentDto appointment = await _appointmentService.GetByIdAsync(dto.AppointmentId);
                ViewBag.Appointment = appointment;
                ViewBag.AvailableSlots = await _appointmentService.GetAvailableSlotsAsync(appointment.DoctorId, dto.NewDate);
                return View(dto);
            }

            DetailsAppointmentDto updatedAppointment = await _appointmentService.GetByIdAsync(dto.AppointmentId);

            if (!string.IsNullOrEmpty(updatedAppointment.PatientEmail))
            {
                string subject = "Your Appointment Has Been Rescheduled";
                string body = "Dear " + updatedAppointment.PatientName + ",<br><br>" +
                              "Your appointment with Dr. " + updatedAppointment.DoctorName +
                              " has been rescheduled to " + dto.NewDate.ToString("dd MMM yyyy") +
                              " at " + dto.NewStartTime.ToString(@"hh\:mm") + ".<br><br>" +
                              "Reason: " + dto.Reason + "<br><br>" +
                              "Best regards,<br>Hospital Management System";

                try
                {
                    await _emailService.SendEmailAsync(updatedAppointment.PatientEmail, subject, body);
                }
                catch
                {
                }
            }

            TempData["Success"] = "Appointment rescheduled successfully";
            return RedirectToAction("Cabinet");

        }
        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> WritePrescription(int appointmentId)
        {
            DetailsAppointmentDto appointment = await _appointmentService.GetByIdAsync(appointmentId);

            if (appointment == null)
            {
                return NotFound();
            }

            PrescriptionDto existingPrescription = await _prescriptionService.GetByAppointmentIdAsync(appointmentId);

            if (existingPrescription != null)
            {
                TempData["Error"] = "Prescription already exists for this appointment";
                return RedirectToAction("ViewPrescription", new { id = existingPrescription.Id });
            }

            CreatePrescriptionDto dto = new CreatePrescriptionDto
            {
                AppointmentId = appointmentId
            };

            ViewBag.Appointment = appointment;

            return View(dto);
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WritePrescription(CreatePrescriptionDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
            {
                ModelState.AddModelError("", "Please add at least one medicine");
            }

            if (!ModelState.IsValid)
            {
                DetailsAppointmentDto appointment = await _appointmentService.GetByIdAsync(dto.AppointmentId);
                ViewBag.Appointment = appointment;
                return View(dto);
            }

            string userId = GetUserId();
            Doctor doctor = await _doctorRepository.GetByUserIdAsync(userId);

            if (doctor == null)
            {
                TempData["Error"] = "Doctor not found";
                return RedirectToAction("Cabinet");
            }

            bool result = await _prescriptionService.CreateAsync(dto, doctor.Id);

            if (result)
            {
                TempData["Success"] = "Prescription created successfully";
            }
            else
            {
                TempData["Error"] = "Failed to create prescription";
            }

            return RedirectToAction("Cabinet");
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> ViewPrescription(int id)
        {
            PrescriptionDto prescription = await _prescriptionService.GetByIdAsync(id);

            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> MyPrescriptions()
        {
            string userId = GetUserId();
            Doctor doctor = await _doctorRepository.GetByUserIdAsync(userId);

            if (doctor == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<PrescriptionDto> prescriptions = await _prescriptionService.GetByDoctorIdAsync(doctor.Id);

            return View(prescriptions);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<IActionResult> PrintPrescription(int id)
        {
            string html = await _prescriptionService.GenerateHtmlAsync(id);

            if (html == null)
            {
                return NotFound();
            }

            return Content(html, "text/html");
        }
    }
}

