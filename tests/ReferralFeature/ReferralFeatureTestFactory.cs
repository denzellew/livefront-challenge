using CartonCaps.ReferralFeature.Data;
using CartonCaps.ReferralFeature.Integrations.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CartonCaps.Tests.ReferralFeature;

public class ReferralFeatureTestFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName;

    public ReferralFeatureTestFactory()
    {
        // Set the environment variable for the test
        Environment.SetEnvironmentVariable("DatabaseProvider", "InMemory");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing db context registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ReferralDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Generate a unique database name for each test run
            var databaseName = Guid.NewGuid().ToString();

            // Add in-memory database with a unique name
            services.AddDbContext<ReferralDbContext>(options =>
                options.UseInMemoryDatabase(databaseName));

            // Configure mocks
            services.AddScoped(sp =>
            {
                var mock = new Mock<IReferralCodeGenerator>();
                mock.Setup(x => x.GenerateReferralCode())
                    .Returns("TEST123");
                return mock.Object;
            });
        });
    }

    public HttpClient CreateAuthenticatedClient(Guid userId)
    {
        var client = CreateClient();

        var token = CreateJwtToken(userId);

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public string CreateJwtToken(Guid userId)
    {
        // Create JWT payload with userId as sub claim
        var payload = new Dictionary<string, object>
        {
            { "sub", userId.ToString() },
            { "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds() }
        };

        // Create JWT header
        var header = new Dictionary<string, object>
        {
            { "alg", "HS256" },
            { "typ", "JWT" }
        };

        // Convert to Base64URL strings
        var headerJson = System.Text.Json.JsonSerializer.Serialize(header);
        var headerBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(headerJson))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');

        var payloadJson = System.Text.Json.JsonSerializer.Serialize(payload);
        var payloadBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(payloadJson))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');

        // For testing purposes, we can use a simple signature
        var signature = "test_signature";

        var token = $"{headerBase64}.{payloadBase64}.{signature}";

        return token;
    }

    public ReferralDbContext GetDbContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ReferralDbContext>();
    }
}