using Maliev.CompensationService.Application.Common.Mediator;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Domain.Entities;


namespace Maliev.CompensationService.Application.Commands.Handlers;

/// <summary>
/// Handler for EnrollInBenefitsCommand.
/// </summary>
public class EnrollInBenefitsCommandHandler : IRequestHandler<EnrollInBenefitsCommand, BenefitsEnrollmentDto>
{
    private readonly IBenefitsRepository _benefitsRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnrollInBenefitsCommandHandler"/> class.
    /// </summary>
    /// <param name="benefitsRepository">The benefits repository.</param>
    public EnrollInBenefitsCommandHandler(IBenefitsRepository benefitsRepository)
    {
        _benefitsRepository = benefitsRepository;
    }

    /// <inheritdoc/>
    public async Task<BenefitsEnrollmentDto> Handle(EnrollInBenefitsCommand request, CancellationToken cancellationToken)
    {
        var enrollment = new BenefitsEnrollment
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            BenefitId = request.BenefitId,
            EnrollmentDate = request.Data.EnrollmentDate,
            EmployeeContribution = request.Data.EmployeeContribution,
            CoverageLevel = request.Data.CoverageLevel,
            Status = Domain.Enums.EnrollmentStatus.Pending,
            CreatedDate = DateTime.UtcNow,
            Dependents = request.Data.Dependents.Select(d => new Dependent
            {
                Id = Guid.NewGuid(),
                FirstName = d.FirstName,
                LastName = d.LastName,
                Relationship = d.Relationship,
                DateOfBirth = d.DateOfBirth,
                NationalId = d.NationalId,
                CreatedDate = DateTime.UtcNow
            }).ToList()
        };

        await _benefitsRepository.AddEnrollmentAsync(enrollment, cancellationToken);

        return new BenefitsEnrollmentDto
        {
            Id = enrollment.Id,
            EmployeeId = enrollment.EmployeeId,
            BenefitId = enrollment.BenefitId,
            EnrollmentDate = enrollment.EnrollmentDate,
            EmployeeContribution = enrollment.EmployeeContribution,
            CoverageLevel = enrollment.CoverageLevel,
            Status = enrollment.Status,
            Dependents = enrollment.Dependents.Select(d => new DependentDto
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Relationship = d.Relationship,
                DateOfBirth = d.DateOfBirth,
                NationalId = d.NationalId
            }).ToList()
        };
    }
}
