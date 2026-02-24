using System.Net;
using System.Net.Http.Json;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;
using Maliev.CompensationService.Tests.Integration.Controllers;

namespace Maliev.CompensationService.Tests.Integration;

public abstract class BaseIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly TestcontainersFixture Fixture;

    protected BaseIntegrationTest(WebApplicationFactory<Program> factory, TestcontainersFixture fixture)
    {
        Fixture = fixture;
        Factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.UseSetting("CORS:AllowedOrigins:0", "http://localhost:3000");

            Environment.SetEnvironmentVariable("ConnectionStrings__CompensationDbContext", Fixture.PostgreSqlContainer.GetConnectionString());
            Environment.SetEnvironmentVariable("ConnectionStrings__redis", Fixture.RedisContainer.GetConnectionString());
            Environment.SetEnvironmentVariable("ConnectionStrings__rabbitmq", Fixture.RabbitMqContainer.GetConnectionString());

            builder.ConfigureTestServices(services =>
            {
                services.Configure<MassTransitHostOptions>(options =>
                {
                    options.WaitUntilStarted = true;
                    options.StartTimeout = TimeSpan.FromSeconds(30);
                });

                services.AddMassTransitTestHarness();

                services.RemoveAll<IAuthorizationHandler>();
                services.AddSingleton<IAuthorizationHandler, TestAuthHandler>();
            });
        });
    }

    protected HttpClient CreateClient() => Factory.CreateClient();
}
