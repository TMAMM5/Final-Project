#nullable enable 
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_Project.Models
{
    public class DiscountType
    {
        public int Id { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Percentage { get; set; }
    }
}
