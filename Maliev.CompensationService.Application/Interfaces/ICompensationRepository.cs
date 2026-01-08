using Maliev.CompensationService.Domain.Entities;

namespace Maliev.CompensationService.Application.Interfaces;

/// <summary>
/// Repository for managing employee compensation records
/// </summary>
public interface ICompensationRepository
{
    /// <summary>
    /// Gets the current active compensation record for an employee
    /// </summary>
    /// <param name="employeeId">Unique identifier of the employee</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The current compensation record, or null if not found</returns>
    Task<CompensationRecord?> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all current active compensation records, optionally filtered by department
    /// </summary>
    /// <param name="departmentId">Optional department identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of current compensation records</returns>
    Task<IEnumerable<CompensationRecord>> GetAllCurrentAsync(Guid? departmentId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the most recent compensation record for an employee, regardless of whether it is current
    /// </summary>
    /// <param name="employeeId">Unique identifier of the employee</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The most recent compensation record, or null if not found</returns>
    Task<CompensationRecord?> GetMostRecentRecordAsync(Guid employeeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new compensation record
    /// </summary>
    /// <param name="record">The record to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddAsync(CompensationRecord record, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing compensation record
    /// </summary>
    /// <param name="record">The record to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateAsync(CompensationRecord record, CancellationToken cancellationToken = default);
}