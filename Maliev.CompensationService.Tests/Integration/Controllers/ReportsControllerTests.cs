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
public class ReportsControllerTests : BaseIntegrationTest
{
    public ReportsControllerTests(WebApplicationFactory<Program> factory, TestcontainersFixture fixture)
        : base(factory, fixture)
    {
    }

    [Fact]
    public async Task GetCompensationAnalysis_ShouldReturnOk()
    {
        // Arrange
        var client = CreateClient();
        var departmentId = Guid.NewGuid();

        using (var scope = Factory.Services.CreateScope())
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
        var client = CreateClient();

        using (var scope = Factory.Services.CreateScope())
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
