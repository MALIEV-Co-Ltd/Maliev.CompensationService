using MediatR;

namespace Maliev.CompensationService.Application.Commands;

/// <summary>
/// Command to terminate an employee's benefit enrollment
/// </summary>
/// <param name="EmployeeId">Unique identifier of the employee</param>
/// <param name="EnrollmentId">Unique identifier of the enrollment</param>
/// <param name="TerminationDate">The date when the enrollment should end</param>
public record TerminateBenefitCommand(
    Guid EmployeeId,
    Guid EnrollmentId,
    DateTime TerminationDate) : IRequest;
