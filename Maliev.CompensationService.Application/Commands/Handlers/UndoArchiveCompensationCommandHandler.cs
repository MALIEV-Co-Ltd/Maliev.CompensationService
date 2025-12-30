using Maliev.CompensationService.Domain.Commands;
using Microsoft.Extensions.Logging;

namespace Maliev.CompensationService.Application.Commands.Handlers;

/// <summary>
/// Handler for UndoArchiveCompensationCommand (Compensating Transaction).
/// </summary>
public class UndoArchiveCompensationCommandHandler
{
    private readonly ILogger<UndoArchiveCompensationCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UndoArchiveCompensationCommandHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public UndoArchiveCompensationCommandHandler(ILogger<UndoArchiveCompensationCommandHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the undo archive compensation command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task HandleAsync(UndoArchiveCompensationCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("UNDO: Restoring compensation status for employee {EmployeeId}", command.EmployeeId);
        
        // Logic to restore record
        
        await Task.CompletedTask;
    }
}