using HospitalManagement.Domain;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;
using HospitalManagement.Infrastructure.Persistence;
using HospitalManagementSystem.ViewModels;
using HospitalManagementSystem.ViewModels.Appointment;
using HospitalManagementSystem.ViewModels.Doctors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    public class DoctorController : Controller
    {
        private readonly AppDbContext _context;

        public DoctorController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id < 1)
            {
                return BadRequest();
            }

            Doctor? doctor = await _context.Doctors
                .Include(d => d.Department)
                .Include(d => d.TimeSlots.Where(ts => ts.IsAvailable))
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }
    }
}

