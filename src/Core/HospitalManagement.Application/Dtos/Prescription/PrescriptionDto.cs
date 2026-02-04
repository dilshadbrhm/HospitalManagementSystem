using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Dtos.Prescription
{
    public class PrescriptionDto
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSpecialization { get; set; }
        public DateTime PrescriptionDate { get; set; }
        public string Diagnosis { get; set; }
        public string Notes { get; set; }
        public DateTime? ValidUntil { get; set; }
        public List<PrescriptionItemDto> Items { get; set; }
    }
}
