using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Domain 
{ 
    public class Appointment: BaseEntity
    {
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public string Symptoms { get; set; }
        public string Notes { get; set; }
        public decimal Fee { get; set; }
        public bool IsPaid { get; set; }
        public string CancellationReason { get; set; }
        public DateTime? CancelledAt { get; set; }        
        public Doctor Doctor { get; set; }
        public Patient Patient { get; set; }
    }
}
