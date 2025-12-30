using Maliev.CompensationService.Application.DTOs;
using MediatR;

namespace Maliev.CompensationService.Application.Commands;

/// <summary>
/// Command to update an existing benefit enrollment.
/// </summary>
/// <param name="EnrollmentId">The unique identifier of the enrollment to update.</param>
/// <param name="Data">The updated enrollment details.</param>
public record UpdateBenefitsEnrollmentCommand(
    Guid EnrollmentId,
    UpdateBenefitsEnrollmentDto Data) : IRequest<BenefitsEnrollmentDto>;