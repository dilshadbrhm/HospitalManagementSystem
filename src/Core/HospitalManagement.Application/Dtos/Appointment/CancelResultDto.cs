using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Dtos.Appointment
{
    public class CancelResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string PatientEmail { get; set; }
        public string DoctorEmail { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
    }
}
