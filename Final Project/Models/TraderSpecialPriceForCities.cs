#nullable enable 
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_Project.Models
{
    public class TraderSpecialPriceForCities
    {
        public int Id { get; set; }

        public int? Shippingprice { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("Trader")]
        public string TraderId { get; set; }
        public virtual Trader? Trader { get; set; }

        [ForeignKey("City")]
        public int? CityId { get; set; }
        public virtual City? City { get; set; }
    }
}