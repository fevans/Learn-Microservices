using Identity.Service.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.Service.Extensions;


public static class HostExtensions
{
    private static async Task SeedRoles(RoleManager<ApplicationRole> roleManager)
    {
        foreach (var roleName in new[] { Roles.Admin, Roles.Player })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            }
        }
    }
    
    private static async Task AdminUser(UserManager<ApplicationUser> userManager)
    {
        // Seed admin user
        const string adminEmail    = "admin@playeconomy.com";
        const string adminPassword = "Admin1234";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                Id       = Guid.NewGuid(),
                UserName = "admin",
                Email    = adminEmail,
                Gil      = 0
            };

            await userManager.CreateAsync(adminUser, adminPassword);
            await userManager.AddToRoleAsync(adminUser, Roles.Admin);
        }
    }
    
    public  static async Task SeedRolesAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<ApplicationRole>>();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        await SeedRoles(roleManager);
        await AdminUser(userManager);
    }
}