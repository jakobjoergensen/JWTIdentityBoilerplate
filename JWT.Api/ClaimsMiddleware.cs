using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace JWT.Api;

internal class ClaimsMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context, UserManager<ApiUser> userManager)
    {
        if (context.User.Identity!.IsAuthenticated)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var existingClaims = ((ClaimsIdentity)context.User.Identity).Claims;
                    var claimsIdentity = (ClaimsIdentity)context.User.Identity;

                    // Load claims from AspNetUserClaims table
                    var claims = await userManager.GetClaimsAsync(user);

                    foreach (var claim in claims)
                    {
                        if (!existingClaims.Any(x => x.Type == claim.Type))
                        {
                            claimsIdentity.AddClaim(claim);
                        }
                    }
                }
            }
        }

        await _next(context);
    }
}
