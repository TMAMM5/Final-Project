#nullable enable 
using System.ComponentModel.DataAnnotations;

namespace Final_Project.Models
{
    public class Governorate
    {
        public int Id { get; set; }
        [MaxLength(20, ErrorMessage = "Governorate name must be between 3 and 20 characters")]
        [MinLength(3, ErrorMessage = "Governorate name must be between 3 and 20 characters")]
        public string Name { get; set; }
        public virtual List<Trader> Traders { get; set; } = new List<Trader>();
        public bool IsDeleted { get; set; } = false;

        public virtual List<City> Cities { get; set; } = new List<City>();
    }
}