using Maliev.CompensationService.Application.Common.Mediator;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;

namespace Maliev.CompensationService.Application.Queries.Handlers;

/// <summary>
/// Handler for the <see cref="GetBulkJobStatusQuery"/> query
/// </summary>
public class GetBulkJobStatusQueryHandler : IRequestHandler<GetBulkJobStatusQuery, BulkJobStatusDto?>
{
    private readonly IBulkJobRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetBulkJobStatusQueryHandler"/> class
    /// </summary>
    /// <param name="repository">The bulk job repository</param>
    public GetBulkJobStatusQueryHandler(IBulkJobRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<BulkJobStatusDto?> Handle(GetBulkJobStatusQuery request, CancellationToken cancellationToken)
    {
        var job = await _repository.GetByIdAsync(request.JobId, cancellationToken);
        if (job == null) return null;

        return new BulkJobStatusDto
        {
            Id = job.Id,
            JobType = job.JobType,
            Status = job.Status,
            SuccessCount = job.SuccessCount,
            FailureCount = job.FailureCount,
            StartedAt = job.StartedAt,
            CompletedAt = job.CompletedAt,
            ErrorSummary = job.ErrorDetails
        };
    }
}
