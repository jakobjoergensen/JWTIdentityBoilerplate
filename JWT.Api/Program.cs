using System.Text;
using JWT.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Identity;
using JWT.Api.Constants;
using JWT.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

var tokenKey = Guard.Against.NullOrEmpty(builder.Configuration.GetValue<string>("TokenKey"));
var connectionString = Guard.Against.NullOrEmpty(builder.Configuration.GetConnectionString("IdentityContext"));

builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlServer(connectionString));
builder.Services
    .AddIdentityCore<ApiUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = !builder.Environment.IsDevelopment();
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    })
    .AddRoles<IdentityRole>() // Add role support
    .AddRoleManager<RoleManager<IdentityRole>>() // Registers RoleManager
    .AddEntityFrameworkStores<IdentityContext>();

builder.Services.AddScoped<TokenManager>();

builder.Services.AddAuthorization(AppPolicies.SetPolicies);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    // Seed roles
    await InitializeData.SeedRoles(services);
    
    if (app.Environment.IsDevelopment())
    {
        // Apply database migrations
        services.GetRequiredService<IdentityContext>().Database.Migrate();

        // Seed users
        await InitializeData.SeedAdminUser(services);
        await InitializeData.SeedUsers(services);
    }
}

// Order is important!
app.UseAuthentication();
app.UseMiddleware<AccountStatusMiddleware>(); // Checks for suspended account
app.UseMiddleware<ClaimsMiddleware>(); // Adds all the user claims from the database to the context
app.UseAuthorization();

app.UseFastEndpoints();
app.UseSwaggerGen();

app.Run();