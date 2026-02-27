using Maliev.CompensationService.Domain.Enums;

namespace Maliev.CompensationService.Domain.Entities;

/// <summary>
/// Tracks asynchronous bulk operations such as salary increases for multiple employees
/// </summary>
public class BulkJob
{
    /// <summary>
    /// Unique identifier for the bulk job
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The type of operation being performed (e.g., SalaryIncrease)
    /// </summary>
    public string JobType { get; set; } = string.Empty;

    /// <summary>
    /// Current processing status of the job
    /// </summary>
    public BulkJobStatus Status { get; set; }

    /// <summary>
    /// JSON string containing the parameters for the job
    /// </summary>
    public string Parameters { get; set; } = "{}";

    /// <summary>
    /// Number of successfully processed records
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of records that failed processing
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// JSON string containing detailed error information for failed records
    /// </summary>
    public string? ErrorDetails { get; set; }

    /// <summary>
    /// The identifier of the user who started the job
    /// </summary>
    public Guid StartedBy { get; set; }

    /// <summary>
    /// The date and time when the job was started
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// The date and time when the job completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Optional URL to notify via POST request when the job completes
    /// </summary>
    public string? WebhookUrl { get; set; }
}
