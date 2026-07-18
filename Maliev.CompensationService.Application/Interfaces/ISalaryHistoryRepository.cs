using Maliev.CompensationService.Domain.Entities;

namespace Maliev.CompensationService.Application.Interfaces;

/// <summary>
/// Repository for managing salary history records
/// </summary>
public interface ISalaryHistoryRepository
{
    /// <summary>
    /// Gets all salary history records for a specific employee, ordered by date descending
    /// </summary>
    /// <param name="employeeId">Unique identifier of the employee</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of salary history records</returns>
    Task<IEnumerable<SalaryHistory>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new salary history record
    /// </summary>
    /// <param name="history">The history record to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddAsync(SalaryHistory history, CancellationToken cancellationToken = default);
}
