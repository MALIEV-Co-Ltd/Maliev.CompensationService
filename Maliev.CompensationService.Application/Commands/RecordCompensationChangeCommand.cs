using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Common.Mediator;

namespace Maliev.CompensationService.Application.Commands;

/// <summary>
/// Command to record a change in an employee's compensation.
/// </summary>
/// <param name="EmployeeId">The unique identifier of the employee whose compensation is changing.</param>
/// <param name="Data">The details of the compensation change.</param>
/// <param name="ChangedBy">The unique identifier of the user who initiated the change.</param>
public record RecordCompensationChangeCommand(
    Guid EmployeeId,
    RecordCompensationChangeDto Data,
    Guid ChangedBy) : IRequest<SalaryHistoryDto>;