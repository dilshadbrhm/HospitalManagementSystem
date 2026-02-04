using HospitalManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Domain
{
    public class Prescription : BaseEntity
    {
        public int AppointmentId { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public DateTime PrescriptionDate { get; set; }
        public string Diagnosis { get; set; }
        public string? Notes { get; set; }
        public DateTime? ValidUntil { get; set; }

        public Appointment Appointment { get; set; }
        public Doctor Doctor { get; set; }
        public Patient Patient { get; set; }
        public ICollection<PrescriptionItem> Items { get; set; }
    }
}
