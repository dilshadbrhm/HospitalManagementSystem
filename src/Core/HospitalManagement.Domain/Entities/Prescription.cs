using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Domain
{
    public class Prescription: BaseEntity
    {
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public int? AppointmentId { get; set; }
        public string Medications { get; set; }  
        public string Diagnosis { get; set; }
        public string Instructions { get; set; }
        public string PdfFilePath { get; set; }
        public DateTime PrescriptionDate { get; set; }
        public Doctor Doctor { get; set; }
        public Patient Patient { get; set; }
        public Appointment Appointment { get; set; }
    }
}
