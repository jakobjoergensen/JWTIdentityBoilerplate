using FastEndpoints;
using JWT.Api.Endpoints.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace JWT.Api.Endpoints;

internal class RefreshEndpoint(TokenManager tokenManager, UserManager<ApiUser> userManager, IdentityContext identityContext) : Endpoint<RefreshTokenRequest, LoginResponse>
{
    private readonly TokenManager _tokenManager = tokenManager;
    private readonly UserManager<ApiUser> _userManager = userManager;
    private readonly IdentityContext _identityContext = identityContext;

    public override void Configure()
    {
        Post("/Refresh");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RefreshTokenRequest req, CancellationToken ct)
    {
        var principal = _tokenManager.GetPrincipalFromExpiredToken(req.Token);

        if (principal?.Identity?.Name is null)
        {
            Console.WriteLine("principal?.Identity?.Name is null");
            await SendUnauthorizedAsync(ct);
            return;
        }

        var user = await _userManager.FindByNameAsync(principal.Identity.Name);
        if (user == null)
        {
            Console.WriteLine("user == null");
            await SendUnauthorizedAsync(ct);
            return;
        }

        var userRefreshTokens = _identityContext.Users
            .Include(u => u.RefreshTokens)
            .Where(u => u.Id == user.Id)
            .SelectMany(u => u.RefreshTokens).AsEnumerable() ?? Enumerable.Empty<RefreshTokens>();

        var validRefreshToken = userRefreshTokens.FirstOrDefault(x => 
                x.RefreshToken == req.RefreshToken
                && x.ExpiresAt > DateTime.UtcNow
                && x.RevokedAt == null);
        
        if (validRefreshToken is null)
        {
            Console.WriteLine("validRefreshToken is null");
            await SendUnauthorizedAsync(ct);
            return;
        }

        validRefreshToken.ExpiresAt = DateTime.UtcNow.AddDays(7);
        var token = await _tokenManager.GenerateToken(user);

        await _userManager.UpdateAsync(user);

        var loginResponse = new LoginResponse(
            AccessToken: new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt: token.ValidTo,
            RefreshToken: validRefreshToken.RefreshToken);

        await SendOkAsync(loginResponse, ct);
    }
}