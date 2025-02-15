namespace JWTIdentityBoilerplate.Api.Endpoints.Dtos;

internal record RegisterRequest(string Email, string Username, string Password);