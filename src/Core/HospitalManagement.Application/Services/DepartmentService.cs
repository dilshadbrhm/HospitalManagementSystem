using HospitalManagement.Application.Dtos.Department;
using HospitalManagement.Application.Dtos.Doctor;
using HospitalManagement.Application.Interfaces;
using HospitalManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IDoctorRepository _doctorRepository;

        public DepartmentService(IDepartmentRepository departmentRepository, IDoctorRepository doctorRepository)
        {
            _departmentRepository = departmentRepository;
            _doctorRepository = doctorRepository;
        }

        public async Task<List<DepartmentDto>> GetAllAsync()
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

        public async Task<DepartmentDetailsDto> GetByIdAsync(int id)
        {
            Department department = await _departmentRepository.GetByIdAsync(id);

            if (department == null)
            {
                return null;
            }

            DepartmentDetailsDto dto = new DepartmentDetailsDto
            {
               Id = department.Id,
               Name = department.Name,
               ShortDescription = department.Description,
               Doctors = new List<DoctorItemDto>()
            };
           

            IEnumerable<Doctor> doctors = await _doctorRepository.GetAllAsync();

            foreach (Doctor doctor in doctors)
            {
                if (doctor.DepartmentId == id)
                {
                    DoctorItemDto doctorDto = new DoctorItemDto
                    {
                       Id = doctor.Id,
                       FirstName = doctor.FirstName,
                       LastName = doctor.LastName,
                       FullName = doctor.FirstName + " " + doctor.LastName,
                       Specialization = doctor.Specialization,
                       ProfilePicture = doctor.ProfilePicture,
                       Bio = doctor.Bio,
                       DepartmentName = department.Name
                    };
                 

                    dto.Doctors.Add(doctorDto);
                }
            }

            return dto;
        }
    }
}
