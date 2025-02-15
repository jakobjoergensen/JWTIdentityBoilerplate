using FastEndpoints;
using JWT.Api.Constants;
using JWT.Api.Endpoints.Dtos;
using Microsoft.AspNetCore.Identity;

namespace JWT.Api.Endpoints;

/// <summary>
/// An endpoint that requires a policy
/// The policy requires either a role OR a claim
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
