using HospitalManagement.Application.Dtos.Timeslot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Dtos.Appointment
{
    public class AppointmentResultDto
    {
        public bool Success { get; set; }
        public int? AppointmentId { get; set; }
        public string Message { get; set; }
        public List<AlternativeSlotDto> AlternativeSlots { get; set; }
    }
}
