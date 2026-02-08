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
    public class LabResultRepository : ILabResultRepository
    {
        private readonly AppDbContext _context;

        public LabResultRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LabResult>> GetAllAsync()
        {
            return await _context.LabResults
                .Include(l => l.Patient)
                .Include(l => l.Doctor)
                .Where(l => !l.IsDeleted)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<LabResult>> GetByPatientIdAsync(int patientId)
        {
            return await _context.LabResults
                .Include(l => l.Doctor)
                .Where(l => l.PatientId == patientId && !l.IsDeleted)
                .OrderByDescending(l => l.TestDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<LabResult>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.LabResults
                .Include(l => l.Patient)
                .Where(l => l.DoctorId == doctorId && !l.IsDeleted)
                .OrderByDescending(l => l.TestDate)
                .ToListAsync();
        }

        public async Task<LabResult> GetByIdAsync(int id)
        {
            return await _context.LabResults
                .Include(l => l.Patient)
                .Include(l => l.Doctor)
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);
        }

        public async Task AddAsync(LabResult labResult)
        {
            labResult.CreatedAt = DateTime.Now;
            await _context.LabResults.AddAsync(labResult);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LabResult labResult)
        {
            labResult.UpdatedAt = DateTime.Now;
            _context.LabResults.Update(labResult);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            LabResult labResult = await _context.LabResults.FindAsync(id);
            if (labResult != null)
            {
                labResult.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
