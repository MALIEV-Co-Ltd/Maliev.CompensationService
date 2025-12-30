using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Domain.Entities;
using MediatR;

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
            throw new InvalidOperationException("Enrollment not found");
        }

        var dependent = new Dependent
        {
            Id = Guid.NewGuid(),
            BenefitsEnrollmentId = request.EnrollmentId,
            FirstName = request.Data.FirstName,
            LastName = request.Data.LastName,
            Relationship = request.Data.Relationship,
            DateOfBirth = request.Data.DateOfBirth,
            NationalId = request.Data.NationalId,
            CreatedDate = DateTime.UtcNow
        };

        enrollment.Dependents.Add(dependent);
        await _benefitsRepository.UpdateEnrollmentAsync(enrollment, cancellationToken);

        return new DependentDto
        {
            Id = dependent.Id,
            FirstName = dependent.FirstName,
            LastName = dependent.LastName,
            Relationship = dependent.Relationship,
            DateOfBirth = dependent.DateOfBirth,
            NationalId = dependent.NationalId
        };
    }
}
