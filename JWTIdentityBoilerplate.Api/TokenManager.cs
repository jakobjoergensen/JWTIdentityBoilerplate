using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JWTIdentityBoilerplate.Api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace JWTIdentityBoilerplate.Api;

internal class TokenManager(IConfiguration configuration, UserManager<ApiUser> userManager)
{
    private readonly IConfiguration _configuration = configuration;
    private readonly UserManager<ApiUser> _userManager = userManager;

    public async Task<JwtSecurityToken> GenerateToken(ApiUser user)
    {
        var key = Encoding.UTF8.GetBytes(Guard.Against.NullOrEmpty(_configuration.GetValue<string>("TokenKey")));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add roles as claims
        var userRoles = await _userManager.GetRolesAsync(user);
        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: Guard.Against.NullOrEmpty(_configuration.GetValue<string>("Issuer")),
            audience: Guard.Against.NullOrEmpty(_configuration.GetValue<string>("Audience")),
            expires: DateTime.UtcNow.AddSeconds(30),
            claims: claims,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return token;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using (var generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var key = Encoding.UTF8.GetBytes(Guard.Against.NullOrEmpty(_configuration.GetValue<string>("TokenKey")));

        var validation = new TokenValidationParameters
        {
            ValidIssuer = Guard.Against.NullOrEmpty(_configuration.GetValue<string>("Issuer")),
            ValidAudience = Guard.Against.NullOrEmpty(_configuration.GetValue<string>("Audience")),
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = false
        };

        return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
    }
}