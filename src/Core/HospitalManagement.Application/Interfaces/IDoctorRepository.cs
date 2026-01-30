using HospitalManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<IEnumerable<Doctor>> GetAllWithDepartmentAsync();
        Task<Doctor> GetByIdAsync(int id);
        Task<Doctor> GetByUserIdAsync(string userId);
        Task AddAsync(Doctor doctor);
        Task UpdateAsync(Doctor doctor);
        Task DeleteAsync(int id);
        Task<Doctor> GetByIdWithTimeSlotsAsync(int id);
    }
}
