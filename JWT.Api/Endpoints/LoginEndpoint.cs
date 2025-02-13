using FastEndpoints;
using JWT.Api.Endpoints.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using System.IdentityModel.Tokens.Jwt;

namespace JWT.Api.Endpoints;
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
        var user = await _userManager.FindByNameAsync(req.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, req.Password))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var token = await _tokenManager.GenerateToken(user);
        var refreshToken = _tokenManager.GenerateRefreshToken();

        user.RefreshTokens.Add(new RefreshTokens
        {
            RefreshToken = refreshToken,
            Expiration = DateTime.UtcNow.AddMinutes(10)
        });

        await _userManager.UpdateAsync(user);

        var loginResponse = new LoginResponse(
            AccessToken: new JwtSecurityTokenHandler().WriteToken(token),
            Expiration: token.ValidTo,
            RefreshToken: refreshToken);

        await SendOkAsync(loginResponse, ct);
    }
}
