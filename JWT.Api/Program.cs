using System.Text;
using JWT.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

var tokenKey = Guard.Against.NullOrEmpty(builder.Configuration.GetValue<string>("TokenKey"));
var connectionString = Guard.Against.NullOrEmpty(builder.Configuration.GetConnectionString("IdentityContext"));

builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlServer(connectionString));
builder.Services
    .AddIdentityCore<ApiUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<IdentityContext>();

builder.Services.AddSingleton<TokenManager>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            ValidIssuer = Guard.Against.NullOrEmpty(builder.Configuration.GetValue<string>("Issuer")),
            ValidAudience = Guard.Against.NullOrEmpty(builder.Configuration.GetValue<string>("Audience")),
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ClockSkew = TimeSpan.FromSeconds(10)
        };
    });

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        scope.ServiceProvider.GetRequiredService<IdentityContext>().Database.Migrate();
    }
}

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints();
app.UseSwaggerGen();

app.Run();