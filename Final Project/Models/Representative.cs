#nullable enable 
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Final_Project.Models
{
    public class Representative
    {
        //public int Id { get; set; }
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public virtual ApplicationUser AppUser { get; set; }


        [Display(Name = "Company Percentage Of Order")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? CompanyPercentageOfOrder { get; set; }

        [Display(Name = "Governorate")]
        [ForeignKey("Governorate")]
        public int? GovernorateId { get; set; }
        public virtual Governorate? Governorate { get; set; }

        [Display(Name = "Branch")]
        [ForeignKey("Branch")]
        public int? BranchId { get; set; }
        public virtual Branch? Branch { get; set; }

        [Display(Name = "Discount Type")]
        [ForeignKey("DiscountType")]
        public int? DiscountTypeId { get; set; }
        public virtual DiscountType? DiscountType { get; set; }

        public bool IsDeleted { get; set; }
    }
}