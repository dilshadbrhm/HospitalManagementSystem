using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Dtos.Prescription
{
    public class CreatePrescriptionItemDto
    {
        [Required(ErrorMessage = "Medicine name is required")]
        public string MedicineName { get; set; }

        [Required(ErrorMessage = "Dosage is required")]
        public string Dosage { get; set; }

        [Required(ErrorMessage = "Frequency is required")]
        public string Frequency { get; set; }

        public int Duration { get; set; } = 7;

        public string Instructions { get; set; }
    }
}
