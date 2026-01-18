using HospitalManagementSystem.ViewModels.Doctors;

namespace HospitalManagementSystem.ViewModels.Department
{
    public class DetailDepartmentVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<DoctorListVM> Doctors { get; set; }
    }

}