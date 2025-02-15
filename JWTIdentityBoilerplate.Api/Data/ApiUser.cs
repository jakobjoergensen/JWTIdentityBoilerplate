using Microsoft.AspNetCore.Identity;

namespace JWTIdentityBoilerplate.Api.Data;

public class ApiUser : IdentityUser
{
    public bool IsSuspended { get; set; } = false;
    public List<RefreshTokens> RefreshTokens { get; set; } = new();
}