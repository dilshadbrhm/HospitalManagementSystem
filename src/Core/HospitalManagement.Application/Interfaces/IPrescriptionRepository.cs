using HospitalManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces
{
    public interface IPrescriptionRepository
    {
        Task<Prescription> GetByIdAsync(int id);
        Task<Prescription> GetByAppointmentIdAsync(int appointmentId);
        Task<IEnumerable<Prescription>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Prescription>> GetByDoctorIdAsync(int doctorId);
        Task AddAsync(Prescription prescription);
        Task UpdateAsync(Prescription prescription);
        Task DeleteAsync(int id);
    }
}
