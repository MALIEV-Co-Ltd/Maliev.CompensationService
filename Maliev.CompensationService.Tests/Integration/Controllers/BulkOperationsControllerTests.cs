using System.Net;
using System.Net.Http.Json;
using Maliev.CompensationService.Application.Commands;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Domain.Authorization;
using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Domain.Enums;
using Maliev.CompensationService.Infrastructure.Data;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Maliev.CompensationService.Tests.Integration.Controllers;

[Collection("Testcontainers")]
public class BulkOperationsControllerTests : BaseIntegrationTest
{
    public BulkOperationsControllerTests(WebApplicationFactory<Program> factory, TestcontainersFixture fixture)
        : base(factory, fixture)
    {
    }

    [Fact]
    public async Task BulkSalaryIncrease_Preview_ShouldReturnAccepted()
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
                BaseSalary = 50000,
                Currency = "USD",
                IsCurrent = true,
                EffectiveDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        var postData = new BulkSalaryIncreaseDto
        {
            DepartmentId = departmentId,
            PercentageIncrease = 10,
            Reason = "Annual Increase",
            EffectiveDate = DateTime.UtcNow.AddDays(30),
            PreviewOnly = true
        };

        // Act
        var response = await client.PostAsJsonSnakeCaseAsync("/compensation/v1/bulk/salary-increases", postData);

        // Assert
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        var result = await response.Content.ReadFromJsonSnakeCaseAsync<BulkSalaryIncreaseResultDto>();
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.JobId);
        Assert.Equal(1, result.TotalEmployeesProcessed);
        Assert.Equal(5000, result.TotalIncreaseAmount);
    }

    [Fact]
    public async Task BulkSalaryIncrease_Real_ShouldReturnAccepted()
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
                BaseSalary = 50000,
                Currency = "USD",
                IsCurrent = true,
                EffectiveDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        var postData = new BulkSalaryIncreaseDto
        {
            DepartmentId = departmentId,
            PercentageIncrease = 5,
            Reason = "Real Increase",
            EffectiveDate = DateTime.UtcNow.AddDays(10),
            PreviewOnly = false
        };

        // Act
        var response = await client.PostAsJsonSnakeCaseAsync("/compensation/v1/bulk/salary-increases", postData);

        // Assert
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        var result = await response.Content.ReadFromJsonSnakeCaseAsync<BulkSalaryIncreaseResultDto>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetBulkJobStatus_ShouldReturnOk_WhenJobExists()
    {
        // Arrange
        var client = CreateClient();
        var jobId = Guid.NewGuid();

        using (var scope = Factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CompensationDbContext>();
            await context.Database.EnsureCreatedAsync();

            context.Set<BulkJob>().Add(new BulkJob
            {
                Id = jobId,
                JobType = "SalaryIncrease",
                Status = BulkJobStatus.Completed,
                StartedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow,
                SuccessCount = 10,
                FailureCount = 0,
                StartedBy = Guid.NewGuid()
            });
            await context.SaveChangesAsync();
        }

        // Act
        var response = await client.GetAsync($"/compensation/v1/bulk/jobs/{jobId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var status = await response.Content.ReadFromJsonSnakeCaseAsync<BulkJobStatusDto>();
        Assert.NotNull(status);
        Assert.Equal(jobId, status.Id);
        Assert.Equal(BulkJobStatus.Completed, status.Status);
    }
}

