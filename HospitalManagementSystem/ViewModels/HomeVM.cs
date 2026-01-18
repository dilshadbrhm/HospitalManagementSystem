using HospitalManagement.Domain;
using HospitalManagementSystem.ViewModels.Department;
using HospitalManagementSystem.ViewModels.Doctors;

namespace HospitalManagementSystem.ViewModels
{
    public class HomeVM
    {
        public List<DepartmentListVM>? Departments { get; set; }
        public List<DoctorListVM> Doctors { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
        public int TotalDepartments { get; set; }
    }
}
