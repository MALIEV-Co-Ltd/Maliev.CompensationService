using Maliev.CompensationService.Application.DTOs;
using MediatR;

namespace Maliev.CompensationService.Application.Queries;

/// <summary>
/// Query to get compensation details for an employee.
/// </summary>
/// <param name="EmployeeId">The employee identifier.</param>
public record GetCompensationDetailsQuery(Guid EmployeeId) : IRequest<CompensationRecordDto?>;
