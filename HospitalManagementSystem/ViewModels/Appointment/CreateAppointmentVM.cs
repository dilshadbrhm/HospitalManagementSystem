using HospitalManagement.Domain;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.ViewModels.Appointment
{
    public class CreateAppointmentVM
    {
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Select date")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Select an hour")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Record the symptoms")]
        [MaxLength(500)]
        public string Symptoms { get; set; }

        public string Notes { get; set; }

        
        public Doctor Doctor { get; set; }
        public List<TimeSpan> AvailableTimeSlots { get; set; }
    }
}
