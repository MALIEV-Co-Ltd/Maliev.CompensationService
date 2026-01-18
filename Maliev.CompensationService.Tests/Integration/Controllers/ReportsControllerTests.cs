using System.Net;
using System.Net.Http.Json;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Infrastructure.Data;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Maliev.CompensationService.Tests.Integration.Controllers;

[Collection("Testcontainers")]
public class ReportsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly TestcontainersFixture _fixture;

    public ReportsControllerTests(WebApplicationFactory<Program> factory, TestcontainersFixture fixture)
    {
        _fixture = fixture;
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");

            Environment.SetEnvironmentVariable("ConnectionStrings__CompensationDbContext", _fixture.PostgreSqlContainer.GetConnectionString());
            Environment.SetEnvironmentVariable("ConnectionStrings__redis", _fixture.RedisContainer.GetConnectionString());
            Environment.SetEnvironmentVariable("ConnectionStrings__rabbitmq", _fixture.RabbitMqContainer.GetConnectionString());

            builder.ConfigureTestServices(services =>
            {
                services.Configure<MassTransitHostOptions>(options =>
                {
                    options.WaitUntilStarted = true;
                });

                services.AddMassTransitTestHarness();

                services.RemoveAll<IAuthorizationHandler>();
                services.AddSingleton<IAuthorizationHandler, TestAuthHandler>();
            });
        });
    }

    [Fact]
    public async Task GetCompensationAnalysis_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        var departmentId = Guid.NewGuid();

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CompensationDbContext>();
            await context.Database.EnsureCreatedAsync();

            context.Set<CompensationRecord>().Add(new CompensationRecord
            {
                Id = Guid.NewGuid(),
                EmployeeId = Guid.NewGuid(),
                DepartmentId = departmentId,
                BaseSalary = 100000,
                Currency = "USD",
                IsCurrent = true,
                EffectiveDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        // Act
        var response = await client.GetAsync($"/compensation/v1/reports/compensation-analysis?departmentId={departmentId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var report = await response.Content.ReadFromJsonSnakeCaseAsync<CompensationAnalysisDto>();
        Assert.NotNull(report);
        Assert.Equal(1, report.TotalEmployees);
        Assert.Equal(100000, report.AverageSalary);
    }

    [Fact]
    public async Task GetCompensationBudget_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClient();

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CompensationDbContext>();
            await context.Database.EnsureCreatedAsync();
        }

        // Act
        var response = await client.GetAsync("/compensation/v1/reports/compensation-budget");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var report = await response.Content.ReadFromJsonSnakeCaseAsync<CompensationAnalysisDto>();
        Assert.NotNull(report);
    }
}
