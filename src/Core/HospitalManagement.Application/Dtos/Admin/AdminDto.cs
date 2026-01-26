using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Dtos.Admin
{
    public class AdminDto
    {
        public int DoctorCount { get; set; }
        public int PatientCount { get; set; }
        public int DepartmentCount { get; set; }
        public int AppointmentCount { get; set; }
        public int PendingAppointmentCount { get; set; }
        public int TodayAppointmentCount { get; set; }
    }
}
