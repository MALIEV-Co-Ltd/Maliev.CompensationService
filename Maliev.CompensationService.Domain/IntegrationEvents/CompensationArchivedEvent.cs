namespace Maliev.CompensationService.Domain.IntegrationEvents;

/// <summary>
/// Event published when an employee's compensation records have been successfully archived.
/// </summary>
public record CompensationArchivedEvent(Guid EmployeeId, Guid CorrelationId);
