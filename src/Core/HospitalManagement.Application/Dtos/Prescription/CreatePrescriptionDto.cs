using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Dtos.Prescription
{
    public class CreatePrescriptionDto
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Diagnosis is required")]
        public string Diagnosis { get; set; }

        public string Notes { get; set; }

        public int ValidDays { get; set; } = 30;

        public List<CreatePrescriptionItemDto> Items { get; set; } = new List<CreatePrescriptionItemDto>();
    }
}
