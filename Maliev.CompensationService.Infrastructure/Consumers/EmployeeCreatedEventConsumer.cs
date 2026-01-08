using Maliev.MessagingContracts.Generated;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Maliev.CompensationService.Infrastructure.Consumers;

/// <summary>
/// Consumer for <see cref="EmployeeCreatedEvent"/>
/// </summary>
public class EmployeeCreatedEventConsumer : IConsumer<EmployeeCreatedEvent>
{
    private readonly ILogger<EmployeeCreatedEventConsumer> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeCreatedEventConsumer"/> class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    public EmployeeCreatedEventConsumer(ILogger<EmployeeCreatedEventConsumer> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task Consume(ConsumeContext<EmployeeCreatedEvent> context)
    {
        var @event = context.Message;
        var payload = @event.Payload; // Access payload

        _logger.LogInformation("New employee created: {EmployeeId} ({EmployeeNumber}). Preparing compensation setup.",
            payload.EmployeeId, payload.EmployeeNumber);

        return Task.CompletedTask;
    }
}