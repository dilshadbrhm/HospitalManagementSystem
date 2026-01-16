using HospitalManagement.Domain;

namespace HospitalManagementSystem.ViewModels
{
    public class HomeVM
    {
        public List<Department> Departments { get; set; }
        public List<Doctor> FeaturedDoctors { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
        public int TotalDepartments { get; set; }
    }
}
