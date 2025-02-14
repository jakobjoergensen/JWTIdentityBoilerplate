using Microsoft.AspNetCore.Identity;

namespace JWT.Api;

internal static class InitializeData
{
    public static async Task SeedRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApiUser>>();

        string[] roleNames = 
        { 
            RoleNames.Admin, 
            RoleNames.User
        };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }


    public static async Task SeedAdminUser(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApiUser>>();

        var user = new ApiUser { UserName = "captainsparrow", Email = "jack.sparrow@theblackpearl.com", EmailConfirmed = true };

        var existingUser = await userManager.FindByEmailAsync(user.Email);
        if (existingUser != null)
        {
            await AddToRole(userManager, existingUser, RoleNames.Admin);
            return;
        }

        var seedResult = await userManager.CreateAsync(user, "Password123!");
        if (seedResult.Succeeded)
        {
            await AddToRole(userManager, user, RoleNames.Admin);
        }
    }


    public static async Task SeedUsers(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApiUser>>();

        ApiUser[] users =
        {
            new ApiUser { UserName = "elizabeth-swann", Email = "elizabeth.swann@port-royal.gov", EmailConfirmed = true },
            new ApiUser { UserName = "willturner", Email = "william.turner@theroyalblacksmith.com", EmailConfirmed = true },
            new ApiUser { UserName = "barbossa", Email = "barbossa@cortes-greed.org", EmailConfirmed = true }
        };

        foreach (var user in users)
        {
            var existingUser = await userManager.FindByEmailAsync(user.Email!);
            if (existingUser != null)
            {
                await AddToRole(userManager, existingUser, RoleNames.User);
                continue;
            }

            var seedResult = await userManager.CreateAsync(user, "Password123!");

            if (seedResult.Succeeded)
            {
                await AddToRole(userManager, user, RoleNames.User);
            }
        }
    }


    private static async Task AddToRole(UserManager<ApiUser> userManager, ApiUser user, string roleName)
    {
        if (await userManager.IsInRoleAsync(user, roleName))
        {
            return;
        }

        await userManager.AddToRoleAsync(user, roleName);
    }
}