using HospitalManagement.Application.Dtos.Appointment;
using HospitalManagement.Application.Dtos.Doctor;
using HospitalManagement.Application.Dtos.Timeslot;
using HospitalManagement.Application.Interfaces;
using HospitalManagement.Application.Services;
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
        private readonly IDoctorService _doctorService;

        public AppointmentController(IAppointmentService appointmentService, IPatientRepository patientRepository,IDoctorService doctorService)
        {
            _appointmentService = appointmentService;
            _patientService = patientRepository;
            _doctorService = doctorService;

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Doctor"))
            {
                return RedirectToAction("Cabinet", "Doctor");
            }

            int patientId = await GetCurrentPatientIdAsync();

            if (patientId == 0)
            {
                TempData["Error"] = "Patient profile not found.";
                return RedirectToAction("Index", "Home");
            }

            List<AppointmentItemDto> appointments = await _appointmentService.GetByPatientIdAsync(patientId);
            return View(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id == 0)
            {
                int patientId = await GetCurrentPatientIdAsync();
                List<AppointmentItemDto> appointments = await _appointmentService.GetByPatientIdAsync(patientId);
                return View("Index", appointments); 
            }

            DetailsAppointmentDto appointment = await _appointmentService.GetByIdAsync(id.Value);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (User.IsInRole("Doctor"))
            {
                TempData["Error"] = "Doctors cannot book appointments.";
                return RedirectToAction("Cabinet", "Doctor");
            }

            int patientId = await GetCurrentPatientIdAsync();

            if (patientId == 0)
            {
                TempData["Error"] = "Patient profile not found.";
                return RedirectToAction("Index", "Home");
            }

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
                string errors = "";
                foreach (var key in ModelState.Keys)
                {
                    foreach (var error in ModelState[key].Errors)
                    {
                        errors = errors + key + ": " + error.ErrorMessage + " | ";
                    }
                }
                TempData["Error"] = errors;

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

            try
            {
                int patientId = await GetCurrentPatientIdAsync();

                AppointmentResultDto result = await _appointmentService.CreateAsync(dto, patientId);

                if (!result.Success)
                {
                    TempData["Error"] = result.Message;

                    dto.Departments = await _appointmentService.GetDepartmentsAsync();
                    dto.Doctors = await _appointmentService.GetDoctorsByDepartmentAsync(dto.DepartmentId);
                    dto.AvailableSlots = await _appointmentService.GetAvailableSlotsAsync(dto.DoctorId, dto.AppointmentDate);

                    if (result.AlternativeSlots != null && result.AlternativeSlots.Count > 0)
                    {
                        ViewBag.AlternativeSlots = result.AlternativeSlots;
                    }

                    return View(dto);
                }
                TempData["Success"] = "Appointment created successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;

                if (ex.InnerException != null)
                {
                    errorMessage = errorMessage + " | Inner: " + ex.InnerException.Message;
                }

                TempData["Error"] = errorMessage;
                dto.Departments = await _appointmentService.GetDepartmentsAsync();
                return View(dto);
            }
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

            if (string.IsNullOrEmpty(userId))
            {
                return 0;
            }

            Patient patient = await _patientService.GetByUserIdAsync(userId);

            if (patient == null)
            {
                return 0;
            }

            return patient.Id;
        }
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> DoctorSchedule(int doctorId, string date)
        {
            DateTime selectedDate = string.IsNullOrEmpty(date)
                ? DateTime.Today
                : DateTime.Parse(date);

            DoctorProfileDto schedule = await _doctorService.GetDoctorScheduleAsync(doctorId, selectedDate);

            if (schedule == null)
            {
                return NotFound();
            }

            List<DateTime> availableDates = GetNext14Days();
            Dictionary<DateTime, int> slotsCount = await _appointmentService.GetAvailableSlotsCountByDatesAsync(doctorId, availableDates);

            ViewBag.SelectedDate = selectedDate;
            ViewBag.AvailableDates = availableDates;
            ViewBag.SlotsCount = slotsCount;

            return View(schedule);
        }
        private List<DateTime> GetNext14Days()
        {
            List<DateTime> dates = new List<DateTime>();

            for (int i = 0; i < 14; i++)
            {
                DateTime checkDate = DateTime.Today.AddDays(i);

                if (checkDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    dates.Add(checkDate);
                }
            }

            return dates;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllSlotsWithStatus(int doctorId, DateTime date)
        {
            List<TimeSlotSelectDto> slots = await _appointmentService.GetAllSlotsWithStatusAsync(doctorId, date);
            return Json(slots);
        }
    }
}
