using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Common.Mediator;

namespace Maliev.CompensationService.Application.Commands;

/// <summary>
/// Command to enroll an employee in a benefit program.
/// </summary>
/// <param name="EmployeeId">The unique identifier of the employee.</param>
/// <param name="BenefitId">The unique identifier of the benefit program.</param>
/// <param name="Data">The enrollment details.</param>
public record EnrollInBenefitsCommand(
    Guid EmployeeId,
    Guid BenefitId,
    EnrollInBenefitDto Data) : IRequest<BenefitsEnrollmentDto>;
