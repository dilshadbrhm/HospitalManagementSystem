using HospitalManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces
{
    public interface ITimeSlotRepository
    {
        Task<IEnumerable<TimeSlot>> GetByDoctorIdAsync(int doctorId);
        Task<TimeSlot?> GetByIdAsync(int id);
        Task AddAsync(TimeSlot timeSlot);
        Task UpdateAsync(TimeSlot timeSlot);
        Task DeleteAsync(int id);
    }
}
