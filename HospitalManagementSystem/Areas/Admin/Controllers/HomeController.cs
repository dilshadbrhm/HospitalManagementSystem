using HospitalManagement.Application.Dtos.Admin;
using HospitalManagement.Application.Dtos.Appointment;
using HospitalManagement.Application.Dtos.Department;
using HospitalManagement.Application.Interfaces;
using HospitalManagement.Application.Interfaces.Admin;
using HospitalManagement.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IDepartmentRepository _departmentRepository;

        public HomeController(IAdminService adminService, IDepartmentRepository departmentRepository)
        {
            _adminService = adminService;
            _departmentRepository = departmentRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            AdminDto data = await _adminService.GetAdminHomeAsync();
            return View("~/Areas/Admin/Views/Home/Index.cshtml", data);
        }

        [HttpGet]
        public async Task<IActionResult> Doctors()
        {
            List<AdminDoctorListDto> doctors = await _adminService.GetAllDoctorsAsync();
            return View(doctors);
        }

        [HttpGet]
        public async Task<IActionResult> CreateDoctor()
        {
            IEnumerable<Department> departments = await _departmentRepository.GetAllAsync();
            ViewBag.Departments = departments;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDoctor(AdminDoctorCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await _departmentRepository.GetAllAsync();
                return View(dto);
            }

            bool result = await _adminService.CreateDoctorAsync(dto);

            if (result)
            {
                TempData["Success"] = "Doctor created successfully";
                return RedirectToAction("Doctors");
            }

            TempData["Error"] = "Failed to create doctor";
            ViewBag.Departments = await _departmentRepository.GetAllAsync();
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> EditDoctor(int id)
        {
            AdminDoctorEditDto doctor = await _adminService.GetDoctorByIdAsync(id);

            if (doctor == null)
            {
                return NotFound();
            }

            ViewBag.Departments = await _departmentRepository.GetAllAsync();
            return View(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> EditDoctor(AdminDoctorEditDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await _departmentRepository.GetAllAsync();
                return View(dto);
            }

            bool result = await _adminService.UpdateDoctorAsync(dto);

            if (result)
            {
                TempData["Success"] = "Doctor updated successfully";
                return RedirectToAction("Doctors");
            }

            TempData["Error"] = "Failed to update doctor";
            ViewBag.Departments = await _departmentRepository.GetAllAsync();
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            bool result = await _adminService.DeleteDoctorAsync(id);

            if (result)
            {
                TempData["Success"] = "Doctor deleted successfully";
            }
            else
            {
                TempData["Error"] = "Failed to delete doctor";
            }

            return RedirectToAction("Doctors");
        }

        [HttpGet]
        public async Task<IActionResult> Patients()
        {
            List<Patient> patients = await _adminService.GetAllPatientsAsync();
            return View(patients);
        }

        [HttpGet]
        public async Task<IActionResult> PatientDetails(int id)
        {
            Patient patient = await _adminService.GetPatientByIdAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePatient(int id)
        {
            bool result = await _adminService.DeletePatientAsync(id);

            if (result)
            {
                TempData["Success"] = "Patient deleted successfully";
            }
            else
            {
                TempData["Error"] = "Failed to delete patient";
            }

            return RedirectToAction("Patients");
        }

        [HttpGet]
        public async Task<IActionResult> Departments()
        {
            List<DepartmentDto> departments = await _adminService.GetAllDepartmentsAsync();
            return View(departments);
        }

        [HttpGet]
        public IActionResult CreateDepartment()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment(Department department)
        {
            if (!ModelState.IsValid)
            {
                return View(department);
            }

            bool result = await _adminService.CreateDepartmentAsync(department);

            if (result)
            {
                TempData["Success"] = "Department created successfully";
                return RedirectToAction("Departments");
            }

            TempData["Error"] = "Failed to create department";
            return View(department);
        }

        [HttpGet]
        public async Task<IActionResult> EditDepartment(int id)
        {
            Department department = await _adminService.GetDepartmentByIdAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [HttpPost]
        public async Task<IActionResult> EditDepartment(Department department)
        {
            if (!ModelState.IsValid)
            {
                return View(department);
            }

            bool result = await _adminService.UpdateDepartmentAsync(department);

            if (result)
            {
                TempData["Success"] = "Department updated successfully";
                return RedirectToAction("Departments");
            }

            TempData["Error"] = "Failed to update department";
            return View(department);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            bool result = await _adminService.DeleteDepartmentAsync(id);

            if (result)
            {
                TempData["Success"] = "Department deleted successfully";
            }
            else
            {
                TempData["Error"] = "Failed to delete department";
            }

            return RedirectToAction("Departments");
        }

        [HttpGet]
        public async Task<IActionResult> Appointments()
        {
            List<AppointmentItemDto> appointments = await _adminService.GetAllAppointmentsAsync();
            return View(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, string status)
        {
            bool result = await _adminService.UpdateAppointmentStatusAsync(id, status);

            if (result)
            {
                TempData["Success"] = "Status updated successfully";
            }
            else
            {
                TempData["Error"] = "Failed to update status";
            }

            return RedirectToAction("Appointments");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            bool result = await _adminService.DeleteAppointmentAsync(id);

            if (result)
            {
                TempData["Success"] = "Appointment deleted successfully";
            }
            else
            {
                TempData["Error"] = "Failed to delete appointment";
            }

            return RedirectToAction("Appointments");
        }
    }
}
