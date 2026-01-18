using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.ViewModels.Appointment
{
    public class CancelAppointmentVM
    {
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Please specify the reason for cancellation")]
        [MaxLength(500)]
        public string CancellationReason { get; set; }
    }
}
