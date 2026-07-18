using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Domain.Enums;
using Maliev.CompensationService.Infrastructure.Data;
using Maliev.CompensationService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Maliev.CompensationService.Tests.Integration.Repositories;

[Collection("Testcontainers")]
public class BulkJobRepositoryTests
{
    private readonly TestcontainersFixture _fixture;

    public BulkJobRepositoryTests(TestcontainersFixture fixture)
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
    public async Task GetByIdAsync_ReturnsJob_WhenExists()
    {
        var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        var repository = new BulkJobRepository(context);

        var jobId = Guid.NewGuid();
        var job = new BulkJob
        {
            Id = jobId,
            JobType = "SalaryIncrease",
            Status = BulkJobStatus.Pending,
            SuccessCount = 0,
            FailureCount = 0,
            StartedAt = DateTime.UtcNow,
            StartedBy = Guid.NewGuid()
        };

        context.Set<BulkJob>().Add(job);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var result = await repository.GetByIdAsync(jobId);

        Assert.NotNull(result);
        Assert.Equal("SalaryIncrease", result.JobType);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        var repository = new BulkJobRepository(context);

        var result = await repository.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_StoresJob()
    {
        var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        var repository = new BulkJobRepository(context);

        var job = new BulkJob
        {
            Id = Guid.NewGuid(),
            JobType = "SalaryIncrease",
            Status = BulkJobStatus.Pending,
            SuccessCount = 50,
            StartedAt = DateTime.UtcNow,
            StartedBy = Guid.NewGuid()
        };

        await repository.AddAsync(job);

        var stored = await context.Set<BulkJob>().FirstOrDefaultAsync(j => j.Id == job.Id);
        Assert.NotNull(stored);
    }

    [Fact]
    public async Task UpdateAsync_ModifiesJob()
    {
        var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        var repository = new BulkJobRepository(context);

        var job = new BulkJob
        {
            Id = Guid.NewGuid(),
            JobType = "SalaryIncrease",
            Status = BulkJobStatus.Pending,
            SuccessCount = 0,
            FailureCount = 0,
            StartedAt = DateTime.UtcNow,
            StartedBy = Guid.NewGuid()
        };

        context.Set<BulkJob>().Add(job);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        job.Status = BulkJobStatus.Completed;
        job.SuccessCount = 100;
        job.CompletedAt = DateTime.UtcNow;

        await repository.UpdateAsync(job);

        context.ChangeTracker.Clear();
        var updated = await context.Set<BulkJob>().FirstOrDefaultAsync(j => j.Id == job.Id);

        Assert.Equal(BulkJobStatus.Completed, updated!.Status);
        Assert.Equal(100, updated.SuccessCount);
    }
}
