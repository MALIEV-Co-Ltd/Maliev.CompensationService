using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Common.Mediator;

namespace Maliev.CompensationService.Application.Queries;

/// <summary>
/// Query to get compensation history for an employee.
/// </summary>
/// <param name="EmployeeId">The employee identifier.</param>
public record GetCompensationHistoryQuery(Guid EmployeeId) : IRequest<IEnumerable<SalaryHistoryDto>>;
