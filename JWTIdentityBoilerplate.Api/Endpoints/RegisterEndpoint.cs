using FastEndpoints;
using JWTIdentityBoilerplate.Api.Data;
using JWTIdentityBoilerplate.Api.Endpoints.Dtos;
using Microsoft.AspNetCore.Identity;

namespace JWTIdentityBoilerplate.Api.Endpoints;

internal class RegisterEndpoint(UserManager<ApiUser> userManager) : Endpoint<RegisterRequest>
{
    private readonly UserManager<ApiUser> _userManager = userManager;

    public override void Configure()
    {
        Post("/Register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var user = new ApiUser
        {
            UserName = req.Username,
            Email = req.Email,
            EmailConfirmed = true
        };

        var passwordHasher = new PasswordHasher<ApiUser>();
        var hashedPassword = passwordHasher.HashPassword(user, req.Password);
        user.PasswordHash = hashedPassword;

        var result = await _userManager.CreateAsync(user);

        if (result.Succeeded)
        {
            await SendNoContentAsync(ct);
            return;
        }

        await SendErrorsAsync(cancellation: ct);
    }
}