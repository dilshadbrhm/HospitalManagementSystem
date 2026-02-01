using HospitalManagement.Application.Dtos.Appointment;
using HospitalManagement.Application.Dtos.Patient;
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
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public PatientService(IPatientRepository patientRepository, IAppointmentRepository appointmentRepository)
        {
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<PatientDashboardDto> GetDashboardAsync(string userId)
        {
            Patient patient = await _patientRepository.GetByUserIdAsync(userId);

            if (patient == null)
            {
                return null;
            }

            IEnumerable<Appointment> appointments = await _appointmentRepository.GetByPatientIdAsync(patient.Id);
            List<Appointment> appointmentList = appointments.ToList();

            DateTime today = DateTime.Today;

            List<Appointment> upcomingList = appointmentList
                .Where(a => a.AppointmentDate.Date >= today && a.Status != AppointmentStatus.Cancelled)
                .OrderBy(a => a.AppointmentDate)
                .Take(5)
                .ToList();

            List<Appointment> pastList = appointmentList
                .Where(a => a.AppointmentDate.Date < today || a.Status == AppointmentStatus.Completed)
                .OrderByDescending(a => a.AppointmentDate)
                .Take(5)
                .ToList();

            PatientDashboardDto dashboardDto = new PatientDashboardDto
            {
                PatientName = patient.FirstName + " " + patient.LastName,
                Email = patient.Email,
                Phone = patient.Phone,
                TotalAppointments = appointmentList.Count,
                UpcomingCount = appointmentList.Count(a => a.AppointmentDate.Date >= today && a.Status != AppointmentStatus.Cancelled),
                CompletedCount = appointmentList.Count(a => a.Status == AppointmentStatus.Completed),
                CancelledCount = appointmentList.Count(a => a.Status == AppointmentStatus.Cancelled),
                UpcomingAppointments = new List<AppointmentItemDto>(),
                PastAppointments = new List<AppointmentItemDto>()
            };

            foreach (Appointment appointment in upcomingList)
            {
                AppointmentItemDto itemDto = MapToItemDto(appointment);
                dashboardDto.UpcomingAppointments.Add(itemDto);
            }

            foreach (Appointment appointment in pastList)
            {
                AppointmentItemDto itemDto = MapToItemDto(appointment);
                dashboardDto.PastAppointments.Add(itemDto);
            }

            return dashboardDto;
        }

        public async Task<List<AppointmentItemDto>> GetAppointmentsAsync(string userId)
        {
            Patient patient = await _patientRepository.GetByUserIdAsync(userId);

            if (patient == null)
            {
                return new List<AppointmentItemDto>();
            }

            IEnumerable<Appointment> appointments = await _appointmentRepository.GetByPatientIdAsync(patient.Id);
            List<AppointmentItemDto> result = new List<AppointmentItemDto>();

            foreach (Appointment appointment in appointments.OrderByDescending(a => a.AppointmentDate))
            {
                AppointmentItemDto itemDto = MapToItemDto(appointment);
                result.Add(itemDto);
            }

            return result;
        }

        public async Task<PatientDto> GetProfileAsync(string userId)
        {
            Patient patient = await _patientRepository.GetByUserIdAsync(userId);

            if (patient == null)
            {
                return null;
            }

            PatientDto dto = new PatientDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.Email,
                Phone = patient.Phone,
                DateOfBirth = patient.DateOfBirth,
                Address = patient.Address,
                ProfilePicture = patient.ProfilePicture
            };

            return dto;
        }

        public async Task<bool> UpdateProfileAsync(string userId, PatientDto dto)
        {
            Patient patient = await _patientRepository.GetByUserIdAsync(userId);

            if (patient == null)
            {
                return false;
            }

            patient.FirstName = dto.FirstName;
            patient.LastName = dto.LastName;
            patient.Phone = dto.Phone;
            patient.DateOfBirth = dto.DateOfBirth;
            patient.Address = dto.Address;

            if (!string.IsNullOrEmpty(dto.ProfilePicture))
            {
                patient.ProfilePicture = dto.ProfilePicture;
            }

            await _patientRepository.UpdateAsync(patient);
            return true;
        }

        private AppointmentItemDto MapToItemDto(Appointment appointment)
        {
            AppointmentItemDto dto = new AppointmentItemDto
            {
                Id = appointment.Id,
                AppointmentDate = appointment.AppointmentDate,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status.ToString(),
                Fee = appointment.Fee,
                IsPaid = appointment.IsPaid,
                Symptoms = appointment.Symptoms
            };

            if (appointment.Doctor != null)
            {
                dto.DoctorName = appointment.Doctor.FirstName + " " + appointment.Doctor.LastName;

                if (appointment.Doctor.Department != null)
                {
                    dto.DepartmentName = appointment.Doctor.Department.Name;
                }
            }

            return dto;
        }
    }
}
