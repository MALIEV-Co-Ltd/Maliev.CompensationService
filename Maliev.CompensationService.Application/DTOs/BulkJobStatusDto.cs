using Maliev.CompensationService.Domain.Enums;

namespace Maliev.CompensationService.Application.DTOs;

/// <summary>
/// Data transfer object for bulk job status
/// </summary>
public class BulkJobStatusDto
{
    /// <summary>
    /// Unique identifier of the job
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Type of bulk operation
    /// </summary>
    public string JobType { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the job
    /// </summary>
    public BulkJobStatus Status { get; set; }

    /// <summary>
    /// Number of successfully processed records
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of failed records
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// When the job was started
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// When the job completed, if applicable
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Detailed error information if failures occurred
    /// </summary>
    public string? ErrorSummary { get; set; }
}
