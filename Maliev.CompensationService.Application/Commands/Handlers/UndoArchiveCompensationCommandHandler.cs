using Maliev.CompensationService.Application.Commands;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Application.Common.Mediator;
using Microsoft.Extensions.Logging;

namespace Maliev.CompensationService.Application.Commands.Handlers;

/// <summary>
/// Handler for UndoArchiveCompensationCommand (Compensating Transaction).
/// </summary>
public class UndoArchiveCompensationCommandHandler
{
    private readonly ICompensationRepository _compRepository;
    private readonly ILogger<UndoArchiveCompensationCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UndoArchiveCompensationCommandHandler"/> class.
    /// </summary>
    /// <param name="compRepository">The compensation repository.</param>
    /// <param name="logger">The logger.</param>
    public UndoArchiveCompensationCommandHandler(
        ICompensationRepository compRepository,
        ILogger<UndoArchiveCompensationCommandHandler> logger)
    {
        _compRepository = compRepository;
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

        var current = await _compRepository.GetByEmployeeIdAsync(command.EmployeeId, cancellationToken);
        if (current != null)
        {
            _logger.LogInformation("UNDO: A current record already exists for employee {EmployeeId}. No action taken.", command.EmployeeId);
            return;
        }

        var mostRecent = await _compRepository.GetMostRecentRecordAsync(command.EmployeeId, cancellationToken);
        if (mostRecent != null)
        {
            mostRecent.IsCurrent = true;
            await _compRepository.UpdateAsync(mostRecent, cancellationToken);
            _logger.LogInformation("UNDO: Successfully restored compensation status for employee {EmployeeId} (Record ID: {RecordId})", command.EmployeeId, mostRecent.Id);
        }
        else
        {
            _logger.LogWarning("UNDO: No compensation records found for employee {EmployeeId} to restore.", command.EmployeeId);
        }
    }
}
