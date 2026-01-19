using HospitalManagement.Application.Dtos.Department;
using HospitalManagement.Application.Dtos.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Dtos
{
    public class HomeDto
    {
        public IEnumerable<DepartmentDto> Departments { get; set; }
        public IEnumerable<DoctorDto> Doctors { get; set; }
    }
}
