using HospitalManagement.Application.Dtos.Doctor;
using HospitalManagement.Application.Dtos.Timeslot;
using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain;
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
                DoctorItemDto dto = new DoctorItemDto();
                dto.Id = doctor.Id;
                dto.FirstName = doctor.FirstName;
                dto.LastName = doctor.LastName;
                dto.FullName = doctor.FirstName + " " + doctor.LastName;
                dto.Specialization = doctor.Specialization;
                dto.ProfilePicture = doctor.ProfilePicture;
                dto.Bio = doctor.Bio;

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

            DoctorProfileDto dto = new DoctorProfileDto();
            dto.Id = doctor.Id;
            dto.FirstName = doctor.FirstName;
            dto.LastName = doctor.LastName;
            dto.FullName = doctor.FirstName + " " + doctor.LastName;
            dto.Email = doctor.Email;
            dto.Phone = doctor.Phone;
            dto.Specialization = doctor.Specialization;
            dto.ProfilePicture = doctor.ProfilePicture;
            dto.Bio = doctor.Bio;
            dto.ConsultationFee = doctor.ConsultationFee;

            if (doctor.Department != null)
            {
                dto.DepartmentName = doctor.Department.Name;
            }

            dto.TimeSlots = new List<TimeSlotDto>();

            foreach (TimeSlot slot in timeSlots)
            {
                if (slot.IsAvailable)
                {
                    TimeSlotDto slotDto = new TimeSlotDto();
                    slotDto.Id = slot.Id;
                    slotDto.DayOfWeek = (int)slot.DayOfWeek;
                    slotDto.DayName = dayNames[(int)slot.DayOfWeek];
                    slotDto.StartTime = slot.StartTime;
                    slotDto.EndTime = slot.EndTime;
                    slotDto.Location = slot.Location;
                    slotDto.IsAvailable = slot.IsAvailable;

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
                    TimeSlotDto dto = new TimeSlotDto();
                    dto.Id = slot.Id;
                    dto.DayOfWeek = (int)slot.DayOfWeek;
                    dto.DayName = dayNames[(int)slot.DayOfWeek];
                    dto.StartTime = slot.StartTime;
                    dto.EndTime = slot.EndTime;
                    dto.Location = slot.Location;
                    dto.IsAvailable = slot.IsAvailable;

                    result.Add(dto);
                }
            }

            return result;
        }
    }
}
