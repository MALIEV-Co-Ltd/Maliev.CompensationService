using Maliev.CompensationService.Domain.Entities;

namespace Maliev.CompensationService.Application.Interfaces;

/// <summary>
/// Repository for managing benefits and enrollments
/// </summary>
public interface IBenefitsRepository
{
    /// <summary>
    /// Gets all active benefits available for enrollment
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of active benefits</returns>
    Task<IEnumerable<Benefit>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all benefit enrollments for a specific employee
    /// </summary>
    /// <param name="employeeId">Unique identifier of the employee</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of benefit enrollments</returns>
    Task<IEnumerable<BenefitsEnrollment>> GetEnrollmentsByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific benefit enrollment by its identifier
    /// </summary>
    /// <param name="enrollmentId">Unique identifier of the enrollment</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The enrollment record, or null if not found</returns>
    Task<BenefitsEnrollment?> GetEnrollmentByIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new benefit enrollment
    /// </summary>
    /// <param name="enrollment">The enrollment to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddEnrollmentAsync(BenefitsEnrollment enrollment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing benefit enrollment
    /// </summary>
    /// <param name="enrollment">The enrollment to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateEnrollmentAsync(BenefitsEnrollment enrollment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Terminates all active enrollments for an employee (typically on termination)
    /// </summary>
    /// <param name="employeeId">Unique identifier of the employee</param>
    /// <param name="terminationDate">The date when enrollments should end</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task TerminateActiveEnrollmentsAsync(Guid employeeId, DateTime terminationDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a dependent to an existing enrollment
    /// </summary>
    Task AddDependentAsync(Dependent dependent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rejects all pending enrollments for an employee
    /// </summary>
    /// <param name="employeeId">Unique identifier of the employee</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RejectPendingEnrollmentsAsync(Guid employeeId, CancellationToken cancellationToken = default);
}