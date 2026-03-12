using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;
using Maliev.CompensationService.Domain.Authorization;

namespace Maliev.CompensationService.Tests.Integration;

public abstract class BaseIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly TestcontainersFixture Fixture;

    private readonly RSA _testRsa;

    protected BaseIntegrationTest(WebApplicationFactory<Program> factory, TestcontainersFixture fixture)
    {
        _testRsa = RSA.Create(2048);
        Fixture = fixture;

        var rsaParams = _testRsa.ExportParameters(false);

        Factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.UseSetting("CORS:AllowedOrigins:0", "http://localhost:3000");

            Environment.SetEnvironmentVariable("ConnectionStrings__CompensationDbContext", Fixture.PostgreSqlContainer.GetConnectionString());
            Environment.SetEnvironmentVariable("ConnectionStrings__redis", Fixture.RedisContainer.GetConnectionString());
            Environment.SetEnvironmentVariable("ConnectionStrings__rabbitmq", Fixture.RabbitMqContainer.GetConnectionString());
            Environment.SetEnvironmentVariable("JWT_PUBLIC_KEY_MODULUS", Convert.ToBase64String(rsaParams.Modulus!));
            Environment.SetEnvironmentVariable("JWT_PUBLIC_KEY_EXPONENT", Convert.ToBase64String(rsaParams.Exponent!));

            builder.ConfigureTestServices(services =>
            {
                services.Configure<MassTransitHostOptions>(options =>
                {
                    options.WaitUntilStarted = true;
                    options.StartTimeout = TimeSpan.FromSeconds(30);
                });

                services.AddMassTransitTestHarness();

                // Mock IIamServiceClient so PermissionAuthorizationHandler falls back to
                // JWT claims (returns false → handler uses claims from the token)
                var mockIamClient = new Mock<Maliev.Aspire.ServiceDefaults.IAM.IIamServiceClient>();
                mockIamClient
                    .Setup(x => x.CheckPermissionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(false);
                services.AddScoped(_ => mockIamClient.Object);

                // Override JWT validation to use our test RSA key
                services.PostConfigureAll<JwtBearerOptions>(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new RsaSecurityKey(_testRsa),
                        ClockSkew = TimeSpan.Zero,
                        NameClaimType = "sub",
                        RoleClaimType = "role"
                    };
                });
            });
        });
    }

    /// <summary>
    /// Creates an authenticated HTTP client with all compensation permissions.
    /// Use this for tests that don't need to test authorization boundaries.
    /// </summary>
    protected HttpClient CreateClient() => CreateAuthenticatedClient();

    /// <summary>
    /// Creates an authenticated HTTP client with the specified permissions.
    /// Pass an empty array to create a client with no permissions (for testing 403 scenarios).
    /// </summary>
    protected HttpClient CreateAuthenticatedClient(string[]? permissions = null)
    {
        // Default: all compensation permissions
        permissions ??= new[]
        {
            CompensationPermissions.Read,
            CompensationPermissions.ReadSensitive,
            CompensationPermissions.Update,
            CompensationPermissions.Admin,
            CompensationPermissions.Reports
        };

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, "test-user"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, "test-user")
        };

        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permissions", permission));
        }

        var signingCredentials = new SigningCredentials(
            new RsaSecurityKey(_testRsa),
            SecurityAlgorithms.RsaSha256);

        var token = new JwtSecurityToken(
            issuer: "test-issuer",
            audience: "test-audience",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: signingCredentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenString}");
        return client;
    }
}
