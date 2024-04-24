using Final_Project.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Final_Project.ViewModel
{
    public class TraderVM
    {
        public string? AppUserId { get; set; }

        [MaxLength(50,
            ErrorMessage = "Name must be less than 50 characters")]

        [MinLength(5,
            ErrorMessage = "Minimum length is 5 character")]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$",
            ErrorMessage = "Email address is not valid")]
        [Required(ErrorMessage ="Email Is Required")]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{5,20}$", ErrorMessage = "Password Must Contain At Least one lowercase letter , one uppercase letter and one uppercase letter </br>" +
            "With Minimum Length 5 Character")]
        public string? Password { get; set; }

        public bool IsDeleted { get; set; } = false;

        [RegularExpression(@"^01[0125][0-9]{8}$",
            ErrorMessage = "The Phone number field is not valid")]
        public string Phone { get; set; }

        [MinLength(5,
            ErrorMessage = "Minimum length for Address is 5 Characters")]
        [MaxLength(50,
            ErrorMessage = "Address must be less than 50 characters")]
        public string Address { get; set; }

        [Display(Name = "Store Name")]
        public string StoreName { get; set; }

        [Display(Name = "Special Pickup Cost")]
        public int SpecialPickupCost { get; set; }

        [Display(Name = "Trader Tax For Rejected Orders")]
        public int TraderTaxForRejectedOrders { get; set; }

        [Display(Name = "Governorate")]
        [ForeignKey("Governorate")]
        [Required(ErrorMessage = "Please select a governorate")]
        public int GoverId { get; set; }
        public virtual Governorate? Governorate { get; set; }


        [Display(Name = "City")]
        [ForeignKey("City")]
        [Required(ErrorMessage = "Please select city")]
        public int CityId { get; set; }
        public virtual City? City { get; set; }


        [Display(Name = "Branch")]
        [ForeignKey("Branch")]
        [Required(ErrorMessage = "Please select branch")]
        public int BranchId { get; set; }
        public virtual Branch? Branch { get; set; }



        public List<Governorate>? Governorates { get; set; }
        public List<Branch>? Branchs { get; set; }
        public List<City>? Cities { get; set; }
    }
}
