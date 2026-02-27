using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maliev.CompensationService.Infrastructure.Repositories;

/// <summary>
/// Repository for managing bulk job entities.
/// </summary>
public class BulkJobRepository : IBulkJobRepository
{
    private readonly CompensationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="BulkJobRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public BulkJobRepository(CompensationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<BulkJob?> GetByIdAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<BulkJob>()
            .FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task AddAsync(BulkJob job, CancellationToken cancellationToken = default)
    {
        await _context.Set<BulkJob>().AddAsync(job, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(BulkJob job, CancellationToken cancellationToken = default)
    {
        _context.Set<BulkJob>().Update(job);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
