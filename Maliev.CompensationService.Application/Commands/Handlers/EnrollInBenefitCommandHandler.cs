using Maliev.CompensationService.Application.Common.Mediator;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Application.Mappers;
using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Domain.Enums;
using Maliev.MessagingContracts.Generated;
using Maliev.MessagingContracts.Contracts.Compensation;
using MassTransit;

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
        if (existing.Any(e => e.BenefitId == request.Data.BenefitId && (e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending)))
        {
            throw new InvalidOperationException("Employee is already active or pending in this benefit program.");
        }

        // Fetch benefit to check for waiting period
        var activeBenefits = await _repository.GetAllActiveAsync(cancellationToken);
        var benefit = activeBenefits.FirstOrDefault(b => b.Id == request.Data.BenefitId)
            ?? throw new InvalidOperationException("Benefit program not found or inactive.");

        var enrollmentId = Guid.NewGuid();
        var status = benefit.WaitingPeriodDays > 0 ? EnrollmentStatus.Pending : EnrollmentStatus.Active;
        var effectiveDate = request.Data.EnrollmentDate;

        if (benefit.WaitingPeriodDays > 0)
        {
            effectiveDate = effectiveDate.AddDays(benefit.WaitingPeriodDays);
        }

        var enrollment = new BenefitsEnrollment
        {
            Id = enrollmentId,
            EmployeeId = request.EmployeeId,
            BenefitId = request.Data.BenefitId,
            EnrollmentDate = effectiveDate,
            Status = status,
            EmployeeContribution = request.Data.EmployeeContribution,
            CoverageLevel = request.Data.CoverageLevel,
            CreatedDate = DateTime.UtcNow,
            Dependents = request.Data.Dependents.Select(d => d.ToEntity(enrollmentId)).ToList()
        };

        await _repository.AddEnrollmentAsync(enrollment, cancellationToken);

        // Publish event
        await _publishEndpoint.Publish(new BenefitEnrolledEvent(
            MessageId: Guid.NewGuid(),
            MessageName: nameof(BenefitEnrolledEvent),
            MessageType: MessageType.Event,
            MessageVersion: "1.0.0",
            PublishedBy: "CompensationService",
            ConsumedBy: Array.Empty<string>(),
            CorrelationId: Guid.NewGuid(),
            CausationId: null,
            OccurredAtUtc: DateTimeOffset.UtcNow,
            IsPublic: false,
            Payload: new BenefitEnrolledEventPayload(
                EmployeeId: enrollment.EmployeeId,
                BenefitId: enrollment.BenefitId,
                Status: enrollment.Status.ToString(),
                EffectiveDate: enrollment.EnrollmentDate
            )
        ), cancellationToken);

        // Fetch again to get related data (Benefit name)
        var result = await _repository.GetEnrollmentByIdAsync(enrollmentId, cancellationToken);
        return result!.ToDto();
    }
}
