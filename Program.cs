using CartonCaps;
using CartonCaps.ReferralFeature.Data;
using CartonCaps.ReferralFeature.Integrations;
using CartonCaps.ReferralFeature.Integrations.Interfaces;
using CartonCaps.ReferralFeature.Repositories;
using CartonCaps.ReferralFeature.Repositories.Interfaces;
using CartonCaps.ReferralFeature.Services;
using CartonCaps.ReferralFeature.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<ReferralDbContext>(options =>
    options.UseInMemoryDatabase("ReferralDb"));

// Register repositories
builder.Services.AddScoped<IReferralCodeRepository, ReferralCodeRepository>();
builder.Services.AddScoped<IReferralRepository, ReferralRepository>();

// Register integrations
builder.Services.AddScoped<IDeepLinkService, MockDeepLinkService>();
builder.Services.AddScoped<IReferralCodeGenerator, ReferralCodeGenerator>();
// Register services
builder.Services.AddScoped<IReferralService, ReferralService>();

// Add authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    // Configure JWT Bearer Auth to expect certain parameters
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = false,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<FakeJwtUserMiddleware>();
    app.MapOpenApi();
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// Add authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
