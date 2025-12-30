using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Domain.Enums;
using Maliev.CompensationService.Infrastructure.Data;
using Maliev.CompensationService.Infrastructure.Repositories;
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
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Encryption:Key"] = "MDEyMzQ1Njc4OWFiY2RlZmdoaWprbG1ub3BxcnN0dXY="
            })
            .Build();
        
        // I need a real encryption service to test transparency
        _encryptionService = new Infrastructure.Services.EncryptionService(configuration);
    }

    private CompensationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CompensationDbContext>()
            .UseNpgsql(_fixture.PostgreSqlContainer.GetConnectionString())
            .Options;
        
        return new CompensationDbContext(options, _encryptionService);
    }

    [Fact]
    public async Task AddAsync_ShouldEncryptSalaryInDatabase()
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
            using (var cmd = new Npgsql.NpgsqlCommand($"SELECT base_salary_encrypted FROM compensation_records WHERE employee_id = '{employeeId}'", conn))
            {
                var rawValue = await cmd.ExecuteScalarAsync() as string;
                Assert.NotNull(rawValue);
                Assert.NotEqual("75000.00", rawValue);
                Assert.NotEqual("75000", rawValue);
                
                // Decrypt manually to verify
                var decrypted = _encryptionService.Decrypt(rawValue);
                Assert.Equal("75000.00", decrypted);
            }
        }
    }

    [Fact]
    public async Task GetByEmployeeIdAsync_ShouldDecryptSalary()
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
