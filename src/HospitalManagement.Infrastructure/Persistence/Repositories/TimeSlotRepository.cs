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
    public class TimeSlotRepository : ITimeSlotRepository
    {
        private readonly AppDbContext _context;

        public TimeSlotRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TimeSlot>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.TimeSlots
                .Where(t => t.DoctorId == doctorId)
                .OrderBy(t => t.DayOfWeek)
                .ThenBy(t => t.StartTime)
                .ToListAsync();
        }

        public async Task<TimeSlot?> GetByIdAsync(int id)
        {
            return await _context.TimeSlots.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddAsync(TimeSlot timeSlot)
        {
            await _context.TimeSlots.AddAsync(timeSlot);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TimeSlot timeSlot)
        {
            _context.TimeSlots.Update(timeSlot);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            TimeSlot? timeSlot = await _context.TimeSlots.FindAsync(id);
            if (timeSlot != null)
            {
                _context.TimeSlots.Remove(timeSlot);
                await _context.SaveChangesAsync();
            }
        }
    }
}
