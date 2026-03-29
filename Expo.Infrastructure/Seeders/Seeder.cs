using Expo.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Expo.Infrastructure.Seeders;

/// <summary>
/// Class to initialize default data on the database
/// </summary>
public static class Seeder
{
    /// <summary>
    /// Seed default roles, admin user, and permissions
    /// </summary>
    /// <param name="services">Service provider</param>
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // Create default roles
        foreach (Role role in Enum.GetValues(typeof(Role)))
        {
            var roleName = RoleHierarchy.GetRoleName(role);

            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        // Create default admin user
        var adminEmail = Users.AdminUser.Email;
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(adminUser, Users.AdminUser.Password);
            await userManager.AddToRoleAsync(adminUser, RoleHierarchy.GetRoleName(Role.Admin));
        }

        // Assign default permissions to admin role
        var adminRoleName = RoleHierarchy.GetRoleName(Role.Admin);
        var adminRole = await roleManager.FindByNameAsync(adminRoleName);
        var permissions = RoleHierarchy.GetRolePermissions(Role.Admin);

        var existingClaims = await roleManager.GetClaimsAsync(adminRole);

        foreach (var permission in permissions)
        {
            var claim = new Claim(permission, "true");

            if (!existingClaims.Any(c => c.Type == claim.Type))
                await roleManager.AddClaimAsync(adminRole, claim);
        }
    }
}