using Microsoft.AspNetCore.Authorization;

namespace JWT.Api;

internal static class Policies
{
    public const string IsAdmin = "IsAdmin";

    public static void SetPolicies(AuthorizationOptions options)
    {
        // Define a policy by requiring a certain role
        options.AddPolicy(IsAdmin, policy => policy.RequireRole(Roles.Admin));
    }
}
