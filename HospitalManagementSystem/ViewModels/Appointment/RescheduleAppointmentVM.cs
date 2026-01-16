using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.ViewModels.Appointment
{
    public class RescheduleAppointmentVM
    {
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Select a new date")]
        [DataType(DataType.Date)]
        public DateTime NewDate { get; set; }

        [Required(ErrorMessage = "Select a new hour")]
        public TimeSpan NewStartTime { get; set; }

        public List<TimeSpan> AvailableTimeSlots { get; set; }
    }
}
