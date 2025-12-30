using MediatR;

namespace Maliev.CompensationService.Domain.Commands;

/// <summary>
/// Saga command to archive an employee's compensation records.
/// </summary>
public record ArchiveCompensationCommand(Guid EmployeeId, Guid CorrelationId) : IRequest<bool>;