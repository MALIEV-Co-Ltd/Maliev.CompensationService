using Maliev.CompensationService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

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

    /// <summary>
    /// Starts a new database transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task that represents the asynchronous transaction operation</returns>
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an action within a transaction using the configured execution strategy.
    /// </summary>
    /// <typeparam name="T">The type of the return value of the action.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous execution operation.</returns>
    Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default);
}