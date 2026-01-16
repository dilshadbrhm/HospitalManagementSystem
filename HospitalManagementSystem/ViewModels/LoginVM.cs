using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Enter email or username")]
        public string EmailOrUsername { get; set; }

        [Required(ErrorMessage = "Enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
