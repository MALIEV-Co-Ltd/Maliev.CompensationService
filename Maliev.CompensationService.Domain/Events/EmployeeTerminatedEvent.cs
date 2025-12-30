namespace Maliev.CompensationService.Domain.Events;

/// <summary>
/// Event received when an employee is terminated in the Employee Service
/// </summary>
/// <param name="EmployeeId">Unique identifier of the employee</param>
/// <param name="TerminationDate">The date when the employee was terminated</param>
/// <param name="TerminationType">Optional type of termination</param>
/// <param name="FinalWorkingDay">Optional final working day</param>
/// <param name="Timestamp">When the event occurred</param>
public record EmployeeTerminatedEvent(
    Guid EmployeeId,
    DateTime TerminationDate,
    string? TerminationType,
    DateTime? FinalWorkingDay,
    DateTime Timestamp);
