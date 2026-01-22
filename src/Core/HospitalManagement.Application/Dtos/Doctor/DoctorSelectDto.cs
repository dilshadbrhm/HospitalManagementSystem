using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Dtos.Doctor
{
    public class DoctorSelectDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Specialization { get; set; }
        public decimal ConsultationFee { get; set; }
        public int DepartmentId { get; set; }
    }
}
