using System.ComponentModel.DataAnnotations;

namespace Final_Project.ViewModel
{
    public class RoleFormVM
    {
        [MaxLength(20, ErrorMessage = "Maximum Length For Role Name is 20 Character")]
        [MinLength(3, ErrorMessage = "Minimum Length For Role Name is 3 Character")]
        public string Name { get; set; }
    }
}
