using Maliev.CompensationService.Application.Interfaces;
using Maliev.EmployeeService.Domain.IntegrationEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Maliev.CompensationService.Infrastructure.Consumers;

/// <summary>
/// Consumer for <see cref="EmployeeTerminatedIntegrationEvent"/>
/// </summary>
public class EmployeeTerminatedEventConsumer : IConsumer<EmployeeTerminatedIntegrationEvent>
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
    public async Task Consume(ConsumeContext<EmployeeTerminatedIntegrationEvent> context)
    {
        var @event = context.Message;
        _logger.LogInformation("Employee terminated: {EmployeeId}. Terminating active benefits and rejecting pending ones.",
            @event.EmployeeId);

        await _benefitsRepository.TerminateActiveEnrollmentsAsync(@event.EmployeeId, @event.TerminationDate);
        await _benefitsRepository.RejectPendingEnrollmentsAsync(@event.EmployeeId);
    }
}