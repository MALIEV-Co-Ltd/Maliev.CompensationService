using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Infrastructure.Data;
using Maliev.CompensationService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Maliev.CompensationService.Tests.Integration.Repositories;

[Collection("Testcontainers")]
public class SalaryHistoryRepositoryTests
{
    private readonly TestcontainersFixture _fixture;

    public SalaryHistoryRepositoryTests(TestcontainersFixture fixture)
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
    public async Task GetByEmployeeIdAsync_ReturnsEmpty_WhenNoHistory()
    {
        var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        var repository = new SalaryHistoryRepository(context);

        var result = await repository.GetByEmployeeIdAsync(Guid.NewGuid());

        Assert.Empty(result);
    }
}
