using Microsoft.AspNetCore.Identity;

namespace JWT.Api.Data;

public class ApiUser : IdentityUser
{
    public List<RefreshTokens> RefreshTokens { get; set; } = new();
}