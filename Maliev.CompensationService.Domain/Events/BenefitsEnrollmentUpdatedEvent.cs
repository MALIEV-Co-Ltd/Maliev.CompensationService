namespace Maliev.CompensationService.Domain.Events;

/// <summary>
/// Event published when an employee's benefit enrollment is updated
/// </summary>
/// <param name="EmployeeId">Unique identifier of the employee</param>
/// <param name="BenefitId">Unique identifier of the benefit</param>
/// <param name="Status">The new status of the enrollment (e.g., Active, Terminated)</param>
/// <param name="EffectiveDate">The date when the change becomes effective</param>
public record BenefitsEnrollmentUpdatedEvent(
    Guid EmployeeId,
    Guid BenefitId,
    string Status,
    DateTime EffectiveDate);
