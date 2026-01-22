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
    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        private readonly IDoctorCabinetService _cabinetService;

        public DoctorController(IDoctorCabinetService cabinetService)
        {
            _cabinetService = cabinetService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<IActionResult> Cabinet()
        {
            DoctorCabinetDto result = await _cabinetService.GetCabinetAsync(GetUserId());

            if (result == null) return RedirectToAction("Index", "Home");

            return View(result);
        }

        public async Task<IActionResult> TimeTable()
        {
            List<TimeSlotDto> result = await _cabinetService.GetTimeSlotsAsync(GetUserId());

            return View(result);
        }

        public IActionResult AddTimeSlot()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddTimeSlot(CreateTimeSlotDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            bool result = await _cabinetService.AddTimeSlotAsync(GetUserId(), dto);

            if (!result)
            {
                ModelState.AddModelError("", "Something went wrong");
                return View(dto);
            }

            return RedirectToAction("TimeTable");
        }

        public async Task<IActionResult> EditTimeSlot(int id)
        {
            TimeSlotDto result = await _cabinetService.GetTimeSlotByIdAsync(GetUserId(), id);

            if (result == null) return RedirectToAction("TimeTable");

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditTimeSlot(TimeSlotDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            bool result = await _cabinetService.UpdateTimeSlotAsync(GetUserId(), dto);

            if (!result)
            {
                ModelState.AddModelError("", "Something went wrong");
                return View(dto);
            }

            return RedirectToAction("TimeTable");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTimeSlot(int id)
        {
            await _cabinetService.DeleteTimeSlotAsync(GetUserId(), id);

            return RedirectToAction("TimeTable");
        }
    }
}

