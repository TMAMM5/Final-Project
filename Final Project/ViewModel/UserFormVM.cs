using Final_Project.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Final_Project.ViewModel
{
    public class UserFormVM
    {
        public string? Id { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Name must be at least 3 characters long.", MinimumLength = 3)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Address must be at least 4 characters long.", MinimumLength = 4)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{5}$", ErrorMessage = "Password Must Contain At Least one lowercase letter , one uppercase letter and " +
         "With Minimum Length 5 Character")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and Confirm password doesn't match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        [ForeignKey("Branch")]
        [Display(Name = "Branch")]
        [Required(ErrorMessage = "Please select branch")]
        public int BranchId { get; set; }
        public virtual Branch? Branch { get; set; }
        public string RoleName { get; set; }

        public List<Branch>? Branches { get; set; }
        public List<RoleVM>? Roles { get; set; }
    }
}
