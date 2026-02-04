using HospitalManagement.Application.Interfaces;
using HospitalManagement.Application.Interfaces.Generic;
using HospitalManagement.Domain;
using HospitalManagement.Infrastructure.Persistence.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IGenericRepository<Appointment> _appointments;
        private IAppointmentRepository _appointmentRepository;
        private IGenericRepository<Doctor> _doctors;
        private IDoctorRepository _doctorRepository;
        private IGenericRepository<Department> _departments;
        private IGenericRepository<Patient> _patients;
        private IGenericRepository<TimeSlot> _timeSlots;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<Appointment> Appointments
        {
            get
            {
                if (_appointments == null)
                {
                    _appointments = new GenericRepository<Appointment>(_context);
                }
                return _appointments;
            }
        }

        public IAppointmentRepository AppointmentRepository
        {
            get
            {
                if (_appointmentRepository == null)
                {
                    _appointmentRepository = new AppointmentRepository(_context);
                }
                return _appointmentRepository;
            }
        }

        public IGenericRepository<Doctor> Doctors
        {
            get
            {
                if (_doctors == null)
                {
                    _doctors = new GenericRepository<Doctor>(_context);
                }
                return _doctors;
            }
        }

        public IDoctorRepository DoctorRepository
        {
            get
            {
                if (_doctorRepository == null)
                {
                    _doctorRepository = new DoctorRepository(_context);
                }
                return _doctorRepository;
            }
        }

        public IGenericRepository<Department> Departments
        {
            get
            {
                if (_departments == null)
                {
                    _departments = new GenericRepository<Department>(_context);
                }
                return _departments;
            }
        }

        public IGenericRepository<Patient> Patients
        {
            get
            {
                if (_patients == null)
                {
                    _patients = new GenericRepository<Patient>(_context);
                }
                return _patients;
            }
        }

        public IGenericRepository<TimeSlot> TimeSlots
        {
            get
            {
                if (_timeSlots == null)
                {
                    _timeSlots = new GenericRepository<TimeSlot>(_context);
                }
                return _timeSlots;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

