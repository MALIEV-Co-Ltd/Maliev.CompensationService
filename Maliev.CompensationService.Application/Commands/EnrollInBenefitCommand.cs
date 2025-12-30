using Maliev.CompensationService.Application.DTOs;
using MediatR;

namespace Maliev.CompensationService.Application.Commands;

/// <summary>
/// Command to enroll an employee in a benefit program
/// </summary>
/// <param name="EmployeeId">Unique identifier of the employee</param>
/// <param name="Data">Enrollment details</param>
/// <param name="CreatedBy">Identifier of the user who created the enrollment</param>
public record EnrollInBenefitCommand(
    Guid EmployeeId,
    EnrollInBenefitDto Data,
    Guid CreatedBy) : IRequest<BenefitsEnrollmentDto>;
