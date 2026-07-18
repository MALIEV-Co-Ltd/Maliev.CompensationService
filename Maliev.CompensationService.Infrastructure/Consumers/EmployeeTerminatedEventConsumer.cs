using Maliev.CompensationService.Application.Interfaces;
using Maliev.MessagingContracts.Contracts.Employee;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Maliev.CompensationService.Infrastructure.Consumers;

/// <summary>
/// Consumer for <see cref="EmployeeTerminatedEvent"/>
/// </summary>
public class EmployeeTerminatedEventConsumer : IConsumer<EmployeeTerminatedEvent>
{
    private readonly IBenefitsRepository _benefitsRepository;
    private readonly ILogger<EmployeeTerminatedEventConsumer> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeTerminatedEventConsumer"/> class
    /// </summary>
    /// <param name="benefitsRepository">The benefits repository</param>
    /// <param name="logger">The logger instance</param>
    public EmployeeTerminatedEventConsumer(IBenefitsRepository benefitsRepository, ILogger<EmployeeTerminatedEventConsumer> logger)
    {
        _benefitsRepository = benefitsRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task Consume(ConsumeContext<EmployeeTerminatedEvent> context)
    {
        var @event = context.Message;
        var payload = @event.Payload; // Access payload

        _logger.LogInformation("Employee terminated: {EmployeeId}. Terminating active benefits and rejecting pending ones.",
            payload.EmployeeId);

        await _benefitsRepository.TerminateActiveEnrollmentsAsync(payload.EmployeeId, payload.TerminationDate.UtcDateTime);
        await _benefitsRepository.RejectPendingEnrollmentsAsync(payload.EmployeeId);
    }
}
