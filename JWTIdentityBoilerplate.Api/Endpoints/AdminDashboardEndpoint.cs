using FastEndpoints;
using JWTIdentityBoilerplate.Api.Constants;

namespace JWTIdentityBoilerplate.Api.Endpoints;

/// <summary>
/// Limiting access with a policy 
/// </summary>

internal class AdminDashboardEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/AdminDashboard");
        Policies(AppPolicies.IsAdmin);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendOkAsync(new { Message = "some value" });
    }
}
