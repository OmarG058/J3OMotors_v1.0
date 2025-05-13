using System.ComponentModel.DataAnnotations;

namespace J3OMotors_v1._0.Models
{
    public class LoginViewModel
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
