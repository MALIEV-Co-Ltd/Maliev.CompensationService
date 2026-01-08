namespace Maliev.CompensationService.Application.Commands;

/// <summary>
/// Command to undo the archival of a compensation record.
/// </summary>
public record UndoArchiveCompensationCommand
{
    /// <summary>
    /// Gets the unique identifier of the employee.
    /// </summary>
    public Guid EmployeeId { get; init; }
}