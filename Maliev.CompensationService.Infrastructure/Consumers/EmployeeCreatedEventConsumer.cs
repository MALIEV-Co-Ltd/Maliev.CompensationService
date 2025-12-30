using Maliev.EmployeeService.Domain.IntegrationEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Maliev.CompensationService.Infrastructure.Consumers;

/// <summary>
/// Consumer for <see cref="EmployeeCreatedIntegrationEvent"/>
/// </summary>
public class EmployeeCreatedEventConsumer : IConsumer<EmployeeCreatedIntegrationEvent>
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
    public Task Consume(ConsumeContext<EmployeeCreatedIntegrationEvent> context)
    {
        var @event = context.Message;
        _logger.LogInformation("New employee created: {EmployeeId} ({EmployeeNumber}). Preparing compensation setup.", 
            @event.EmployeeId, @event.EmployeeNumber);
        
        return Task.CompletedTask;
    }
}