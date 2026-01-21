using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Dtos.Appointment
{
    public class AppointmentItemDto
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public string Status { get; set; }
        public string Symptoms { get; set; }
    }
}
