using Final_Project.Needs;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Final_Project.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            
                await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
                await roleManager.CreateAsync(new IdentityRole(Roles.Trader.ToString()));
                await roleManager.CreateAsync(new IdentityRole(Roles.Representative.ToString()));
        
            await roleManager.SeedClaimsForRepresentative();
            await roleManager.SeedClaimsForTrader();

        }
        private static async Task SeedClaimsForRepresentative(this RoleManager<IdentityRole> roleManager)
        {
            var representativeRole = await roleManager.FindByNameAsync(Roles.Representative.ToString());

            await roleManager.AddClaimAsync(representativeRole, new Claim("Permission", $"Permissions.Representative.View"));

        }
        private static async Task SeedClaimsForTrader(this RoleManager<IdentityRole> roleManager)
        {
            var traderRole = await roleManager.FindByNameAsync(Roles.Trader.ToString());
            await roleManager.AddClaimAsync(traderRole, new Claim("Permission", $"Permissions.Trader.View"));
            await roleManager.AddPermissionClaims(traderRole, Modules.Orders.ToString());

        }
    }
}
