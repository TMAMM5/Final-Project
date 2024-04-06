using Final_Project.Filter;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Final_Project
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new WebHostBuilder()
              .UseKestrel(options =>
                  {
                      options.Limits.MaxRequestBufferSize = 302768;
                      options.Limits.MaxRequestLineSize = 302768;
                  });

            var builder = WebApplication.CreateBuilder(args);

            //policy
            builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            //--------
            //Repos
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IBranchRepository, BranchRepository>();
            builder.Services.AddScoped<ICityRepository, CityRepository>();
            builder.Services.AddScoped<IDeliverTypeRepository, DeliverTypeRepository>();
            builder.Services.AddScoped<IDiscountTypeRepository, DiscountTypeRepository>();
            builder.Services.AddScoped<IGovernorateRepository, GovernorateRepository>();
            builder.Services.AddScoped<IOrderStateRepository, OrderStateRepository>();
            builder.Services.AddScoped<IOrderTypeRepository, OrderTypeRepository>();
            builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
            builder.Services.AddScoped<IProductRepositoy, ProductRepositoy>();
            builder.Services.AddScoped<IRepresintativeRepository, RepresintativeRepository>();
            builder.Services.AddScoped<ITraderRepository, TraderRepository>();
            builder.Services.AddScoped<IWeightSettingRepo, WeightSettingRepo>();
            //------
            //context
            builder.Services.AddDbContext<ProjContext>(p => p.UseLazyLoadingProxies().UseSqlServer(
                        builder.Configuration.GetConnectionString("Project")));
            //----

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();


            // identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
            }).AddEntityFrameworkStores<ProjContext>().AddDefaultUI().AddDefaultTokenProviders();
           
            //---- -
            builder.Services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.Zero;
            });
            builder.Services.AddControllersWithViews();






            var app = builder.Build();
            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerProvider>();
            var logger = loggerFactory.CreateLogger("app");

            try
            {
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await Seeds.DefaultRoles.SeedAsync(roleManager);
                await Seeds.DefaultUsers.SeedSuperAdminUserAsync(userManager, roleManager);
                logger.LogInformation("Data seeded");
                logger.LogInformation("Application Started");
            }
            catch (System.Exception ex)
            {
                logger.LogWarning(ex, "An error occurred while seeding data");
            }

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
