using HospitalManagement.Application.Dtos;
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
    public class HomeService:IHomeService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IDoctorRepository _doctorRepository;

        public HomeService(IDepartmentRepository departmentRepository, IDoctorRepository doctorRepository)
        {
            _departmentRepository = departmentRepository;
            _doctorRepository = doctorRepository;
        }

        public async Task<HomeDto> GetHomeDataAsync()
        {
            IEnumerable<Department> departments = await _departmentRepository.GetAllAsync();
            IEnumerable<Doctor> doctors = await _doctorRepository.GetAllWithDepartmentAsync();

            return new HomeDto
            {
                Departments = departments.Select(d => new DepartmentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    ShortDescription = d.Description
                }),
                Doctors = doctors.Select(d => new DoctorDto
                {
                    Id = d.Id,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    DepartmentName = d.Department?.Name,
                    ProfilePicture = d.ProfilePicture,
                    Specialization = d.Specialization,
                    Bio = d.Bio
                })
            };
        }
    }
}
