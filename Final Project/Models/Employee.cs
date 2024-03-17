#nullable enable 
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Final_Project.Models
{
    public class Employee:Account
    {
        //public int Id { get; set; }
        public string Role { get; set; } = "Employee";

        [ForeignKey("Branch")]
        [Display(Name = "Branch")]
        [Required(ErrorMessage = "Please select branch")]
        public int? BranchId { get; set; }
        public virtual Branch? Branch { get; set; }
    }
}