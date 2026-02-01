using HospitalManagement.Application.Dtos.Doctor;
using HospitalManagement.Application.Dtos.Timeslot;
using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;
using HospitalManagement.Infrastructure.Persistence;
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

        public DoctorController(IDoctorCabinetService cabinetService, IDoctorService doctorService)
        {
            _cabinetService = cabinetService;
            _doctorService = doctorService;
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
            if (id == null || id == 0)
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
        public async Task<IActionResult> Profile()
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
        public async Task<IActionResult> Profile(DoctorProfileEditDto dto, IFormFile profileImage)
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

            return RedirectToAction("Profile");
        }
    }
}

