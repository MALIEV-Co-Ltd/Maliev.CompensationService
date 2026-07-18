using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Application.Queries;
using Maliev.CompensationService.Application.Queries.Handlers;
using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Domain.Enums;
using Moq;
using Xunit;

namespace Maliev.CompensationService.Tests.Unit.Queries;

public class GetBulkJobStatusQueryHandlerTests
{
    private readonly Mock<IBulkJobRepository> _bulkJobRepoMock;
    private readonly GetBulkJobStatusQueryHandler _handler;

    public GetBulkJobStatusQueryHandlerTests()
    {
        _bulkJobRepoMock = new Mock<IBulkJobRepository>();
        _handler = new GetBulkJobStatusQueryHandler(_bulkJobRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnJobStatus_WhenJobExists()
    {
        var jobId = Guid.NewGuid();
        var job = new BulkJob
        {
            Id = jobId,
            JobType = "SalaryIncrease",
            Status = BulkJobStatus.Completed,
            SuccessCount = 10,
            FailureCount = 0,
            StartedAt = DateTime.UtcNow.AddHours(-1),
            CompletedAt = DateTime.UtcNow
        };

        _bulkJobRepoMock.Setup(r => r.GetByIdAsync(jobId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(job);

        var query = new GetBulkJobStatusQuery(jobId);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(jobId, result.Id);
        Assert.Equal(BulkJobStatus.Completed, result.Status);
        Assert.Equal(10, result.SuccessCount);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenJobNotFound()
    {
        var jobId = Guid.NewGuid();

        _bulkJobRepoMock.Setup(r => r.GetByIdAsync(jobId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BulkJob?)null);

        var query = new GetBulkJobStatusQuery(jobId);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnErrorSummary_WhenJobHasErrors()
    {
        var jobId = Guid.NewGuid();
        var errorDetails = "[{\"EmployeeId\":\"abc\",\"Error\":\"Test error\"}]";

        var job = new BulkJob
        {
            Id = jobId,
            JobType = "SalaryIncrease",
            Status = BulkJobStatus.PartiallyCompleted,
            SuccessCount = 8,
            FailureCount = 2,
            StartedAt = DateTime.UtcNow.AddHours(-1),
            CompletedAt = DateTime.UtcNow,
            ErrorDetails = errorDetails
        };

        _bulkJobRepoMock.Setup(r => r.GetByIdAsync(jobId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(job);

        var query = new GetBulkJobStatusQuery(jobId);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(errorDetails, result.ErrorSummary);
    }
}
