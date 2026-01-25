using HospitalManagement.Application.Dtos.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Dtos.Department
{
    public class DepartmentDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public List<DoctorItemDto> Doctors { get; set; }
    }
}
