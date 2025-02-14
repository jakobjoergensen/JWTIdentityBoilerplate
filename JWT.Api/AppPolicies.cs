using Microsoft.AspNetCore.Authorization;

namespace JWT.Api;

internal static class AppPolicies
{
    public const string IsAdmin = "IsAdmin";
    public const string CanSuspend = "CanSuspend";

    public static void SetPolicies(AuthorizationOptions options)
    {
        // Define a policy by requiring a certain role
        options.AddPolicy(IsAdmin, policy => policy.RequireRole(AppRoles.Admin));

        // Define a policy that requires a role OR a claim
        options.AddPolicy(CanSuspend, policy =>
        {
            policy.RequireAssertion(context =>
                context.User.IsInRole(AppRoles.Admin)
                || context.User.HasClaim(c => c.Type == AppClaims.CanSuspend));
        });
    }
}
