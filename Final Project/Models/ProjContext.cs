using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Final_Project.Models
{
    public class ProjContext : IdentityDbContext<Account>
    {
        public ProjContext() 
        {

        }

        public ProjContext(DbContextOptions<ProjContext> options) : base(options)
        {

        }

        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Representative> Representatives { get; set; }
        public virtual DbSet<Branch> Branches { get; set; }

        public virtual DbSet<DeliverType> DeliverTypes { get; set; }
        public virtual DbSet<Governorate> governorates { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<DiscountType> DiscountTypes { get; set; }
        public virtual DbSet<Trader> Traders { get; set; }

        public virtual DbSet<WeightSetting> WeightSetting { get; set; }
        public virtual DbSet<OrderState> OrderStates { get; set; }
        public virtual DbSet<OrderType> OrderTypes { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<TraderSpecialPriceForCities> TraderSpecialPriceForCities { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Employee>().Property(m => m.creationDate).HasDefaultValueSql("GetDate()");
            builder.Entity<Branch>().Property(m => m.CreationDate).HasDefaultValueSql("GetDate()");
            builder.Entity<Order>().Property(m => m.creationDate).HasDefaultValueSql("GetDate()");
            builder.Entity<Representative>().Property(m => m.creationDate).HasDefaultValueSql("GetDate()");
            builder.Entity<Trader>().Property(m => m.creationDate).HasDefaultValueSql("GetDate()");
            builder.Entity<Branch>().Property(m => m.CreationDate).HasDefaultValueSql("GetDate()");
            builder.Entity<Branch>().Property(m => m.IsDeleted).HasDefaultValue(false);
            builder.Entity<Employee>().Property(m => m.IsDeleted).HasDefaultValue(false);
            builder.Entity<Order>().Property(m => m.IsDeleted).HasDefaultValue(false);
            builder.Entity<Product>().Property(m => m.IsDeleted).HasDefaultValue(false);
            builder.Entity<Representative>().Property(m => m.IsDeleted).HasDefaultValue(false);
            builder.Entity<Trader>().Property(m => m.IsDeleted).HasDefaultValue(false);
            builder.Entity<TraderSpecialPriceForCities>().Property(m => m.IsDeleted).HasDefaultValue(false);


            builder.Entity<Governorate>().HasData(new Governorate { Id = 1, Name = "Cairo" });

            //صفحه 13 
            builder.Entity<DeliverType>().HasData(
                new DeliverType { Id = 1, Type = "Normal", Price = 0 },
                new DeliverType { Id = 2, Type = "2 Days", Price = 30 },
                new DeliverType { Id = 3, Type = "24 Hours", Price = 50 }
            );

            //صفحه 14
            builder.Entity<OrderState>().HasData(
                new OrderState { Id = 1, Name = "New" },
                new OrderState { Id = 2, Name = "Waiting" },
                new OrderState { Id = 3, Name = "Delivered to the representative" },
                new OrderState { Id = 4, Name = "Delivered to the client" },
                new OrderState { Id = 5, Name = "Cannot reach" },
                new OrderState { Id = 6, Name = "Postponed" },
                new OrderState { Id = 7, Name = "Partially delivered" },
                new OrderState { Id = 8, Name = "Canceled by the client" },
                new OrderState { Id = 9, Name = "Declined but Paid" },
                new OrderState { Id = 10, Name = "Declined but Partially Paid" },
                new OrderState { Id = 11, Name = "Declined without Payment" }
            );

            builder.Entity<OrderType>().HasData(
                new OrderType { Id = 1, Name = "From Branch" },
                new OrderType { Id = 2, Name = "From Trader" }
            );
            //صفحه 13
            builder.Entity<PaymentMethod>().HasData(
                new PaymentMethod { Id = 1, Name = "Cash" },
                new PaymentMethod { Id = 2, Name = "Visa" }
            );
            //صفحه 11
            builder.Entity<WeightSetting>().HasData(
                new WeightSetting { Id = 1, DefaultSize = 10, PriceForEachExtraKilo = 100 }
            );



        }

   

    }
}

