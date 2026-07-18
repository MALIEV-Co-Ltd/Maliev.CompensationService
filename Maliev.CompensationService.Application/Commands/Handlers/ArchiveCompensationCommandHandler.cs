using Maliev.CompensationService.Application.Common.Mediator;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.MessagingContracts;
using Maliev.MessagingContracts.Contracts.Compensation;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Maliev.CompensationService.Application.Commands.Handlers;

/// <summary>
/// Handler for ArchiveCompensationCommand (Saga step).
/// </summary>
public class ArchiveCompensationCommandHandler : IRequestHandler<ArchiveCompensationCommand, bool>
{
    private readonly ICompensationRepository _compRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<ArchiveCompensationCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArchiveCompensationCommandHandler"/> class.
    /// </summary>
    /// <param name="compRepository">The compensation repository.</param>
    /// <param name="publishEndpoint">The message bus publish endpoint.</param>
    /// <param name="logger">The logger.</param>
    public ArchiveCompensationCommandHandler(
        ICompensationRepository compRepository,
        IPublishEndpoint publishEndpoint,
        ILogger<ArchiveCompensationCommandHandler> logger)
    {
        _compRepository = compRepository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<bool> Handle(ArchiveCompensationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Archiving compensation for employee {EmployeeId} (Correlation: {CorrelationId})",
            request.EmployeeId, request.CorrelationId);

        var currentRecord = await _compRepository.GetByEmployeeIdAsync(request.EmployeeId, cancellationToken);
        if (currentRecord != null)
        {
            currentRecord.IsCurrent = false;
            currentRecord.ModifiedDate = DateTime.UtcNow;
            await _compRepository.UpdateAsync(currentRecord, cancellationToken);
        }

        await _publishEndpoint.Publish(new CompensationArchivedEvent(
            MessageId: Guid.NewGuid(),
            MessageName: nameof(CompensationArchivedEvent),
            MessageType: MessageType.Event,
            MessageVersion: "1.0.0",
            PublishedBy: "CompensationService",
            ConsumedBy: Array.Empty<string>(),
            CorrelationId: request.CorrelationId,
            CausationId: null,
            OccurredAtUtc: DateTimeOffset.UtcNow,
            IsPublic: false,
            Payload: new CompensationArchivedEventPayload(
                EmployeeId: request.EmployeeId,
                ArchivedAt: DateTimeOffset.UtcNow
            )
        ), cancellationToken);

        return true;
    }
}
