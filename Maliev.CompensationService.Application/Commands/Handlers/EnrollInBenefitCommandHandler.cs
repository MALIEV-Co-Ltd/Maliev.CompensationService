using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Application.Mappers;
using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Domain.Enums;
using Maliev.CompensationService.Domain.Events;
using MassTransit;
using Maliev.CompensationService.Application.Common.Mediator;

namespace Maliev.CompensationService.Application.Commands.Handlers;

/// <summary>
/// Handler for the <see cref="EnrollInBenefitCommand"/> command
/// </summary>
public class EnrollInBenefitCommandHandler : IRequestHandler<EnrollInBenefitCommand, BenefitsEnrollmentDto>
{
    private readonly IBenefitsRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnrollInBenefitCommandHandler"/> class
    /// </summary>
    /// <param name="repository">The benefits repository</param>
    /// <param name="publishEndpoint">The event publishing endpoint</param>
    public EnrollInBenefitCommandHandler(IBenefitsRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    /// <inheritdoc />
    public async Task<BenefitsEnrollmentDto> Handle(EnrollInBenefitCommand request, CancellationToken cancellationToken)
    {
        // Check for existing active enrollment for this benefit
        var existing = await _repository.GetEnrollmentsByEmployeeIdAsync(request.EmployeeId, cancellationToken);
        if (existing.Any(e => e.BenefitId == request.Data.BenefitId && e.Status == EnrollmentStatus.Active))
        {
            throw new InvalidOperationException("Employee is already active in this benefit program.");
        }

        // We assume the benefit existence is validated by foreign key or a separate check if needed
        // For simplicity we'll just create the enrollment

        var enrollmentId = Guid.NewGuid();
        var enrollment = new BenefitsEnrollment
        {
            Id = enrollmentId,
            EmployeeId = request.EmployeeId,
            BenefitId = request.Data.BenefitId,
            EnrollmentDate = request.Data.EnrollmentDate,
            Status = EnrollmentStatus.Active, // For now we set to active directly, or Pending if we implement waiting period logic fully
            EmployeeContribution = request.Data.EmployeeContribution,
            CoverageLevel = request.Data.CoverageLevel,
            CreatedDate = DateTime.UtcNow,
            Dependents = request.Data.Dependents.Select(d => d.ToEntity(enrollmentId)).ToList()
        };

        await _repository.AddEnrollmentAsync(enrollment, cancellationToken);

        // Publish event
        await _publishEndpoint.Publish(new BenefitEnrolledEvent(
            enrollment.EmployeeId,
            enrollment.BenefitId,
            enrollment.Status.ToString(),
            enrollment.EnrollmentDate
        ), cancellationToken);

        // Fetch again to get related data (Benefit name)
        var result = await _repository.GetEnrollmentByIdAsync(enrollmentId, cancellationToken);
        return result!.ToDto();
    }
}
