namespace HospitalManagementSystem.ViewModels.Appointment
{
    public class DetailsAppointmentVM
    {
        public int Id { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSpecialization { get; set; }
        public string PatientName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; }
        public string Symptoms { get; set; }
        public string Notes { get; set; }
        public decimal Fee { get; set; }
        public bool IsPaid { get; set; }
        public string CancellationReason { get; set; }
    }
}
