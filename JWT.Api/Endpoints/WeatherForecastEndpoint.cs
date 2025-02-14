using FastEndpoints;
using JWT.Api.Endpoints.Dtos;

namespace JWT.Api.Endpoints;

internal class WeatherForecastEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/WeatherForecast");
        Roles(RoleNames.Admin, RoleNames.User);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
 
        var forecast = Enumerable
            .Range(1, 5)
            .Select(index => new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();

        await SendOkAsync(forecast, ct);
    }
}
