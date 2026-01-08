using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Common.Mediator;

namespace Maliev.CompensationService.Application.Queries;

/// <summary>
/// Query to retrieve all benefit enrollments for a specific employee
/// </summary>
/// <param name="EmployeeId">Unique identifier of the employee</param>
public record GetEmployeeBenefitsQuery(Guid EmployeeId) : IRequest<IEnumerable<BenefitsEnrollmentDto>>;
