using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Dtos.Admin
{
    public class AdminDoctorEditDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Specialization { get; set; }
        public string LicenseNumber { get; set; }
        public int DepartmentId { get; set; }
        public decimal ConsultationFee { get; set; }
        public string Bio { get; set; }
    }
}
