using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Domain.Enums;
using Maliev.CompensationService.Infrastructure.Data;
using Maliev.CompensationService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Maliev.CompensationService.Tests.Integration.Repositories;

[Collection("Testcontainers")]
public class BenefitsRepositoryTests
{
    private readonly TestcontainersFixture _fixture;

    public BenefitsRepositoryTests(TestcontainersFixture fixture)
    {
        _fixture = fixture;
    }

    private CompensationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CompensationDbContext>()
            .UseNpgsql(_fixture.PostgreSqlContainer.GetConnectionString())
            .Options;

        return new CompensationDbContext(options);
    }

    [Fact]
    public async Task GetAllActiveAsync_ReturnsOnlyActiveBenefits()
    {
        var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        var repository = new BenefitsRepository(context);

        var result = await repository.GetAllActiveAsync();

        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetEnrollmentsByEmployeeIdAsync_ReturnsEmpty_WhenNoEnrollments()
    {
        var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        var repository = new BenefitsRepository(context);

        var result = await repository.GetEnrollmentsByEmployeeIdAsync(Guid.NewGuid());

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetEnrollmentByIdAsync_ReturnsNull_WhenNotExists()
    {
        var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        var repository = new BenefitsRepository(context);

        var result = await repository.GetEnrollmentByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }
}
