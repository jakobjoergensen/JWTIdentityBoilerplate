namespace JWTIdentityBoilerplate.Api.Endpoints.Dtos;

public record LoginResponse(string AccessToken, DateTime ExpiresAt, string RefreshToken);