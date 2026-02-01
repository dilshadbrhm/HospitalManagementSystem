using HospitalManagement.Application.Dtos.Appointment;
using HospitalManagement.Application.Dtos.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces
{
    public interface IPatientService
    {
        Task<PatientDashboardDto> GetDashboardAsync(string userId);
        Task<List<AppointmentItemDto>> GetAppointmentsAsync(string userId);
        Task<PatientDto> GetProfileAsync(string userId);
        Task<bool> UpdateProfileAsync(string userId, PatientDto dto);
    }
}
