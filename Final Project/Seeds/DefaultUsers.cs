﻿using Final_Project.Models;
using Final_Project.Needs;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using System.Security.Claims;

namespace Final_Project.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedSuperAdminUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManger)
        {
            var defaultUser = new ApplicationUser
            {
                UserName = "superadmin@domain.com",
                Email = "superadmin@domain.com",
                EmailConfirmed = true,
               
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "aA123");
            }

            await userManager.AddToRoleAsync(defaultUser, Roles.SuperAdmin.ToString());

            await roleManger.SeedClaimsForSuperUser();
        }
        private static async Task SeedClaimsForSuperUser(this RoleManager<IdentityRole> roleManager)
        {
            var superAdminRole = await roleManager.FindByNameAsync(Roles.SuperAdmin.ToString());

            await roleManager.AddPermissionClaims(superAdminRole, Modules.Branches.ToString());
            await roleManager.AddPermissionClaims(superAdminRole, Modules.Roles.ToString());
            await roleManager.AddPermissionClaims(superAdminRole, Modules.Users.ToString());
            await roleManager.AddPermissionClaims(superAdminRole, Modules.City.ToString());
            await roleManager.AddPermissionClaims(superAdminRole, Modules.Governorate.ToString());
            await roleManager.AddPermissionClaims(superAdminRole, Modules.WeightSetting.ToString());
            await roleManager.AddClaimAsync(superAdminRole, new Claim("Permission", $"Permissions.OrderReports.View"));
            await roleManager.AddClaimAsync(superAdminRole, new Claim("Permission", $"Permissions.Orders.View"));
        }

        public static async Task AddPermissionClaims(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GeneratePermissionsList(module);

            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(c => c.Type == "Permission" && c.Value == permission))
                    await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            }
        }
    }
}
