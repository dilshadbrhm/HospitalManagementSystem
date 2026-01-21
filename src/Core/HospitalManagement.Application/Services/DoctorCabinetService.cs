using HospitalManagement.Application.Dtos.Appointment;
using HospitalManagement.Application.Dtos.Doctor;
using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Services
{
    public class DoctorCabinetService : IDoctorCabinetService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly ITimeSlotRepository _timeSlotRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public DoctorCabinetService(
            IDoctorRepository doctorRepository,
            ITimeSlotRepository timeSlotRepository,
            IAppointmentRepository appointmentRepository)
        {
            _doctorRepository = doctorRepository;
            _timeSlotRepository = timeSlotRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<DoctorCabinetDto> GetCabinetAsync(string userId)
        {
            Doctor doctor = await _doctorRepository.GetByUserIdAsync(userId);
            if (doctor is null) return null;

            IEnumerable<Appointment> appointments = await _appointmentRepository.GetByDoctorIdAsync(doctor.Id);
            DateTime today = DateTime.Today;

            List<Appointment> todayAppointments = appointments
                .Where(a => a.AppointmentDate.Date == today)
                .ToList();

            List<Appointment> upcomingAppointments = appointments
                .Where(a => a.AppointmentDate.Date > today)
                .Take(10)
                .ToList();

            return new DoctorCabinetDto
            {
                DoctorName = doctor.FirstName + " " + doctor.LastName,
                Specialization = doctor.Specialization,
                TodayAppointments = todayAppointments.Select(a => new AppointmentItemDto
                {
                    Id = a.Id,
                    PatientName = a.Patient.FirstName + " " + a.Patient.LastName,
                    Date = a.AppointmentDate,
                    StartTime = a.StartTime,
                    Status = a.Status.ToString(),
                    Symptoms = a.Symptoms
                }).ToList(),
                UpcomingAppointments = upcomingAppointments.Select(a => new AppointmentItemDto
                {
                    Id = a.Id,
                    PatientName = a.Patient.FirstName + " " + a.Patient.LastName,
                    Date = a.AppointmentDate,
                    StartTime = a.StartTime,
                    Status = a.Status.ToString(),
                    Symptoms = a.Symptoms
                }).ToList()
            };
        }

        public async Task<List<TimeSlotDto>> GetTimeSlotsAsync(string userId)
        {
            Doctor doctor = await _doctorRepository.GetByUserIdAsync(userId);
            if (doctor is null) return new List<TimeSlotDto>();

            IEnumerable<TimeSlot> slots = await _timeSlotRepository.GetByDoctorIdAsync(doctor.Id);
            string[] dayNames = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

            return slots.Select(s => new TimeSlotDto
            {
                Id = s.Id,
                DayOfWeek = (int)s.DayOfWeek,
                DayName = dayNames[(int)s.DayOfWeek],
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Location = s.Location,
                IsAvailable = s.IsAvailable
            }).ToList();
        }

        public async Task<TimeSlotDto> GetTimeSlotByIdAsync(string userId, int id)
        {
            Doctor doctor = await _doctorRepository.GetByUserIdAsync(userId);
            if (doctor is null) return null;

            TimeSlot slot = await _timeSlotRepository.GetByIdAsync(id);
            if (slot is null || slot.DoctorId != doctor.Id) return null;

            string[] dayNames = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

            return new TimeSlotDto
            {
                Id = slot.Id,
                DayOfWeek = (int)slot.DayOfWeek,
                DayName = dayNames[(int)slot.DayOfWeek],
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                Location = slot.Location,
                IsAvailable = slot.IsAvailable
            };
        }

        public async Task<bool> AddTimeSlotAsync(string userId, CreateTimeSlotDto dto)
        {
            Doctor doctor = await _doctorRepository.GetByUserIdAsync(userId);
            if (doctor is null) return false;

            TimeSlot slot = new TimeSlot
            {
                DoctorId = doctor.Id,
                DayOfWeek = (DayOfWeek)dto.DayOfWeek,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Location = dto.Location,
                IsAvailable = true
            };

            await _timeSlotRepository.AddAsync(slot);
            return true;
        }

        public async Task<bool> UpdateTimeSlotAsync(string userId, TimeSlotDto dto)
        {
            Doctor doctor = await _doctorRepository.GetByUserIdAsync(userId);
            if (doctor is null) return false;

            TimeSlot slot = await _timeSlotRepository.GetByIdAsync(dto.Id);
            if (slot is null || slot.DoctorId != doctor.Id) return false;

            slot.DayOfWeek = (DayOfWeek)dto.DayOfWeek;
            slot.StartTime = dto.StartTime;
            slot.EndTime = dto.EndTime;
            slot.Location = dto.Location;
            slot.IsAvailable = dto.IsAvailable;

            await _timeSlotRepository.UpdateAsync(slot);
            return true;
        }

        public async Task<bool> DeleteTimeSlotAsync(string userId, int id)
        {
            Doctor doctor = await _doctorRepository.GetByUserIdAsync(userId);
            if (doctor is null) return false;

            TimeSlot slot = await _timeSlotRepository.GetByIdAsync(id);
            if (slot is null || slot.DoctorId != doctor.Id) return false;

            await _timeSlotRepository.DeleteAsync(id);
            return true;
        }
    }
}
