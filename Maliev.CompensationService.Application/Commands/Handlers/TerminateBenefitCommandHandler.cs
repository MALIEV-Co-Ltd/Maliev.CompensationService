using Maliev.CompensationService.Application.Common.Mediator;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Domain.Enums;
using Maliev.MessagingContracts.Generated;
using Maliev.MessagingContracts.Contracts.Compensation;
using MassTransit;

namespace Maliev.CompensationService.Application.Commands.Handlers;

/// <summary>
/// Handler for the <see cref="TerminateBenefitCommand"/> command
/// </summary>
public class TerminateBenefitCommandHandler : IRequestHandler<TerminateBenefitCommand>
{
    private readonly IBenefitsRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminateBenefitCommandHandler"/> class
    /// </summary>
    /// <param name="repository">The benefits repository</param>
    /// <param name="publishEndpoint">The event publishing endpoint</param>
    public TerminateBenefitCommandHandler(IBenefitsRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    /// <inheritdoc />
    public async Task<Unit> Handle(TerminateBenefitCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _repository.GetEnrollmentByIdAsync(request.EnrollmentId, cancellationToken);
        if (enrollment == null || enrollment.EmployeeId != request.EmployeeId)
        {
            throw new KeyNotFoundException("Enrollment not found.");
        }

        enrollment.Status = EnrollmentStatus.Terminated;
        enrollment.TerminationDate = request.TerminationDate;
        enrollment.ModifiedDate = DateTime.UtcNow;

        await _repository.UpdateEnrollmentAsync(enrollment, cancellationToken);

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
                EffectiveDate: request.TerminationDate
            )
        ), cancellationToken);

        return Unit.Value;
    }
}
