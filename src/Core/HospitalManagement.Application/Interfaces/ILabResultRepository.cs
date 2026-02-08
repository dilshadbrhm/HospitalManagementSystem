using HospitalManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces
{
    public interface ILabResultRepository
    {
        Task<IEnumerable<LabResult>> GetAllAsync();
        Task<IEnumerable<LabResult>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<LabResult>> GetByDoctorIdAsync(int doctorId);
        Task<LabResult> GetByIdAsync(int id);
        Task AddAsync(LabResult labResult);
        Task UpdateAsync(LabResult labResult);
        Task DeleteAsync(int id);
    }
}
