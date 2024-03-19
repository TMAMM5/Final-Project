using Final_Project.Models;
using Final_Project.Repository.BranchRepo;
using Final_Project.Repository.CityRepo;
using Final_Project.Repository.DeliverTypeRepo;
using Final_Project.Repository.DiscountTypeRepo;
using Final_Project.Repository.GovernorateRepo;
using Final_Project.Repository.OrderRepo;
using Final_Project.Repository.OrderStateRepo;
using Final_Project.Repository.OrderTypeRepo;
using Final_Project.Repository.PaymentMethodRepo;
using Final_Project.Repository.ProductRepo;
using Final_Project.Repository.RepresintativeRepo;
using Final_Project.Repository.TraderRepo;
using Final_Project.Repository.WeightSettingRepo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace Final_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IBranchRepository, BranchRepository>();
            builder.Services.AddScoped<ICityRepository, CityRepository>();
            builder.Services.AddScoped<IDeliverTypeRepository, DeliverTypeRepository>();
            builder.Services.AddScoped<IDiscountTypeRepository, DiscountTypeRepository>();
            builder.Services.AddScoped<IGovernorateRepository, GovernorateRepository>();
            builder.Services.AddScoped<IOrderStateRepository, OrderStateRepository>();
            builder.Services.AddScoped<IOrderTypeRepository, OrderTypeRepository>();
            builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
            builder.Services.AddScoped<IProductRepositoy,ProductRepositoy>();
            builder.Services.AddScoped<IRepresintativeRepository, RepresintativeRepository>();
            builder.Services.AddScoped<ITraderRepository, TraderRepository>();
            builder.Services.AddScoped<IWeightSettingRepo, WeightSettingRepo>();

            builder.Services.AddDbContext<ProjContext>(p => p.UseLazyLoadingProxies().UseSqlServer(
                        builder.Configuration.GetConnectionString("Project")));
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
            }).AddEntityFrameworkStores<ProjContext>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
