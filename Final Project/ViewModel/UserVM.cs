using Final_Project.Models;

namespace Final_Project.ViewModel
{
    public class UserVM
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime creationDate { get; set; }
        public virtual Branch? Branch { get; set; }
        public bool IsDeleted { get; set; }
        public string Role { get; set; }
    }
}
