using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Dtos.Appointment
{
    public class CancelAppointmentDto
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Please provide a cancellation reason")]
        [StringLength(300, MinimumLength = 5, ErrorMessage = "Reason must be between 5 300 characters")]
        public string CancellationReason { get; set; }
    }
}
