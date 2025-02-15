using FastEndpoints;
using JWTIdentityBoilerplate.Api.Endpoints.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace JWTIdentityBoilerplate.Api.Endpoints;
internal class LoginEndpoint(TokenManager tokenManager, UserManager<ApiUser> userManager) : Endpoint<LoginRequest, LoginResponse>
{
    private readonly TokenManager _tokenManager = tokenManager;
    private readonly UserManager<ApiUser> _userManager = userManager;

    public override void Configure()
    {
        Post("/Login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await FindUser(req.Email ?? req.Username);

        if (user == null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            await SendForbiddenAsync(ct);
            return;
        }

        // Attempting user sign in
        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, req.Password);

        if (!isPasswordCorrect)
        {
            await _userManager.AccessFailedAsync(user); // Increment failed login count
            await SendUnauthorizedAsync(ct);
            return;
        }

        var token = await _tokenManager.GenerateToken(user);
        var refreshToken = _tokenManager.GenerateRefreshToken();

        user.RefreshTokens.Add(new RefreshTokens
        {
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        });

        await _userManager.UpdateAsync(user);

        var loginResponse = new LoginResponse(
            AccessToken: new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt: token.ValidTo,
            RefreshToken: refreshToken);

        await SendOkAsync(loginResponse, ct);
    }


    private async Task<ApiUser?> FindUser(string? emailOrUsername) =>
        await _userManager.Users
            .Where(u => u.Email == emailOrUsername || u.UserName == emailOrUsername)
            .FirstOrDefaultAsync();
}
