#nullable enable 
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_Project.Models
{
    public class DeliverType
    {
        public int Id { get; set; }

        public string Type { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Price { get; set; }

    }
}
