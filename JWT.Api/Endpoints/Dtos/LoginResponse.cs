namespace JWT.Api.Endpoints.Dtos;

public record LoginResponse(string AccessToken, DateTime Expiration, string RefreshToken);