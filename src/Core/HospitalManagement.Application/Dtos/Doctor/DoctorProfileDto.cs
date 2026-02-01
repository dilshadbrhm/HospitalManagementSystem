using HospitalManagement.Application.Dtos.Timeslot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Dtos.Doctor
{
    public class DoctorProfileDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Specialization { get; set; }
        public string DepartmentName { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
        public decimal ConsultationFee { get; set; }
        public List<TimeSlotDto> TimeSlots { get; set; }

    }
}
