using HospitalManagement.Application.Dtos.Appointment;
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
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientRepository _patientService;

        public AppointmentController(IAppointmentService appointmentService, IPatientRepository patientRepository)
        {
            _appointmentService = appointmentService;
            _patientService = patientRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int patientId = await GetCurrentPatientIdAsync();

            if (patientId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            List<AppointmentItemDto> appointments = await _appointmentService.GetByPatientIdAsync(patientId);
            return View(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            DetailsAppointmentDto appointment = await _appointmentService.GetByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            CreateAppointmentDto dto = new CreateAppointmentDto();
            dto.Departments = await _appointmentService.GetDepartmentsAsync();
            dto.AppointmentDate = DateTime.Now.Date.AddDays(1);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAppointmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                dto.Departments = await _appointmentService.GetDepartmentsAsync();

                if (dto.DepartmentId > 0)
                {
                    dto.Doctors = await _appointmentService.GetDoctorsByDepartmentAsync(dto.DepartmentId);
                }

                if (dto.DoctorId > 0 && dto.AppointmentDate != default)
                {
                    dto.AvailableSlots = await _appointmentService.GetAvailableSlotsAsync(dto.DoctorId, dto.AppointmentDate);
                }

                return View(dto);
            }

            int patientId = await GetCurrentPatientIdAsync();

            if (patientId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            AppointmentResultDto result = await _appointmentService.CreateAsync(dto, patientId);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);

                dto.Departments = await _appointmentService.GetDepartmentsAsync();
                dto.Doctors = await _appointmentService.GetDoctorsByDepartmentAsync(dto.DepartmentId);
                dto.AvailableSlots = await _appointmentService.GetAvailableSlotsAsync(dto.DoctorId, dto.AppointmentDate);

                if (result.AlternativeSlots != null && result.AlternativeSlots.Count > 0)
                {
                    ViewBag.AlternativeSlots = result.AlternativeSlots;
                }

                return View(dto);
            }

            TempData["Success"] = "The view was created successfully";
            return RedirectToAction("Details", new { id = result.AppointmentId });
        }

        [HttpGet]
        public async Task<IActionResult> Cancel(int id)
        {
            DetailsAppointmentDto appointment = await _appointmentService.GetByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            CancelAppointmentDto dto = new CancelAppointmentDto();
            dto.AppointmentId = id;

            ViewBag.Appointment = appointment;
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(CancelAppointmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                DetailsAppointmentDto appointment = await _appointmentService.GetByIdAsync(dto.AppointmentId);
                ViewBag.Appointment = appointment;
                return View(dto);
            }

            bool result = await _appointmentService.CancelAsync(dto);

            if (!result)
            {
                TempData["Error"] = "The meeting could not be canceled";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "The meeting was successfully canceled";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctorsByDepartment(int departmentId)
        {
            List<DoctorSelectDto> doctors = await _appointmentService.GetDoctorsByDepartmentAsync(departmentId);
            return Json(doctors);
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(int doctorId, DateTime date)
        {
            List<TimeSlotSelectDto> slots = await _appointmentService.GetAvailableSlotsAsync(doctorId, date);
            return Json(slots);
        }

        [HttpGet]
        public async Task<IActionResult> GetAlternativeSlots(int doctorId, DateTime date, TimeSpan time)
        {
            List<AlternativeSlotDto> slots = await _appointmentService.GetAlternativeSlotsAsync(doctorId, date, time);
            return Json(slots);
        }

        private async Task<int> GetCurrentPatientIdAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || userId == "")
            {
                throw new Exception("User not found");
            }

            Patient patient = await _patientService.GetByUserIdAsync(userId);

            if (patient == null)
            {
                throw new Exception("Patient not found");
            }
            return patient.Id;
        }
    }
}
