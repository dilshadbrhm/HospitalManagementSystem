namespace HospitalManagementSystem.ViewModels.Doctors
{
    public class GetDoctorVM
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Specialization { get; set; }
        public string DepartmentName { get; set; }
        public decimal ConsultationFee { get; set; }
        public string ProfilePicture { get; set; }
        public string Bio { get; set; }
    }
}
