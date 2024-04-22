#nullable enable 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_Project.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string OrderNO { get; set; }
        [Required(ErrorMessage = "Product Name is Required")]

        public string Name { get; set; }
        [Required(ErrorMessage = "Product Quantity is Required")]

        public int? Quantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Required(ErrorMessage = "Product Weight is Required")]

        public decimal? Weight { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Required(ErrorMessage = "Product Price is Required")]

        public decimal? Price { get; set; }

        [ForeignKey("Order")]
        public int? OrderId { get; set; }
        public virtual Order? Order { get; set; }

        public bool IsDeleted { get; set; }
    }
}