using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Dtos.Doctor
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string DepartmentName { get; set; }
        public string ProfilePicture { get; set; }
        public string Specialization { get; set; }
        public string Bio { get; set; }
    }
}
