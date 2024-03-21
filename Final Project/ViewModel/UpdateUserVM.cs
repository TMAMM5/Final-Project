using Final_Project.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_Project.ViewModel
{
    public class UpdateUserVM
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
