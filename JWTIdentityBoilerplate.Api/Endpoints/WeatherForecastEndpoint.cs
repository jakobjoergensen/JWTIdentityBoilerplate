using FastEndpoints;
using JWTIdentityBoilerplate.Api.Constants;
using JWTIdentityBoilerplate.Api.Endpoints.Dtos;

namespace JWTIdentityBoilerplate.Api.Endpoints;

/// <summary>
/// Example of limiting access by multiple roles
/// </summary>

internal class WeatherForecastEndpoint : EndpointWithoutRequest<IEnumerable<WeatherForecastResponse>>
{
    public override void Configure()
    {
        Get("/WeatherForecast");
        Roles(AppRoles.Admin, AppRoles.User);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        var forecast = Enumerable
            .Range(1, 5)
            .Select(index => new WeatherForecastResponse
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();

        await SendOkAsync(forecast, ct);
    }
}
