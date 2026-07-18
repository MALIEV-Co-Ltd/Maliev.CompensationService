using Maliev.CompensationService.Domain.Entities;

namespace Maliev.CompensationService.Application.Interfaces;

/// <summary>
/// Repository for managing asynchronous bulk operation jobs
/// </summary>
public interface IBulkJobRepository
{
    /// <summary>
    /// Gets a bulk job by its identifier
    /// </summary>
    /// <param name="jobId">Unique identifier of the job</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The job record, or null if not found</returns>
    Task<BulkJob?> GetByIdAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new bulk job
    /// </summary>
    /// <param name="job">The job to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddAsync(BulkJob job, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing bulk job
    /// </summary>
    /// <param name="job">The job to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateAsync(BulkJob job, CancellationToken cancellationToken = default);
}
