using Maliev.CompensationService.Application.Common.Mediator;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Application.Mappers;
using Maliev.CompensationService.Domain.Entities;

namespace Maliev.CompensationService.Application.Commands.Handlers;

/// <summary>
/// Handler for AddDependentCommand.
/// </summary>
public class AddDependentCommandHandler : IRequestHandler<AddDependentCommand, DependentDto>
{
    private readonly IBenefitsRepository _benefitsRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddDependentCommandHandler"/> class.
    /// </summary>
    /// <param name="benefitsRepository">The benefits repository.</param>
    public AddDependentCommandHandler(IBenefitsRepository benefitsRepository)
    {
        _benefitsRepository = benefitsRepository;
    }

    /// <inheritdoc/>
    public async Task<DependentDto> Handle(AddDependentCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _benefitsRepository.GetEnrollmentByIdAsync(request.EnrollmentId, cancellationToken);
        if (enrollment == null)
        {
            throw new InvalidOperationException($"Enrollment with ID {request.EnrollmentId} not found");
        }

        var dependent = request.Data.ToEntity(request.EnrollmentId);

        await _benefitsRepository.AddDependentAsync(dependent, cancellationToken);

        return dependent.ToDto();
    }
}

