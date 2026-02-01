using HospitalManagement.Application.Dtos.Appointment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Dtos.Patient
{
    public class PatientDashboardDto
    {
        public string PatientName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int TotalAppointments { get; set; }
        public int UpcomingCount { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
        public List<AppointmentItemDto> UpcomingAppointments { get; set; }
        public List<AppointmentItemDto> PastAppointments { get; set; }
    }
}
