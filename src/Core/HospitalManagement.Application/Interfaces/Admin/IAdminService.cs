using HospitalManagement.Application.Dtos.Admin;
using HospitalManagement.Application.Dtos.Appointment;
using HospitalManagement.Application.Dtos.Department;
using HospitalManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Interfaces.Admin
{
    public interface IAdminService
    {
        Task<AdminDto> GetAdminHomeAsync();

        Task<List<AdminDoctorListDto>> GetAllDoctorsAsync();
        Task<AdminDoctorEditDto> GetDoctorByIdAsync(int id);
        Task<bool> CreateDoctorAsync(AdminDoctorCreateDto dto);
        Task<bool> UpdateDoctorAsync(AdminDoctorEditDto dto);
        Task<bool> DeleteDoctorAsync(int id);
        Task<List<Patient>> GetAllPatientsAsync();
        Task<Patient> GetPatientByIdAsync(int id);
        Task<bool> DeletePatientAsync(int id);
        Task<List<DepartmentDto>> GetAllDepartmentsAsync();
        Task<Department> GetDepartmentByIdAsync(int id);
        Task<bool> CreateDepartmentAsync(Department department);
        Task<bool> UpdateDepartmentAsync(Department department);
        Task<bool> DeleteDepartmentAsync(int id);
        Task<List<AppointmentItemDto>> GetAllAppointmentsAsync();
        Task<bool> UpdateAppointmentStatusAsync(int id, string status);
        Task<bool> DeleteAppointmentAsync(int id);
    }
}
