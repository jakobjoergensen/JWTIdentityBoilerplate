namespace JWTIdentityBoilerplate.Api.Data;

public class RefreshTokens
{
    public Guid Id { get; init; }
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; set; } = DateTime.MinValue;
    public DateTime? RevokedAt { get; set; } = null;
}