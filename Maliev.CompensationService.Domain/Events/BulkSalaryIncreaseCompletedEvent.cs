namespace Maliev.CompensationService.Domain.Events;

/// <summary>
/// Event published when a bulk salary increase job is completed
/// </summary>
/// <param name="JobId">Unique identifier of the bulk job</param>
/// <param name="SuccessCount">Number of employees successfully processed</param>
/// <param name="FailureCount">Number of employees that failed processing</param>
/// <param name="TotalBudgetImpact">Total annual budget impact of all increases in this job</param>
public record BulkSalaryIncreaseCompletedEvent(
    Guid JobId,
    int SuccessCount,
    int FailureCount,
    decimal TotalBudgetImpact);
