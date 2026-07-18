namespace Maliev.CompensationService.Domain.Enums;

/// <summary>
/// Represents the processing status of a bulk job
/// </summary>
public enum BulkJobStatus
{
    /// <summary>
    /// Job is queued but not yet started
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Job is currently being processed
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Job completed successfully
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Job failed with errors
    /// </summary>
    Failed = 3,

    /// <summary>
    /// Job was partially completed
    /// </summary>
    PartiallyCompleted = 4
}
