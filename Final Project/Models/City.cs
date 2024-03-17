#nullable enable 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_Project.Models
{
    public class City
    {
        public int Id { get; set; }
        [MaxLength(25 , ErrorMessage = "City name must be between 3 and 25 Character")]
        [MinLength(3, ErrorMessage = "City name must be between 3 and 25 Character")]
        public string Name { get; set; }
        [Required(ErrorMessage ="Please Enter Valid Cost")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? ShippingCost { get; set; }
        [ForeignKey("Governorate")]
        public int? GoverId { get; set; }
        public virtual Governorate? Governorate { get; set; }
        public virtual List<Trader> Traders { get; set; } = new List<Trader>();


    }
}
