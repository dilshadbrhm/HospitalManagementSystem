using HospitalManagement.Domain;

namespace HospitalManagementSystem.ViewModels.Doctors
{
    public class DoctorListVM
    {
        public List<GetDoctorVM> Doctors { get; set; }
        public List<Department> Departments { get; set; }
        public string Search { get; set; }
        public int? DepartmentId { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
