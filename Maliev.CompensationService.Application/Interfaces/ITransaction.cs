namespace Maliev.CompensationService.Application.Interfaces;

/// <summary>
/// Represents a database transaction abstraction.
/// </summary>
public interface ITransaction : IDisposable
{
    /// <summary>
    /// Commits the transaction.
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the transaction.
    /// </summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the unique identifier of the transaction.
    /// </summary>
    Guid TransactionId { get; }
}
