using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Maliev.CompensationService.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing benefits and enrollments
/// </summary>
public class BenefitsRepository : IBenefitsRepository
{
    private readonly CompensationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="BenefitsRepository"/> class
    /// </summary>
    /// <param name="context">The database context</param>
    public BenefitsRepository(CompensationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Benefit>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<Benefit>()
            .Where(b => b.IsActive)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<BenefitsEnrollment>> GetEnrollmentsByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<BenefitsEnrollment>()
            .Include(e => e.Benefit)
            .Include(e => e.Dependents)
            .Where(e => e.EmployeeId == employeeId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<BenefitsEnrollment?> GetEnrollmentByIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<BenefitsEnrollment>()
            .Include(e => e.Benefit)
            .Include(e => e.Dependents)
            .FirstOrDefaultAsync(e => e.Id == enrollmentId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddEnrollmentAsync(BenefitsEnrollment enrollment, CancellationToken cancellationToken = default)
    {
        await _context.Set<BenefitsEnrollment>().AddAsync(enrollment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateEnrollmentAsync(BenefitsEnrollment enrollment, CancellationToken cancellationToken = default)
    {
        if (_context.Entry(enrollment).State == EntityState.Detached)
        {
            _context.Set<BenefitsEnrollment>().Update(enrollment);
        }
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task TerminateActiveEnrollmentsAsync(Guid employeeId, DateTime terminationDate, CancellationToken cancellationToken = default)
    {
        var enrollments = await _context.Set<BenefitsEnrollment>()
            .Where(e => e.EmployeeId == employeeId && e.Status == Domain.Enums.EnrollmentStatus.Active)
            .ToListAsync(cancellationToken);

        foreach (var enrollment in enrollments)
        {
            enrollment.Status = Domain.Enums.EnrollmentStatus.Terminated;
            enrollment.TerminationDate = terminationDate;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task RejectPendingEnrollmentsAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var enrollments = await _context.Set<BenefitsEnrollment>()
            .Where(e => e.EmployeeId == employeeId && e.Status == Domain.Enums.EnrollmentStatus.Pending)
            .ToListAsync(cancellationToken);

        foreach (var enrollment in enrollments)
        {
            enrollment.Status = Domain.Enums.EnrollmentStatus.Terminated; // Or some 'Rejected' status if added
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}