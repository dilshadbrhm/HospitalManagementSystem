using HospitalManagement.Application.Dtos.Department;
using HospitalManagement.Application.Dtos.Doctor;
using HospitalManagement.Application.Dtos.Timeslot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Dtos.Appointment
{
    public class CreateAppointmentDto
    {
        [Required(ErrorMessage = "Please select a department")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Please select a doctor")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Please select a date")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Please select a time")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Please describe your symptoms")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Symptoms must be between 10 500 characters")]
        public string Symptoms { get; set; }

        [StringLength(300)]
        public string Notes { get; set; }
        public List<DepartmentSelectDto> Departments { get; set; }
        public List<DoctorSelectDto> Doctors { get; set; }
        public List<TimeSlotSelectDto> AvailableSlots { get; set; }
    }

}