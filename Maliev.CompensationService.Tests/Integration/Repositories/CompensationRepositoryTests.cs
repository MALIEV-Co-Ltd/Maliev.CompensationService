using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Domain.Enums;
using Maliev.CompensationService.Infrastructure.Data;
using Maliev.CompensationService.Infrastructure.Repositories;
using Maliev.CompensationService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Maliev.CompensationService.Tests.Integration.Repositories;

[Collection("Testcontainers")]
public class CompensationRepositoryTests
{
    private readonly TestcontainersFixture _fixture;
    private readonly IEncryptionService _encryptionService;

    public CompensationRepositoryTests(TestcontainersFixture fixture)
    {
        _fixture = fixture;

        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Encryption:Key"]).Returns("MDEyMzQ1Njc4OWFiY2RlZmdoaWprbG1ub3BxcnN0dXY=");
        _encryptionService = new EncryptionService(configMock.Object);
    }

    private CompensationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CompensationDbContext>()
            .UseNpgsql(_fixture.PostgreSqlContainer.GetConnectionString())
            .Options;

        return new CompensationDbContext(options, _encryptionService);
    }

    [Fact]
    public async Task AddAsync_ShouldStoreSalaryEncryptedInDatabase()
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

        // Assert - Check raw value in DB
        using (var conn = new Npgsql.NpgsqlConnection(_fixture.PostgreSqlContainer.GetConnectionString()))
        {
            await conn.OpenAsync();
            using (var cmd = new Npgsql.NpgsqlCommand($"SELECT base_salary FROM compensation_records WHERE employee_id = '{employeeId}'", conn))
            {
                var rawValue = await cmd.ExecuteScalarAsync();
                Assert.NotNull(rawValue);

                // Should NOT be equal to plain text salary
                var rawString = rawValue.ToString();
                Assert.NotEqual("75000", rawString);

                // Should be decryptable back to 75000
                var decrypted = _encryptionService.Decrypt(rawString);
                Assert.Equal("75000", decrypted);
            }
        }
    }

    [Fact]
    public async Task GetByEmployeeIdAsync_ShouldRetrieveSalaryDecrypted()
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
