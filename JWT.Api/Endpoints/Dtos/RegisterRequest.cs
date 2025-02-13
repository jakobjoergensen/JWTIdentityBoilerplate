namespace JWT.Api.Endpoints.Dtos;

internal record RegisterRequest(string Email, string FirstName, string LastName, string Password);