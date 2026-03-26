using Expo.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Expo.Infrastructure.Seeders;

/// <summary>
/// Class to initialize data on DB
/// </summary>
public static class Seeder
{
    /// <summary>
    /// Start seed method: Define the default admin user, role and perimissions
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        foreach (Role role in Enum.GetValues(typeof(Role)))
        {
            var name = RoleHierarchy.GetRoleName(role);

            if (!await roleManager.RoleExistsAsync(name))
                await roleManager.CreateAsync(new IdentityRole(name));
        }

        //default admin user
        var admin = await userManager.FindByEmailAsync(Users.AdminUser.Email);

        if (admin == null)
        {
            admin = new IdentityUser
            {
                UserName = Users.AdminUser.Email,
                Email = Users.AdminUser.Email,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, Users.AdminUser.Passwrod);

            await userManager.AddToRoleAsync(admin, RoleHierarchy.GetRoleName(Role.Admin));
        }

        var adminRole = await roleManager.FindByNameAsync(RoleHierarchy.GetRoleName(Role.Admin));

        var permissions = RoleHierarchy.GetRolePermissions(Role.Admin);

        var roleClaims = await roleManager.GetClaimsAsync(adminRole);

        foreach (var permission in permissions)
        {
            var claim = new Claim(permission, "true");

            if (!roleClaims.Any(c => c.Type == claim.Type))
                await roleManager.AddClaimAsync(adminRole, claim);
        }
    }
}