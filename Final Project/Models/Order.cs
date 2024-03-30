#nullable enable 
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Final_Project.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }

        [Display(Name = "Order Type ")]
        [ForeignKey("OrderType")]
        public int? OrderTypeId { get; set; }
        public virtual OrderType? OrderType { get; set; }

        [MaxLength(50)]
        [MinLength(8, ErrorMessage = "Name must be at Least 8 Character Long.")]
        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Invalid Phone Number")]
        [Display(Name = "Phone Number 1")]
        public string Phone1 { get; set; }

        [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Invalid Phone Number")]
        [Display(Name = "Phone Number 2")]
        public string Phone2 { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string ClientEmailAddress { get; set; }

        [Display(Name = "Governorate")]
        [ForeignKey("ClientGovernorate")]
        public int? ClientGovernorateId { get; set; }
        public virtual Governorate? ClientGovernorate { get; set; }

        [Display(Name = "City")]
        [ForeignKey("ClientCity")]
        public int? ClientCityId { get; set; }
        public virtual City? ClientCity { get; set; }

        [Display(Name = "Deliver To Village ?")]
        public bool DeliverToVillage { get; set; }

        //[Required(ErrorMessage ="Invalid Village Or Street")]
        [Display(Name ="Village & Street")]
        public string? Village_Street { get; set; }

        [Display(Name = "Delivery Type ")]
        [ForeignKey("DeliverType")]
        public int? DeliveryTypeId { get; set; }
        public virtual DeliverType? DeliverType { get; set; }

        [Display(Name = "Payment Method")]
        [ForeignKey("PaymentMethod")]
        public int? PaymentMethodId { get; set; }
        public virtual PaymentMethod? PaymentMethod { get; set; }

        [Display(Name = "Branch")]
        [ForeignKey("Branch")]
        public int? BranchId { get; set; }
        public virtual Branch? Branch { get; set; }

        public DateTime creationDate { get; set; } = DateTime.Now;

        public virtual List<Product> Products { get; set; } = new List<Product>();


        [ForeignKey("OrderState")]
        public int? OrderStateId { get; set; } = 1;
        public virtual OrderState? OrderState { get; set; }

        [ForeignKey("Trader")]
        public string TraderId { get; set; }
        public virtual Trader? Trader { get; set; }

        [ForeignKey("Representative")]
        public string? RepresentativeId { get; set; }
        public virtual Representative? Representative { get; set; }


        public bool IsDeleted { get; set; }


        [Display(Name = "Order Price")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? OrderPrice { get; set; }

        [Display(Name = "Order Price Recieved")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? OrderPriceRecieved { get; set; } = 0;

        [Display(Name = "Shipping Price")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? ShippingPrice { get; set; }

        [Display(Name = "Shipping Price Recieved ")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? ShippingPriceRecived { get; set; } = 0;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TotalWeight { get; set; }


    }
}
