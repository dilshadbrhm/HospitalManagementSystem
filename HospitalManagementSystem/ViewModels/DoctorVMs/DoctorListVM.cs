using HospitalManagement.Domain;
using HospitalManagementSystem.ViewModels.Department;

namespace HospitalManagementSystem.ViewModels.Doctors
{
    public class DoctorListVM
    {
        public int Id { get; set; }
        public string FullName { get; set; } 
        public string? Position { get; set; }
        public string? ImagePath { get; set; }
        public string DepartmentName { get; set; } 
        public string InternshipDetails { get; set; }
    }
}
