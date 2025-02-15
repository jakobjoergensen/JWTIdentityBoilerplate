using FastEndpoints;
using JWTIdentityBoilerplate.Api.Constants;
using JWTIdentityBoilerplate.Api.Data;
using JWTIdentityBoilerplate.Api.Endpoints.Dtos;
using Microsoft.AspNetCore.Identity;

namespace JWTIdentityBoilerplate.Api.Endpoints;

/// <summary>
/// Limiting access by a policy that requires either a role OR a claim
/// </summary>


internal class SuspendEndpoint(UserManager<ApiUser> userManager) : Endpoint<SuspendRequest>
{
    private readonly UserManager<ApiUser> _userManager = userManager;

    public override void Configure()
    {
        Put("/Suspend");
        Policies(AppPolicies.CanSuspend);
    }

    public override async Task HandleAsync(SuspendRequest req, CancellationToken ct)
    {
        var user = await _userManager.FindByNameAsync(req.Username);

        if (user is not null)
        {
            user.IsSuspended = true;
            await _userManager.UpdateAsync(user);

            await SendNoContentAsync();
        }

        await SendNotFoundAsync();
    }
}
