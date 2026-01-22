using HospitalManagement.Application.Dtos.Appointment;
using HospitalManagement.Application.Dtos.Department;
using HospitalManagement.Application.Dtos.Doctor;
using HospitalManagement.Application.Dtos.Timeslot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<DetailsAppointmentDto> GetByIdAsync(int id);
        Task<List<AppointmentItemDto>> GetAllAsync();
        Task<List<AppointmentItemDto>> GetByPatientIdAsync(int patientId);
        Task<List<AppointmentItemDto>> GetByDoctorIdAsync(int doctorId);
        Task<AppointmentResultDto> CreateAsync(CreateAppointmentDto dto, int patientId);
        Task<bool> CancelAsync(CancelAppointmentDto dto);
        Task<bool> UpdateStatusAsync(int appointmentId, string status);
        Task<bool> IsSlotAvailableAsync(int doctorId, DateTime date, TimeSpan startTime);
        Task<List<AlternativeSlotDto>> GetAlternativeSlotsAsync(int doctorId, DateTime date, TimeSpan requestedTime);
        Task<List<TimeSlotSelectDto>> GetAvailableSlotsAsync(int doctorId, DateTime date);
        Task<List<DepartmentSelectDto>> GetDepartmentsAsync();
        Task<List<DoctorSelectDto>> GetDoctorsByDepartmentAsync(int departmentId);
    }
}
