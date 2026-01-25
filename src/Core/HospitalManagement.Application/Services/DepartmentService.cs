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
                DepartmentDto dto = new DepartmentDto();
                dto.Id = department.Id;
                dto.Name = department.Name;
                dto.ShortDescription = department.Description;

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

            DepartmentDetailsDto dto = new DepartmentDetailsDto();
            dto.Id = department.Id;
            dto.Name = department.Name;
            dto.ShortDescription = department.Description;
            dto.Doctors = new List<DoctorItemDto>();

            IEnumerable<Doctor> doctors = await _doctorRepository.GetAllAsync();

            foreach (Doctor doctor in doctors)
            {
                if (doctor.DepartmentId == id)
                {
                    DoctorItemDto doctorDto = new DoctorItemDto();
                    doctorDto.Id = doctor.Id;
                    doctorDto.FirstName = doctor.FirstName;
                    doctorDto.LastName = doctor.LastName;
                    doctorDto.FullName = doctor.FirstName + " " + doctor.LastName;
                    doctorDto.Specialization = doctor.Specialization;
                    doctorDto.ProfilePicture = doctor.ProfilePicture;
                    doctorDto.Bio = doctor.Bio;
                    doctorDto.DepartmentName = department.Name;

                    dto.Doctors.Add(doctorDto);
                }
            }

            return dto;
        }
    }
}
