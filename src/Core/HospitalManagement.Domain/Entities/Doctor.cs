using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Domain
{
    public class Doctor: BaseEntity
    {
        public string UserId { get; set; }  
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Specialization { get; set; }
        public string LicenseNumber { get; set; }
        public int DepartmentId { get; set; }
        public decimal ConsultationFee { get; set; }
        public string Bio { get; set; }
        public string ProfilePicture { get; set; }
        public Department Department { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<TimeSlot> TimeSlots { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; }
    }
}
