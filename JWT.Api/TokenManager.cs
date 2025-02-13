using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace JWT.Api;

internal class TokenManager(IConfiguration configuration)
{
    private readonly IConfiguration _configuration = configuration;

    public Task<JwtSecurityToken> GenerateToken(ApiUser user)
    {
        var key = Encoding.UTF8.GetBytes(Guard.Against.NullOrEmpty(_configuration.GetValue<string>("TokenKey")));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: Guard.Against.NullOrEmpty(_configuration.GetValue<string>("Issuer")),
            audience: Guard.Against.NullOrEmpty(_configuration.GetValue<string>("Audience")),
            expires: DateTime.UtcNow.AddSeconds(30),
            claims: claims,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return Task.FromResult(token);
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