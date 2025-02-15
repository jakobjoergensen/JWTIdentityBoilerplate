using JWTIdentityBoilerplate.Api.Constants;
using JWTIdentityBoilerplate.Api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace JWTIdentityBoilerplate.Api.Middlewares;

internal class ClaimsMiddleware(RequestDelegate next, IMemoryCache cache)
{
    private readonly RequestDelegate _next = next;
    private readonly IMemoryCache _cache = cache;

    public async Task Invoke(HttpContext context, UserManager<ApiUser> userManager)
    {
        // Checking if the user is authenticated before doing anything because:
        // -> Prevents unnecessary database calls. If the user isn’t authenticated, they won’t have an identity.
        // -> Some endpoints might allow anonymous access, for example login or token refresh endpoints
        // -> Avoid modifying an unauthenticated request
        // -> Ensures claims are only loaded for authenticated users
        if (!context.User.Identity!.IsAuthenticated)
        {
            await _next(context);
            return;
        }

        // Get the userId from the JWTs NameIdentifier claim
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            // The JWT does not have the NameIdentifier claim (userId), thus it is not possible to get the user's claims from the database
            // Move on to the next step in the middleware pipeline
            await _next(context);
            return;
        }

        var cacheKey = string.Concat(CacheKeyNames.CLAIMS, userId);

        // Try get the user claims from the cache
        if (!_cache.TryGetValue(cacheKey, out IEnumerable<Claim>? claims))
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Get the claims from the database
                claims = await userManager.GetClaimsAsync(user);

                // Cache the claims
                _cache.Set(cacheKey, claims, TimeSpan.FromMinutes(5));
            }
        }

        var existingClaims = ((ClaimsIdentity)context.User.Identity).Claims;
        var claimsIdentity = (ClaimsIdentity)context.User.Identity;

        // Add claims to the user identity
        foreach (var claim in claims!)
        {
            if (!existingClaims.Any(x => x.Type == claim.Type))
            {
                claimsIdentity.AddClaim(claim);
            }
        }

        // Move on to the next step in the middleware pipeline
        await _next(context);
    }
}
