using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Domain.Commands;
using Maliev.CompensationService.Domain.IntegrationEvents;
using MassTransit;
using MediatR;
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

        await _publishEndpoint.Publish(new CompensationArchivedEvent(request.EmployeeId, request.CorrelationId), cancellationToken);

        return true;
    }
}