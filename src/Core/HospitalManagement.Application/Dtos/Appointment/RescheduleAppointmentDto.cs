using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Dtos.Appointment
{
    public class RescheduleAppointmentDto
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Please select a new date")]
        [DataType(DataType.Date)]
        public DateTime NewDate { get; set; }

        [Required(ErrorMessage = "Please select a new time")]
        public TimeSpan NewStartTime { get; set; }

        public string Reason { get; set; }
    }
}
