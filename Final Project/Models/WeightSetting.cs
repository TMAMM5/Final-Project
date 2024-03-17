#nullable enable 
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_Project.Models
{
    public class WeightSetting
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? DefaultSize { get; set; }

        public int? PriceForEachExtraKilo { get; set; }
    }
}
