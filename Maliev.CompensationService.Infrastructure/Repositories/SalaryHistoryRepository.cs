using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maliev.CompensationService.Infrastructure.Repositories;

/// <summary>
/// Repository for managing salary history entities.
/// </summary>
public class SalaryHistoryRepository : ISalaryHistoryRepository
{
    private readonly CompensationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="SalaryHistoryRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public SalaryHistoryRepository(CompensationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SalaryHistory>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<SalaryHistory>()
            .Where(h => h.EmployeeId == employeeId)
            .OrderByDescending(h => h.EffectiveDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task AddAsync(SalaryHistory history, CancellationToken cancellationToken = default)
    {
        await _context.Set<SalaryHistory>().AddAsync(history, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
