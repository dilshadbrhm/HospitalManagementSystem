using HospitalManagement.Application.Dtos.Doctor;
using HospitalManagement.Application.Dtos.Timeslot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces
{
    public interface IDoctorCabinetService
    {
        Task<DoctorCabinetDto> GetCabinetAsync(string userId);
        Task<List<TimeSlotDto>> GetTimeSlotsAsync(string userId);
        Task<TimeSlotDto> GetTimeSlotByIdAsync(string userId, int id);
        Task<bool> AddTimeSlotAsync(string userId, CreateTimeSlotDto dto);
        Task<bool> UpdateTimeSlotAsync(string userId, TimeSlotDto dto);
        Task<bool> DeleteTimeSlotAsync(string userId, int id);
        Task<bool> UpdateAppointmentStatusAsync(string userId, int appointmentId, string status);
    }
}
