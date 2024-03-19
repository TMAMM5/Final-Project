#nullable enable 
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Final_Project.Models
{
    public class Branch
    {
        public int Id { get; set; }

        [MaxLength(30)]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
        public string Name { get; set; }

        public DateTime? CreationDate { get; set; }

        public bool IsDeleted { get; set; } = false;
       
        public virtual List<ApplicationUser>? Users { get; set; } = new List<ApplicationUser>();
    }
}
