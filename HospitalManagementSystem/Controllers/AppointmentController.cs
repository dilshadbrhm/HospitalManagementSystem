using HospitalManagement.Domain;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;
using HospitalManagement.Infrastructure.Persistence;
using HospitalManagementSystem.ViewModels.Appointment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [Authorize(Roles = "Patient")]
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AppointmentController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Create(int doctorId)
        {
            Doctor? doctor = await _context.Doctors
                .Include(d => d.Department)
                .Include(d => d.TimeSlots)
                .FirstOrDefaultAsync(d => d.Id == doctorId && !d.IsDeleted);

            if (doctor == null)
            {
                return NotFound();
            }

            CreateAppointmentVM appointmentVM = new CreateAppointmentVM
            {
                DoctorId = doctorId,
                Doctor = doctor,
                AppointmentDate = DateTime.Today.AddDays(1)
            };

            return View(appointmentVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAppointmentVM appointmentVM)
        {
            if (!ModelState.IsValid)
            {
                appointmentVM.Doctor = await _context.Doctors
                    .Include(d => d.Department)
                    .FirstOrDefaultAsync(d => d.Id == appointmentVM.DoctorId);
                return View(appointmentVM);
            }

            if (appointmentVM.AppointmentDate < DateTime.Today)
            {
                ModelState.AddModelError("AppointmentDate", "You cannot schedule an appointment for a past date");
                appointmentVM.Doctor = await _context.Doctors
                    .Include(d => d.Department)
                    .FirstOrDefaultAsync(d => d.Id == appointmentVM.DoctorId);
                return View(appointmentVM);
            }

            AppUser? user = await _userManager.GetUserAsync(User);
            Patient? patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (patient == null)
            {
                return BadRequest();
            }

            Doctor? doctor = await _context.Doctors.FindAsync(appointmentVM.DoctorId);
            if (doctor == null)
            {
                return NotFound();
            }

            DayOfWeek dayOfWeek = appointmentVM.AppointmentDate.DayOfWeek;
            TimeSlot? timeSlot = await _context.TimeSlots
                .FirstOrDefaultAsync(ts =>
                    ts.DoctorId == appointmentVM.DoctorId &&
                    ts.DayOfWeek == dayOfWeek &&
                    ts.StartTime <= appointmentVM.StartTime &&
                    ts.EndTime > appointmentVM.StartTime &&
                    ts.IsAvailable);

            if (timeSlot == null)
            {
                ModelState.AddModelError("StartTime", "The doctor is not available at this time");
                appointmentVM.Doctor = await _context.Doctors
                    .Include(d => d.Department)
                    .FirstOrDefaultAsync(d => d.Id == appointmentVM.DoctorId);
                return View(appointmentVM);
            }

            TimeSpan endTime = appointmentVM.StartTime.Add(TimeSpan.FromMinutes(timeSlot.SlotDurationMinutes));

            bool hasConflict = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == appointmentVM.DoctorId &&
                a.AppointmentDate == appointmentVM.AppointmentDate &&
                a.Status != AppointmentStatus.Cancelled &&
                ((a.StartTime < endTime && a.EndTime > appointmentVM.StartTime)));

            if (hasConflict)
            {
                ModelState.AddModelError("StartTime", "This hour is already full, choose another hour");
                appointmentVM.Doctor = await _context.Doctors
                    .Include(d => d.Department)
                    .FirstOrDefaultAsync(d => d.Id == appointmentVM.DoctorId);
                return View(appointmentVM);
            }

            Appointment appointment = new Appointment
            {
                DoctorId = appointmentVM.DoctorId,
                PatientId = patient.Id,
                AppointmentDate = appointmentVM.AppointmentDate,
                StartTime = appointmentVM.StartTime,
                EndTime = endTime,
                Symptoms = appointmentVM.Symptoms,
                Notes = appointmentVM.Notes,
                Fee = doctor.ConsultationFee,
                Status = AppointmentStatus.Pending,
                IsPaid = false
            };

            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Payment", new { id = appointment.Id });
        }

        public async Task<IActionResult> MyAppointments()
        {
            AppUser? user = await _userManager.GetUserAsync(User);
            Patient? patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (patient == null)
            {
                return BadRequest();
            }

            List<Appointment> appointments = await _context.Appointments
                .Where(a => a.PatientId == patient.Id)
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Department)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.StartTime)
                .ToListAsync();

            return View(appointments);
        }

        public async Task<IActionResult> Cancel(int id)
        {
            Appointment? appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            AppUser? user = await _userManager.GetUserAsync(User);
            Patient? patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (appointment.PatientId != patient?.Id)
            {
                return Forbid();
            }

            DateTime appointmentDateTime = appointment.AppointmentDate.Date.Add(appointment.StartTime);
            if (appointmentDateTime.Subtract(DateTime.Now).TotalHours < 24)
            {
                return RedirectToAction("MyAppointments");
            }

            CancelAppointmentVM cancelVM = new CancelAppointmentVM
            {
                AppointmentId = id
            };

            return View(cancelVM);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(CancelAppointmentVM cancelVM)
        {
            if (!ModelState.IsValid)
            {
                return View(cancelVM);
            }

            Appointment? appointment = await _context.Appointments.FindAsync(cancelVM.AppointmentId);
            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.CancellationReason = cancelVM.CancellationReason;
            appointment.CancelledAt = DateTime.Now;
            appointment.UpdatedAt = DateTime.Now;

            if (appointment.IsPaid)
            {
                Payment? payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.AppointmentId == appointment.Id);

                if (payment != null)
                {
                    payment.Status = PaymentStatus.Refunded;
                    payment.Notes = "Meeting was canceled and returned";
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("MyAppointments");
        }

        public async Task<IActionResult> Reschedule(int id)
        {
            Appointment? appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            RescheduleAppointmentVM rescheduleVM = new RescheduleAppointmentVM
            {
                AppointmentId = id,
                NewDate = appointment.AppointmentDate.AddDays(1)
            };

            return View(rescheduleVM);
        }

        [HttpPost]
        public async Task<IActionResult> Reschedule(RescheduleAppointmentVM rescheduleVM)
        {
            if (!ModelState.IsValid)
            {
                return View(rescheduleVM);
            }

            Appointment? appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == rescheduleVM.AppointmentId);

            if (appointment == null)
            {
                return NotFound();
            }

            TimeSlot? timeSlot = await _context.TimeSlots
                .FirstOrDefaultAsync(ts =>
                    ts.DoctorId == appointment.DoctorId &&
                    ts.DayOfWeek == rescheduleVM.NewDate.DayOfWeek &&
                    ts.StartTime <= rescheduleVM.NewStartTime &&
                    ts.IsAvailable);

            if (timeSlot == null)
            {
                ModelState.AddModelError("NewStartTime", "This watch is not available");
                return View(rescheduleVM);
            }

            TimeSpan newEndTime = rescheduleVM.NewStartTime.Add(TimeSpan.FromMinutes(timeSlot.SlotDurationMinutes));

            bool hasConflict = await _context.Appointments.AnyAsync(a =>
                a.Id != rescheduleVM.AppointmentId &&
                a.DoctorId == appointment.DoctorId &&
                a.AppointmentDate == rescheduleVM.NewDate &&
                a.Status != AppointmentStatus.Cancelled &&
                ((a.StartTime < newEndTime && a.EndTime > rescheduleVM.NewStartTime)));

            if (hasConflict)
            {
                ModelState.AddModelError("NewStartTime", "This hour is already full");
                return View(rescheduleVM);
            }

            appointment.AppointmentDate = rescheduleVM.NewDate;
            appointment.StartTime = rescheduleVM.NewStartTime;
            appointment.EndTime = newEndTime;
            appointment.Status = AppointmentStatus.Rescheduled;
            appointment.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction("MyAppointments");
        }

        public async Task<IActionResult> Payment(int id)
        {
            Appointment? appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Department)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }
    }
}
