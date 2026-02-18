using HospitalManagement.Application.Dtos.Admin;
using HospitalManagement.Application.Dtos.Appointment;
using HospitalManagement.Application.Dtos.Department;
using HospitalManagement.Application.Interfaces;
using HospitalManagement.Application.Interfaces.Admin;
using HospitalManagement.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using System.IO;

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
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Doctors()
        {
            List<AdminDoctorListDto> doctors = await _adminService.GetAllDoctorsAsync();
            return View("~/Areas/Admin/Views/Doctor/Doctors.cshtml", doctors);
        }

        [HttpGet]
        public async Task<IActionResult> CreateDoctor()
        {
            IEnumerable<Department> departments = await _departmentRepository.GetAllAsync();
            ViewBag.Departments = departments;
            return View("~/Areas/Admin/Views/Doctor/CreateDoctor.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateDoctor(AdminDoctorCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await _departmentRepository.GetAllAsync();
                return View("~/Areas/Admin/Views/Doctor/CreateDoctor.cshtml", dto);
            }

            bool result = await _adminService.CreateDoctorAsync(dto);

            if (result)
            {
                TempData["Success"] = "Doctor created successfully";
                return RedirectToAction("Doctors");
            }

            TempData["Error"] = "Failed to create doctor";
            ViewBag.Departments = await _departmentRepository.GetAllAsync();
            return View("~/Areas/Admin/Views/Doctor/CreateDoctor.cshtml", dto);
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
            return View("~/Areas/Admin/Views/Doctor/EditDoctor.cshtml", doctor);
        }

        [HttpPost]
        public async Task<IActionResult> EditDoctor(AdminDoctorEditDto dto, IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await _departmentRepository.GetAllAsync();
                return View("~/Areas/Admin/Views/Doctor/EditDoctor.cshtml", dto);
            }

            if (ImageFile != null && ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "image");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                dto.ProfilePicture = "/assets/image/" + uniqueFileName;
            }

            bool result = await _adminService.UpdateDoctorAsync(dto);

            if (result)
            {
                TempData["Success"] = "Doctor updated successfully";
                return RedirectToAction("Doctors");
            }

            TempData["Error"] = "Failed to update doctor";
            ViewBag.Departments = await _departmentRepository.GetAllAsync();
            return View("~/Areas/Admin/Views/Doctor/EditDoctor.cshtml", dto);
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
            return View("~/Areas/Admin/Views/Patient/Patients.cshtml", patients);
        }

        [HttpGet]
        public async Task<IActionResult> PatientDetails(int id)
        {
            Patient patient = await _adminService.GetPatientByIdAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return View("~/Areas/Admin/Views/Patient/PatientDetails.cshtml", patient);
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
                TempData["Error"] = "Cannot delete patient. Patient has appointments or not found.";
            }

            return RedirectToAction("Patients");
        }

        [HttpGet]
        public async Task<IActionResult> Departments()
        {
            List<DepartmentDto> departments = await _adminService.GetAllDepartmentsAsync();
            return View("~/Areas/Admin/Views/Department/Departments.cshtml", departments);
        }

        [HttpGet]
        public IActionResult CreateDepartment()
        {
            return View("~/Areas/Admin/Views/Department/CreateDepartment.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment(Department department)
        {
            ModelState.Remove("CreatedAt");
            ModelState.Remove("UpdatedAt");
            ModelState.Remove("IsDeleted");
            ModelState.Remove("Doctors");

            if (!ModelState.IsValid)
            {
                return View("~/Areas/Admin/Views/Department/CreateDepartment.cshtml", department);
            }

            department.CreatedAt = DateTime.Now;

            bool result = await _adminService.CreateDepartmentAsync(department);

            if (result)
            {
                TempData["Success"] = "Department created successfully";
                return RedirectToAction("Departments");
            }

            TempData["Error"] = "Failed to create department";
            return View("~/Areas/Admin/Views/Department/CreateDepartment.cshtml", department);
        }

        [HttpGet]
        public async Task<IActionResult> EditDepartment(int id)
        {
            Department department = await _adminService.GetDepartmentByIdAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return View("~/Areas/Admin/Views/Department/EditDepartment.cshtml", department);
        }

        [HttpPost]
        public async Task<IActionResult> EditDepartment(Department department)
        {
            ModelState.Remove("CreatedAt");
            ModelState.Remove("UpdatedAt");
            ModelState.Remove("IsDeleted");
            ModelState.Remove("Doctors");

            if (!ModelState.IsValid)
            {
                return View("~/Areas/Admin/Views/Department/EditDepartment.cshtml", department);
            }

            bool result = await _adminService.UpdateDepartmentAsync(department);

            if (result)
            {
                TempData["Success"] = "Department updated successfully";
                return RedirectToAction("Departments");
            }

            TempData["Error"] = "Failed to update department";
            return View("~/Areas/Admin/Views/Department/EditDepartment.cshtml", department);
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
            return View("~/Areas/Admin/Views/Appointment/Appointments.cshtml", appointments);
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

        [HttpPost]
        public async Task<IActionResult> ApproveAppointment(int id)
        {
            bool result = await _adminService.UpdateAppointmentStatusAsync(id, "Confirmed");

            if (result)
            {
                TempData["Success"] = "Appointment approved successfully";
            }
            else
            {
                TempData["Error"] = "Failed to approve appointment";
            }

            return RedirectToAction("Appointments");
        }

        [HttpPost]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            bool result = await _adminService.UpdateAppointmentStatusAsync(id, "Cancelled");

            if (result)
            {
                TempData["Success"] = "Appointment cancelled successfully";
            }
            else
            {
                TempData["Error"] = "Failed to cancel appointment";
            }

            return RedirectToAction("Appointments");
        }
    }
}
