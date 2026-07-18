using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Domain.Enums;
using Maliev.CompensationService.Infrastructure.Data;
using Maliev.CompensationService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Maliev.CompensationService.Tests.Integration.Repositories;

[Collection("Testcontainers")]
public class CompensationRepositoryTests
{
    private readonly TestcontainersFixture _fixture;

    public CompensationRepositoryTests(TestcontainersFixture fixture)
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
    public async Task AddAsync_ShouldStoreSalaryInDatabase()
    {
        // Arrange
        var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        var repository = new CompensationRepository(context);
        var employeeId = Guid.NewGuid();
        var record = new CompensationRecord
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            BaseSalary = 75000,
            IsCurrent = true,
            EffectiveDate = DateTime.UtcNow,
            CreatedDate = DateTime.UtcNow
        };

        // Act
        await repository.AddAsync(record);

        // Assert - Check value in DB
        using (var conn = new Npgsql.NpgsqlConnection(_fixture.PostgreSqlContainer.GetConnectionString()))
        {
            await conn.OpenAsync();
            using (var cmd = new Npgsql.NpgsqlCommand($"SELECT base_salary FROM compensation_records WHERE employee_id = '{employeeId}'", conn))
            {
                var value = await cmd.ExecuteScalarAsync();
                Assert.NotNull(value);

                // Should be plain text salary
                Assert.Equal(75000m, Convert.ToDecimal(value));
            }
        }
    }

    [Fact]
    public async Task GetByEmployeeIdAsync_ShouldRetrieveSalary()
    {
        // Arrange
        var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        var repository = new CompensationRepository(context);
        var employeeId = Guid.NewGuid();
        var record = new CompensationRecord
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            BaseSalary = 85000,
            IsCurrent = true,
            EffectiveDate = DateTime.UtcNow,
            CreatedDate = DateTime.UtcNow
        };
        await repository.AddAsync(record);

        // Clear context to force reload from DB
        context.ChangeTracker.Clear();

        // Act
        var retrieved = await repository.GetByEmployeeIdAsync(employeeId);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(85000, retrieved.BaseSalary);
    }
}
