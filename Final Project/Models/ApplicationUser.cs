#nullable enable 
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_Project.Models
{
    public class ApplicationUser : IdentityUser
    {
        //[DataType(DataType.EmailAddress)]

        //public string Email { get; set; }
        //[DataType(DataType.Password)]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8}$",ErrorMessage = "Password Must Contain At Least one lowercase letter , one uppercase letter and one uppercase letter </br>" +
        //    "With Minimum Length 8 Character")]
        //public string Password { get; set; }

        [MaxLength(30)]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
        public string Name { get; set; }

        //[RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Please Enter Valid Phone Number")]
        //public string Phone { get; set; }
        public string Address { get; set; }

        public DateTime creationDate { get; set; } = DateTime.Now;
        [Required]
        public bool IsDeleted { get; set; } = false;

        [ForeignKey("Branch")]
        [Display(Name = "Branch")]
        [Required(ErrorMessage = "Please select branch")]
        public int BranchId { get; set; }
        public virtual Branch? Branch { get; set; }

    }
}
