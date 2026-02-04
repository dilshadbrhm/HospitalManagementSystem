using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Dtos.Prescription
{
    public class PrescriptionItemDto
    {
        public int Id { get; set; }
        public string MedicineName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public int Duration { get; set; }
        public string Instructions { get; set; }
    }
}
