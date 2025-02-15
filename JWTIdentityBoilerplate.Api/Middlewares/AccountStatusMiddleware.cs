using JWTIdentityBoilerplate.Api.Constants;
using JWTIdentityBoilerplate.Api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace JWTIdentityBoilerplate.Api.Middlewares;

internal class AccountStatusMiddleware(RequestDelegate next, IMemoryCache cache)
{
    private readonly RequestDelegate _next = next;
    private readonly IMemoryCache _cache = cache;

    public async Task Invoke(HttpContext context, UserManager<ApiUser> userManager)
    {
        // Checking if the user is authenticated before doing anything because:
        // -> Prevents unnecessary database calls. If the user isn’t authenticated, they won’t have an identity.
        // -> Some endpoints might allow anonymous access, for example login or token refresh endpoints
        // -> Avoid modifying an unauthenticated request
        if (!context.User.Identity!.IsAuthenticated)
        {
            await _next(context); // Move on to the next step in the middleware pipeline
            return;
        }

        // Get the userId from the JWTs NameIdentifier claim
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            // The JWT does not have the NameIdentifier claim (userId), thus it is not possible to check the database
            // Move on to the next step in the middleware pipeline
            await _next(context);
            return;
        }

        var cacheKey = string.Concat(CacheKeyNames.SUSPENDED_ACCOUNT, userId);

        // Check if the suspended status is in cache 
        if (!_cache.TryGetValue(cacheKey, out bool? isSuspended))
        {
            // Get the suspended status from the database
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Cache the value
                isSuspended = user.IsSuspended;
                _cache.Set(cacheKey, isSuspended, TimeSpan.FromHours(1)); // Use a relative long-lived cached value here since the cached value will be removed when the suspended status is changed
            }

            isSuspended = false;
        }

        // Handle a suspended account
        if (isSuspended is null || (bool)isSuspended)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Access denied: The account is suspended.");
            return; // Important: do NOT move on to the next step in the middleware pipeline
        }

        // Move on to the next step in the middleware pipeline
        await _next(context);
    }
}