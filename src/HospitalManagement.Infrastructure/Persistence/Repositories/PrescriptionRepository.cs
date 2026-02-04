using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Infrastructure.Persistence.Repositories
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly AppDbContext _context;

        public PrescriptionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Prescription> GetByIdAsync(int id)
        {
            return await _context.Prescriptions
                .Include(p => p.Items)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d.Department)
                .Include(p => p.Patient)
                .Include(p => p.Appointment)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Prescription> GetByAppointmentIdAsync(int appointmentId)
        {
            return await _context.Prescriptions
                .Include(p => p.Items)
                .Include(p => p.Doctor)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(p => p.AppointmentId == appointmentId);
        }

        public async Task<IEnumerable<Prescription>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Prescriptions
                .Where(p => p.PatientId == patientId && !p.IsDeleted)
                .Include(p => p.Items)
                .Include(p => p.Doctor)
                .Include(p => p.Appointment)
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.Prescriptions
                .Where(p => p.DoctorId == doctorId && !p.IsDeleted)
                .Include(p => p.Items)
                .Include(p => p.Patient)
                .Include(p => p.Appointment)
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();
        }

        public async Task AddAsync(Prescription prescription)
        {
            await _context.Prescriptions.AddAsync(prescription);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Prescription prescription)
        {
            _context.Prescriptions.Update(prescription);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Prescription prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription != null)
            {
                prescription.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
