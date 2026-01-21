using HospitalManagement.Application.Dtos.Appointment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Dtos.Doctor
{
    public class DoctorCabinetDto
    {
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public List<AppointmentItemDto> TodayAppointments { get; set; }
        public List<AppointmentItemDto> UpcomingAppointments { get; set; }
    }
}
