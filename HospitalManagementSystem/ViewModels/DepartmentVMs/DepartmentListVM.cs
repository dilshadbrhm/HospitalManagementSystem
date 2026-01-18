namespace HospitalManagementSystem.ViewModels.Department
{
    public class DepartmentListVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string? ImagePath { get; set; } 
        public int DoctorCount { get; set; }
    }
}
