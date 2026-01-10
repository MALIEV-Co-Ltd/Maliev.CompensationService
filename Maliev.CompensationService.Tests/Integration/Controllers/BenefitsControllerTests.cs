using System.Net;
using System.Net.Http.Json;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Domain.Authorization;
using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Domain.Enums;
using Maliev.CompensationService.Infrastructure.Data;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Maliev.CompensationService.Tests.Integration.Controllers;

[Collection("Testcontainers")]
public class BenefitsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly TestcontainersFixture _fixture;

    public BenefitsControllerTests(WebApplicationFactory<Program> factory, TestcontainersFixture fixture)
    {
        _fixture = fixture;
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");

            // Set environment variables for connection strings (read early in configuration pipeline)
            Environment.SetEnvironmentVariable("ConnectionStrings__CompensationDbContext", _fixture.PostgreSqlContainer.GetConnectionString());
            Environment.SetEnvironmentVariable("ConnectionStrings__redis", _fixture.RedisContainer.GetConnectionString());
            Environment.SetEnvironmentVariable("ConnectionStrings__rabbitmq", _fixture.RabbitMqContainer.GetConnectionString());
            Environment.SetEnvironmentVariable("Encryption__Key", "MDEyMzQ1Njc4OWFiY2RlZmdoaWprbG1ub3BxcnN0dXY=");

            builder.ConfigureTestServices(services =>
            {
                // Ensure MassTransit waits until started for tests to avoid race conditions
                services.Configure<MassTransitHostOptions>(options =>
                {
                    options.WaitUntilStarted = true;
                    options.StartTimeout = TimeSpan.FromSeconds(30);
                });

                // Add MassTransit Test Harness (overrides real MassTransit/RabbitMQ)
                services.AddMassTransitTestHarness();

                services.RemoveAll<IAuthorizationHandler>();
                services.AddSingleton<IAuthorizationHandler, AllowAnonymousHandler>();
            });
        });
    }

    [Fact]
    public async Task EnrollInBenefit_ShouldReturnCreated()
    {
        // Arrange
        var client = _factory.CreateClient();
        var employeeId = Guid.NewGuid();
        Guid benefitId;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CompensationDbContext>();
            await context.Database.EnsureCreatedAsync();

            var benefit = new Benefit { Id = Guid.NewGuid(), Name = "Integration Health", BenefitType = BenefitType.HealthInsurance };
            context.Set<Benefit>().Add(benefit);
            await context.SaveChangesAsync();
            benefitId = benefit.Id;
        }

        var postData = new EnrollInBenefitDto
        {
            BenefitId = benefitId,
            EnrollmentDate = DateTime.UtcNow,
            EmployeeContribution = 150,
            CoverageLevel = "Individual",
            Dependents = new List<DependentDto>
            {
                new DependentDto { FirstName = "Jane", LastName = "Doe", Relationship = DependentRelationship.Spouse, DateOfBirth = new DateTime(1990, 1, 1) }
            }
        };

        // Act
        var response = await client.PostAsJsonSnakeCaseAsync($"/compensation/v1/employees/{employeeId}/benefits/enrollments", postData);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonSnakeCaseAsync<BenefitsEnrollmentDto>();
        Assert.NotNull(result);
        Assert.Equal(employeeId, result.EmployeeId);
        Assert.Single(result.Dependents);
    }

    [Fact]
    public async Task UpdateBenefitsEnrollment_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        var employeeId = Guid.NewGuid();
        Guid enrollmentId;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CompensationDbContext>();
            await context.Database.EnsureCreatedAsync();

            var benefit = new Benefit { Id = Guid.NewGuid(), Name = "Update Health", BenefitType = BenefitType.HealthInsurance };
            context.Set<Benefit>().Add(benefit);

            var enrollment = new BenefitsEnrollment
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                BenefitId = benefit.Id,
                EnrollmentDate = DateTime.UtcNow,
                Status = EnrollmentStatus.Active
            };
            context.Set<BenefitsEnrollment>().Add(enrollment);
            await context.SaveChangesAsync();
            enrollmentId = enrollment.Id;
        }

        var updateData = new UpdateBenefitsEnrollmentDto
        {
            EmployeeContribution = 200,
            CoverageLevel = "Family",
            Dependents = new List<DependentDto>()
        };

        // Act
        var response = await client.PutAsJsonSnakeCaseAsync($"/compensation/v1/employees/{employeeId}/benefits/enrollments/{enrollmentId}", updateData);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonSnakeCaseAsync<BenefitsEnrollmentDto>();
        Assert.Equal(200, result!.EmployeeContribution);
        Assert.Equal("Family", result.CoverageLevel);
    }

    [Fact]
    public async Task TerminateBenefit_ShouldReturnNoContent()
    {
        // Arrange
        var client = _factory.CreateClient();
        var employeeId = Guid.NewGuid();
        Guid enrollmentId;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CompensationDbContext>();
            await context.Database.EnsureCreatedAsync();

            var benefit = new Benefit { Id = Guid.NewGuid(), Name = "Terminate Health", BenefitType = BenefitType.HealthInsurance };
            context.Set<Benefit>().Add(benefit);

            var enrollment = new BenefitsEnrollment
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                BenefitId = benefit.Id,
                EnrollmentDate = DateTime.UtcNow,
                Status = EnrollmentStatus.Active
            };
            context.Set<BenefitsEnrollment>().Add(enrollment);
            await context.SaveChangesAsync();
            enrollmentId = enrollment.Id;
        }

        // Act
        var response = await client.DeleteAsync($"/compensation/v1/employees/{employeeId}/benefits/enrollments/{enrollmentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
