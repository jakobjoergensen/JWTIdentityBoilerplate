using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace JWT.Api;

internal static class InitializeData
{
    public static async Task SeedRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApiUser>>();

        string[] roleNames = 
        { 
            AppRoles.Admin, 
            AppRoles.User
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
            await AddToRole(userManager, existingUser, AppRoles.Admin);
            return;
        }

        var seedResult = await userManager.CreateAsync(user, "Password123!");
        if (seedResult.Succeeded)
        {
            await AddToRole(userManager, user, AppRoles.Admin);
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
                await AddToRole(userManager, existingUser, AppRoles.User);
                continue;
            }

            var seedResult = await userManager.CreateAsync(user, "Password123!");

            if (seedResult.Succeeded)
            {
                await AddToRole(userManager, user, AppRoles.User);
            }

        }

        var willturner = await userManager.FindByNameAsync("willturner");
        if (willturner != null)
        {
            var existingClaims = await userManager.GetClaimsAsync(willturner);
            if (!existingClaims.Any(x => x.Type == AppClaims.CanSuspend))
            {
                await userManager.AddClaimAsync(willturner, new Claim(AppClaims.CanSuspend, string.Empty));
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