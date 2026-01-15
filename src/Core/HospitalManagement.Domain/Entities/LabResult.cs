using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Domain
{
    public class LabResult: BaseEntity
    {
        public int PatientId { get; set; }
        public int? DoctorId { get; set; }
        public string TestName { get; set; }
        public string TestType { get; set; }
        public string Description { get; set; }
        public string PdfFilePath { get; set; }
        public DateTime TestDate { get; set; }
        public DateTime UploadedAt { get; set; }
        public string UploadedBy { get; set; } 
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
    }
}
