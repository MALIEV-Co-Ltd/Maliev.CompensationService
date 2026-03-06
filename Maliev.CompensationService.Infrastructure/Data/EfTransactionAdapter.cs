using Maliev.CompensationService.Application.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Maliev.CompensationService.Infrastructure.Data;

/// <summary>
/// Adapter that wraps an EF Core <see cref="IDbContextTransaction"/> to provide a transaction abstraction
/// for the Application layer, decoupling it from Infrastructure concerns.
/// </summary>
public class EfTransactionAdapter : ITransaction
{
    private readonly IDbContextTransaction _transaction;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="EfTransactionAdapter"/> class.
    /// </summary>
    /// <param name="transaction">The EF Core database transaction to wrap.</param>
    public EfTransactionAdapter(IDbContextTransaction transaction)
    {
        _transaction = transaction;
        TransactionId = transaction.TransactionId;
    }

    /// <inheritdoc />
    public Guid TransactionId { get; }

    /// <inheritdoc />
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.CommitAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.RollbackAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            _transaction.Dispose();
            _disposed = true;
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            await _transaction.DisposeAsync();
            _disposed = true;
        }
    }
}
