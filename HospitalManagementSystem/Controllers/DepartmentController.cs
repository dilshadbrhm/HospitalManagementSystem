using HospitalManagement.Application.Dtos.Department;
using HospitalManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<DepartmentDto> departments = await _departmentService.GetAllAsync();
            return View(departments);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToAction("Index");
            }

            DepartmentDetailsDto department = await _departmentService.GetByIdAsync(id.Value);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }
    }
}
