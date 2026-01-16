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

        public async Task<IActionResult> Index(string? search, int? departmentId, int page = 1)
        {
            IQueryable<Doctor> query = _context.Doctors.Where(d => !d.IsDeleted);

            if (search != null)
            {
                query = query.Where(d => d.FirstName.Contains(search) ||
                                        d.LastName.Contains(search) ||
                                        d.Specialization.Contains(search));
            }

            if (departmentId != null)
            {
                query = query.Where(d => d.DepartmentId == departmentId);
            }

            query = query.OrderBy(d => d.LastName);

            int totalCount = await query.CountAsync();
            int totalPage = (int)Math.Ceiling((double)totalCount / 9);

            query = query.Skip((page - 1) * 9).Take(9);

            DoctorListVM doctorListVM = new()
            {
                Doctors = await query.Select(d => new GetDoctorVM
                {
                    Id = d.Id,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Specialization = d.Specialization,
                    DepartmentName = d.Department.Name,
                    ConsultationFee = d.ConsultationFee,
                    ProfilePicture = d.ProfilePicture,
                    Bio = d.Bio
                }).ToListAsync(),

                Departments = await _context.Departments
                    .Where(d => !d.IsDeleted)
                    .OrderBy(d => d.Name)
                    .ToListAsync(),

                Search = search,
                DepartmentId = departmentId,
                CurrentPage = page,
                TotalPages = totalPage
            };

            return View(doctorListVM);
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

