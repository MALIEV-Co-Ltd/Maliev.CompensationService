using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Application.Mappers;
using MediatR;

namespace Maliev.CompensationService.Application.Commands.Handlers;

/// <summary>
/// Handler for UpdateBenefitsEnrollmentCommand.
/// </summary>
public class UpdateBenefitsEnrollmentCommandHandler : IRequestHandler<UpdateBenefitsEnrollmentCommand, BenefitsEnrollmentDto>
{
    private readonly IBenefitsRepository _benefitsRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateBenefitsEnrollmentCommandHandler"/> class.
    /// </summary>
    /// <param name="benefitsRepository">The benefits repository.</param>
    public UpdateBenefitsEnrollmentCommandHandler(IBenefitsRepository benefitsRepository)
    {
        _benefitsRepository = benefitsRepository;
    }

    /// <inheritdoc/>
    public async Task<BenefitsEnrollmentDto> Handle(UpdateBenefitsEnrollmentCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _benefitsRepository.GetEnrollmentByIdAsync(request.EnrollmentId, cancellationToken);
        if (enrollment == null)
        {
            throw new InvalidOperationException("Enrollment not found");
        }

        enrollment.EmployeeContribution = request.Data.EmployeeContribution;
        enrollment.CoverageLevel = request.Data.CoverageLevel;
        enrollment.ModifiedDate = DateTime.UtcNow;

        await _benefitsRepository.UpdateEnrollmentAsync(enrollment, cancellationToken);

        // Fetch the updated enrollment with all related data
        var updatedEnrollment = await _benefitsRepository.GetEnrollmentByIdAsync(request.EnrollmentId, cancellationToken);
        return updatedEnrollment!.ToDto();
    }
}
