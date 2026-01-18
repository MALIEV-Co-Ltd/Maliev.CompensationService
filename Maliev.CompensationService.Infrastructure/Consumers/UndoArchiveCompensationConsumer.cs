using Maliev.CompensationService.Application.Commands;
using Maliev.CompensationService.Application.Commands.Handlers;
using MassTransit;

namespace Maliev.CompensationService.Infrastructure.Consumers;

/// <summary>
/// Consumes <see cref="UndoArchiveCompensationCommand"/> to revert compensation record status.
/// </summary>
public class UndoArchiveCompensationConsumer : IConsumer<UndoArchiveCompensationCommand>
{
    private readonly UndoArchiveCompensationCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="UndoArchiveCompensationConsumer"/> class.
    /// </summary>
    /// <param name="handler">The command handler.</param>
    public UndoArchiveCompensationConsumer(UndoArchiveCompensationCommandHandler handler)
    {
        _handler = handler;
    }

    /// <inheritdoc/>
    public async Task Consume(ConsumeContext<UndoArchiveCompensationCommand> context)
    {
        await _handler.HandleAsync(context.Message, context.CancellationToken);
    }
}