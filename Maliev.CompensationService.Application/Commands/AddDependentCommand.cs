using Maliev.CompensationService.Application.Common.Mediator;
using Maliev.CompensationService.Application.DTOs;

namespace Maliev.CompensationService.Application.Commands;

/// <summary>
/// Command to add a dependent to a benefit enrollment.
/// </summary>
/// <param name="EnrollmentId">The unique identifier of the benefit enrollment.</param>
/// <param name="Data">The dependent information.</param>
public record AddDependentCommand(
    Guid EnrollmentId,
    DependentDto Data) : IRequest<DependentDto>;
