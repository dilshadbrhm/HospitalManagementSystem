using HospitalManagement.Application.Interfaces.Generic;
using HospitalManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Appointment> Appointments { get; }
        IGenericRepository<Doctor> Doctors { get; }
        IDoctorRepository DoctorRepository { get; }
        IGenericRepository<Department> Departments { get; }
        IGenericRepository<Patient> Patients { get; }
        IGenericRepository<TimeSlot> TimeSlots { get; }
        Task<int> SaveChangesAsync();
    }
}
