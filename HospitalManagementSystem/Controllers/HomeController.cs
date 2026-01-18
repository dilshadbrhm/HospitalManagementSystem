using HospitalManagement.Domain;
using HospitalManagement.Infrastructure.Persistence;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.ViewModels;
using HospitalManagementSystem.ViewModels.Department;
using HospitalManagementSystem.ViewModels.Doctors;
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
            var model = new HomeVM
            {
                Departments = _context.Departments
              .Select(d => new DepartmentListVM
              {
                  Id = d.Id,
                  Name = d.Name,
                    ShortDescription = d.ShortDescription

              })
              .ToList(),

                Doctors = _context.Doctors
              .Select(d => new DoctorListVM
              {
                  Id = d.Id,
                  FullName = d.FullName,
                  ImagePath = d.ImagePath,
                  DepartmentName = d.Department.Name,
                  InternshipDetails = d.InternshipDetails
              })
              .ToList()
            };

            return View(model);
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
