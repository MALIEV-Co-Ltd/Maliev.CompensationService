using Maliev.CompensationService.Application.Commands.Handlers;
using Maliev.MessagingContracts.Contracts.Compensation;
using MassTransit;
using LocalUndoArchiveCompensationCommand = Maliev.CompensationService.Application.Commands.UndoArchiveCompensationCommand;

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
        await _handler.HandleAsync(
            new LocalUndoArchiveCompensationCommand
            {
                EmployeeId = context.Message.Payload.EmployeeId
            },
            context.CancellationToken);
    }
}
