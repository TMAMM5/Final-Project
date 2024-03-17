#nullable enable 
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_Project.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string OrderNO { get; set; }

        public string Name { get; set; }

        public int? Quantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Weight { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Price { get; set; }

        [ForeignKey("Order")]
        public int? OrderId { get; set; }
        public virtual Order? Order { get; set; }

        public bool IsDeleted { get; set; }
    }
}