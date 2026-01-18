namespace HospitalManagementSystem.ViewModels.Doctors
{
    public class DetailDoctorVM
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string? Position { get; set; }
        public string? ImagePath { get; set; }
        public string? Biography { get; set; }
        public string DepartmentName { get; set; } 
        public int DepartmentId { get; set; }
    }
}
