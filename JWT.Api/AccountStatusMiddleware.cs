using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace JWT.Api;

internal class AccountStatusMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext httpContext, UserManager<ApiUser> userManager)
    {
        if (httpContext.User.Identity!.IsAuthenticated)
        {
            // Get the user's identity from the JWT
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                // Check if the user account is suspended
                var user = await userManager.FindByIdAsync(userId);
                if (user != null && user.IsSuspended)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await httpContext.Response.WriteAsync("Access denied: The account is suspended.");
                    return; // Stop any further processing down the pipeline
                }
            }
        }

        await _next(httpContext);
    }
}