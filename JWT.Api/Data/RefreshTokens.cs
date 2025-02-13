namespace JWT.Api.Data;

public class RefreshTokens
{
    public Guid Id { get; init; }
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime Expiration { get; set; } = DateTime.MinValue;
}