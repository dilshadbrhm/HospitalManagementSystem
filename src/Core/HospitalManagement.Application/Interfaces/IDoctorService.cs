using HospitalManagement.Application.Dtos.Doctor;
using HospitalManagement.Application.Dtos.Timeslot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces
{
    public interface IDoctorService
    {
        Task<List<DoctorItemDto>> GetAllDoctorsAsync();
        Task<DoctorProfileDto> GetDoctorProfileAsync(int id);
        Task<List<TimeSlotDto>> GetDoctorTimeSlotsAsync(int doctorId);
    }
}
