using HospitalManagement.Application.Dtos.Doctor;
using HospitalManagement.Application.Dtos.Timeslot;
using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain;
using HospitalManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly ITimeSlotRepository _timeSlotRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public DoctorService(IDoctorRepository doctorRepository, ITimeSlotRepository timeSlotRepository)
        {
            _doctorRepository = doctorRepository;
            _timeSlotRepository = timeSlotRepository;
        }

        public async Task<List<DoctorItemDto>> GetAllDoctorsAsync()
        {
            IEnumerable<Doctor> doctors = await _doctorRepository.GetAllWithDepartmentAsync();

            List<DoctorItemDto> result = new List<DoctorItemDto>();

            foreach (Doctor doctor in doctors)
            {
                DoctorItemDto dto = new DoctorItemDto
                {
                    Id = doctor.Id,
                    FirstName = doctor.FirstName,
                    LastName = doctor.LastName,
                    FullName = doctor.FirstName + " " + doctor.LastName,
                    Specialization = doctor.Specialization,
                    ProfilePicture = doctor.ProfilePicture,
                    Bio = doctor.Bio
                };
                

                if (doctor.Department != null)
                {
                    dto.DepartmentName = doctor.Department.Name;
                }

                result.Add(dto);
            }

            return result;
        }

        public async Task<DoctorProfileDto> GetDoctorProfileAsync(int id)
        {
            Doctor doctor = await _doctorRepository.GetByIdAsync(id);

            if (doctor == null)
            {
                return null;
            }

            IEnumerable<TimeSlot> timeSlots = await _timeSlotRepository.GetByDoctorIdAsync(id);

            string[] dayNames = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

            DoctorProfileDto dto = new DoctorProfileDto
            {
               Id = doctor.Id,
               FirstName = doctor.FirstName,
               LastName = doctor.LastName,
               FullName = doctor.FirstName + " " + doctor.LastName,
               Email = doctor.Email,
               Phone = doctor.Phone,
               Specialization = doctor.Specialization,
               ProfilePicture = doctor.ProfilePicture,
               Bio = doctor.Bio,
               ConsultationFee = doctor.ConsultationFee
            };
            

            if (doctor.Department != null)
            {
                dto.DepartmentName = doctor.Department.Name;
            }

            dto.TimeSlots = new List<TimeSlotDto>();

            foreach (TimeSlot slot in timeSlots)
            {
                if (slot.IsAvailable)
                {
                    TimeSlotDto slotDto = new TimeSlotDto
                    {
                        Id = slot.Id,
                        DayOfWeek = (int)slot.DayOfWeek,
                        DayName = dayNames[(int)slot.DayOfWeek],
                        StartTime = slot.StartTime,
                        EndTime = slot.EndTime,
                        Location = slot.Location,
                        IsAvailable = slot.IsAvailable
                    };
                   

                    dto.TimeSlots.Add(slotDto);
                }
            }

            return dto;
        }

        public async Task<List<TimeSlotDto>> GetDoctorTimeSlotsAsync(int doctorId)
        {
            IEnumerable<TimeSlot> timeSlots = await _timeSlotRepository.GetByDoctorIdAsync(doctorId);

            string[] dayNames = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

            List<TimeSlotDto> result = new List<TimeSlotDto>();

            foreach (TimeSlot slot in timeSlots)
            {
                if (slot.IsAvailable)
                {
                    TimeSlotDto dto = new TimeSlotDto
                    {
                       Id = slot.Id,
                       DayOfWeek = (int)slot.DayOfWeek,
                       DayName = dayNames[(int)slot.DayOfWeek],
                       StartTime = slot.StartTime,
                       EndTime = slot.EndTime,
                       Location = slot.Location,
                       IsAvailable = slot.IsAvailable
                    };
                   

                    result.Add(dto);
                }
            }

            return result;
        }
        public async Task<DoctorProfileDto> GetDoctorScheduleAsync(int doctorId, DateTime date)
        {
            Doctor doctor = await _doctorRepository.GetByIdWithTimeSlotsAsync(doctorId);

            if (doctor == null)
            {
                return null;
            }

            DayOfWeek dayOfWeek = date.DayOfWeek;

            List<Appointment> bookedAppointments = await _appointmentRepository
                .GetAppointmentsByDoctorAndDateAsync(doctorId, date);

            List<TimeSlotDto> timeSlots = new List<TimeSlotDto>();

            if (doctor.TimeSlots != null)
            {
                foreach (TimeSlot slot in doctor.TimeSlots.Where(t => (int)t.DayOfWeek == (int)dayOfWeek))
                {
                    bool isBooked = bookedAppointments.Any(a =>
                        a.StartTime == slot.StartTime && a.Status != AppointmentStatus.Cancelled);

                    TimeSlotDto slotDto = new TimeSlotDto
                    {
                        Id = slot.Id,
                        DayOfWeek = (int)slot.DayOfWeek,
                        DayName = date.DayOfWeek.ToString(),
                        StartTime = slot.StartTime,
                        EndTime = slot.EndTime,
                        Location = slot.Location,
                        IsAvailable = !isBooked
                    };

                    timeSlots.Add(slotDto);
                }
            }

            DoctorProfileDto profileDto = new DoctorProfileDto
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                FullName = doctor.FirstName + " " + doctor.LastName,
                Email = doctor.Email,
                Phone = doctor.Phone,
                Specialization = doctor.Specialization,
                DepartmentName = doctor.Department?.Name,
                ProfilePicture = doctor.ProfilePicture,
                Bio = doctor.Bio,
                ConsultationFee = doctor.ConsultationFee,
                TimeSlots = timeSlots.OrderBy(t => t.StartTime).ToList()
            };

            return profileDto;
        }

        public async Task<List<TimeSlotSelectDto>> GetAvailableSlotsAsync(int doctorId, DateTime date)
        {
            Doctor doctor = await _doctorRepository.GetByIdWithTimeSlotsAsync(doctorId);

            if (doctor == null)
            {
                return new List<TimeSlotSelectDto>();
            }

            int dayOfWeek = (int)date.DayOfWeek;

            List<Appointment> bookedAppointments = await _appointmentRepository
                .GetAppointmentsByDoctorAndDateAsync(doctorId, date);

            List<TimeSlotSelectDto> slots = new List<TimeSlotSelectDto>();

            if (doctor.TimeSlots != null)
            {
                foreach (TimeSlot slot in doctor.TimeSlots.Where(t => (int)t.DayOfWeek == (int)dayOfWeek))
                {
                    bool isBooked = bookedAppointments.Any(a =>
                        a.StartTime == slot.StartTime && a.Status != AppointmentStatus.Cancelled);

                    TimeSlotSelectDto selectDto = new TimeSlotSelectDto
                    {
                        StartTime = slot.StartTime,
                        EndTime = slot.EndTime,
                        IsAvailable = !isBooked
                    };

                    slots.Add(selectDto);
                }
            }

            return slots.OrderBy(s => s.StartTime).ToList();
        }
    }
}
