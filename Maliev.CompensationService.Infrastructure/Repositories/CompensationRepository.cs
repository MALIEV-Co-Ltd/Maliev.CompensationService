using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maliev.CompensationService.Infrastructure.Repositories;

/// <summary>
/// Repository for managing compensation record entities.
/// </summary>
public class CompensationRepository : ICompensationRepository
{
    private readonly CompensationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompensationRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public CompensationRepository(CompensationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<CompensationRecord?> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<CompensationRecord>()
            .FirstOrDefaultAsync(r => r.EmployeeId == employeeId && r.IsCurrent, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CompensationRecord>> GetAllCurrentAsync(Guid? departmentId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<CompensationRecord>()
            .Where(r => r.IsCurrent);

        if (departmentId.HasValue)
        {
            query = query.Where(r => r.DepartmentId == departmentId.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<CompensationRecord?> GetMostRecentRecordAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<CompensationRecord>()
            .Where(r => r.EmployeeId == employeeId)
            .OrderByDescending(r => r.CreatedDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task AddAsync(CompensationRecord record, CancellationToken cancellationToken = default)
    {
        await _context.Set<CompensationRecord>().AddAsync(record, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(CompensationRecord record, CancellationToken cancellationToken = default)
    {
        _context.Set<CompensationRecord>().Update(record);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async ct =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                var result = await action(ct);
                await transaction.CommitAsync(ct);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }, cancellationToken);
    }
}
