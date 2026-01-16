using HospitalManagement.Domain;
using HospitalManagement.Infrastructure.Persistence;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HospitalManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Department> departments = await _context.Departments
                .Where(d => !d.IsDeleted)
                .OrderBy(d => d.Name)
                .Take(6)
                .ToListAsync();

            List<Doctor> doctors = await _context.Doctors
                .Where(d => !d.IsDeleted)
                .OrderByDescending(d => d.CreatedAt)
                .Take(8)
                .Include(d => d.Department)
                .ToListAsync();

            HomeVM homeVM = new HomeVM
            {
                Departments = departments,
                FeaturedDoctors = doctors,
                TotalDoctors = await _context.Doctors.CountAsync(d => !d.IsDeleted),
                TotalPatients = await _context.Patients.CountAsync(p => !p.IsDeleted),
                TotalDepartments = await _context.Departments.CountAsync(d => !d.IsDeleted)
            };

            return View(homeVM);
        }
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }


    }
}
