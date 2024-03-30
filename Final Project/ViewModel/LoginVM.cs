using System.ComponentModel.DataAnnotations;

namespace Final_Project.ViewModel
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Please Enter your Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please Enter your password")]
        public string Password { get; set; }
        public bool IsPresistent { get; set; }

    }
}
