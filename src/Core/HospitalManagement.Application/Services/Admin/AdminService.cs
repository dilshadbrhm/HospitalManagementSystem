using HospitalManagement.Application.Dtos.Admin;
using HospitalManagement.Application.Dtos.Appointment;
using HospitalManagement.Application.Dtos.Department;
using HospitalManagement.Application.Interfaces;
using HospitalManagement.Application.Interfaces.Admin;
using HospitalManagement.Domain;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminService(
            IDoctorRepository doctorRepository,
            IPatientRepository patientRepository,
            IDepartmentRepository departmentRepository,
            IAppointmentRepository appointmentRepository,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _departmentRepository = departmentRepository;
            _appointmentRepository = appointmentRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<AdminDto> GetAdminHomeAsync()
        {
            IEnumerable<Doctor> doctors = await _doctorRepository.GetAllAsync();
            IEnumerable<Patient> patients = await _patientRepository.GetAllAsync();
            IEnumerable<Department> departments = await _departmentRepository.GetAllAsync();
            IEnumerable<Appointment> appointments = await _appointmentRepository.GetAllAsync();

            AdminDto dto = new AdminDto();
            dto.DoctorCount = doctors.Count();
            dto.PatientCount = patients.Count();
            dto.DepartmentCount = departments.Count();
            dto.AppointmentCount = appointments.Count();
            dto.PendingAppointmentCount = appointments.Count(a => a.Status == AppointmentStatus.Pending);
            dto.TodayAppointmentCount = appointments.Count(a => a.AppointmentDate.Date == DateTime.Today);

            return dto;
        }

        public async Task<List<AdminDoctorListDto>> GetAllDoctorsAsync()
        {
            IEnumerable<Doctor> doctors = await _doctorRepository.GetAllWithDepartmentAsync();

            List<AdminDoctorListDto> result = new List<AdminDoctorListDto>();

            foreach (Doctor doctor in doctors)
            {
                AdminDoctorListDto dto = new AdminDoctorListDto
                {
                   Id = doctor.Id,
                   FirstName = doctor.FirstName,
                   LastName = doctor.LastName,
                   Email = doctor.Email,
                   Phone = doctor.Phone,
                   Specialization = doctor.Specialization,
                   ConsultationFee = doctor.ConsultationFee
                };
               

                if (doctor.Department != null)
                {
                    dto.DepartmentName = doctor.Department.Name;
                }

                result.Add(dto);
            }

            return result;
        }

        public async Task<AdminDoctorEditDto> GetDoctorByIdAsync(int id)
        {
            Doctor doctor = await _doctorRepository.GetByIdAsync(id);

            if (doctor == null)
            {
                return null;
            }

            AdminDoctorEditDto dto = new AdminDoctorEditDto
            {
              Id = doctor.Id,
              FirstName = doctor.FirstName,
              LastName = doctor.LastName,
              Email = doctor.Email,
              Phone = doctor.Phone,
              Specialization = doctor.Specialization,
              LicenseNumber = doctor.LicenseNumber,
              DepartmentId = doctor.DepartmentId,
              ConsultationFee = doctor.ConsultationFee,
              Bio = doctor.Bio,
              ProfilePicture = doctor.ProfilePicture
            };
            

            return dto;
        }

        public async Task<bool> CreateDoctorAsync(AdminDoctorCreateDto dto)
        {
            AppUser user = new AppUser
            {
               UserName = dto.Email,
               Email = dto.Email,
               FirstName = dto.FirstName,
               LastName = dto.LastName,
               PhoneNumber = dto.Phone,
               EmailConfirmed = true,
               CreatedAt = DateTime.Now
            };
            

            IdentityResult result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return false;
            }

            if (!await _roleManager.RoleExistsAsync("Doctor"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Doctor"));
            }

            await _userManager.AddToRoleAsync(user, "Doctor");

            Doctor doctor = new Doctor
            {
              UserId = user.Id,
              FirstName = dto.FirstName,
              LastName = dto.LastName,
              Email = dto.Email,
              Phone = dto.Phone,
              Specialization = dto.Specialization,
              LicenseNumber = dto.LicenseNumber,
              DepartmentId = dto.DepartmentId,
              ConsultationFee = dto.ConsultationFee,
              Bio = dto.Bio
            };
           

            await _doctorRepository.AddAsync(doctor);

            return true;
        }

        public async Task<bool> UpdateDoctorAsync(AdminDoctorEditDto dto)
        {
            Doctor doctor = await _doctorRepository.GetByIdAsync(dto.Id);

            if (doctor == null)
            {
                return false;
            }

            doctor.FirstName = dto.FirstName;
            doctor.LastName = dto.LastName;
            doctor.Email = dto.Email;
            doctor.Phone = dto.Phone;
            doctor.Specialization = dto.Specialization;
            doctor.LicenseNumber = dto.LicenseNumber;
            doctor.DepartmentId = dto.DepartmentId;
            doctor.ConsultationFee = dto.ConsultationFee;
            doctor.Bio = dto.Bio;

            if (dto.ProfilePicture != null)
            {
                doctor.ProfilePicture = dto.ProfilePicture;
            }

            await _doctorRepository.UpdateAsync(doctor);

            return true;
        }

        public async Task<bool> DeleteDoctorAsync(int id)
        {
            Doctor doctor = await _doctorRepository.GetByIdAsync(id);

            if (doctor == null)
            {
                return false;
            }

            await _doctorRepository.DeleteAsync(id);

            return true;
        }

        public async Task<List<Patient>> GetAllPatientsAsync()
        {
            IEnumerable<Patient> patients = await _patientRepository.GetAllAsync();
            return patients.ToList();
        }

        public async Task<Patient> GetPatientByIdAsync(int id)
        {
            Patient patient = await _patientRepository.GetByIdAsync(id);
            return patient;
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            Patient patient = await _patientRepository.GetByIdAsync(id);

            if (patient == null)
            {
                return false;
            }

            IEnumerable<Appointment> appointments = await _appointmentRepository.GetAllAsync();
            bool hasAppointments = appointments.Any(a => a.PatientId == id);

            if (hasAppointments)
            {
                return false; 
            }

            await _patientRepository.DeleteAsync(id);

            return true;
        }

        public async Task<List<DepartmentDto>> GetAllDepartmentsAsync()
        {
            IEnumerable<Department> departments = await _departmentRepository.GetAllAsync();

            List<DepartmentDto> result = new List<DepartmentDto>();

            foreach (Department department in departments)
            {
                DepartmentDto dto = new DepartmentDto
                {
                  Id = department.Id,
                  Name = department.Name,
                  ShortDescription = department.Description
                };
               

                result.Add(dto);
            }

            return result;
        }

        public async Task<Department> GetDepartmentByIdAsync(int id)
        {
            Department department = await _departmentRepository.GetByIdAsync(id);
            return department;
        }

        public async Task<bool> CreateDepartmentAsync(Department department)
        {
            await _departmentRepository.AddAsync(department);
            return true;
        }

        public async Task<bool> UpdateDepartmentAsync(Department department)
        {
            Department existingDepartment = await _departmentRepository.GetByIdAsync(department.Id);

            if (existingDepartment == null)
            {
                return false;
            }

            existingDepartment.Name = department.Name;
            existingDepartment.Description = department.Description;

            await _departmentRepository.UpdateAsync(existingDepartment);
            return true;
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            Department department = await _departmentRepository.GetByIdAsync(id);

            if (department == null)
            {
                return false;
            }

            await _departmentRepository.DeleteAsync(id);

            return true;
        }

        public async Task<List<AppointmentItemDto>> GetAllAppointmentsAsync()
        {
            IEnumerable<Appointment> appointments = await _appointmentRepository.GetAllAsync();

            List<AppointmentItemDto> result = new List<AppointmentItemDto>();

            foreach (Appointment appointment in appointments)
            {
                AppointmentItemDto dto = new AppointmentItemDto
                {
                  Id = appointment.Id,
                  AppointmentDate = appointment.AppointmentDate,
                  StartTime = appointment.StartTime,
                  EndTime = appointment.EndTime,
                  Status = appointment.Status.ToString(),
                  Fee = appointment.Fee,
                  IsPaid = appointment.IsPaid,
                  Symptoms = appointment.Symptoms
                };
             

                if (appointment.Doctor != null)
                {
                    dto.DoctorName = appointment.Doctor.FirstName + " " + appointment.Doctor.LastName;

                    if (appointment.Doctor.Department != null)
                    {
                        dto.DepartmentName = appointment.Doctor.Department.Name;
                    }
                }

                if (appointment.Patient != null)
                {
                    dto.PatientName = appointment.Patient.FirstName + " " + appointment.Patient.LastName;
                }

                result.Add(dto);
            }

            return result;
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int id, string status)
        {
            Appointment appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment == null)
            {
                return false;
            }

            appointment.Status = (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), status);
            await _appointmentRepository.UpdateAsync(appointment);

            return true;
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            Appointment appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment == null)
            {
                return false;
            }

            await _appointmentRepository.DeleteAsync(id);

            return true;
        }
    }
}
