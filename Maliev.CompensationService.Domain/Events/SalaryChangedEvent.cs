namespace Maliev.CompensationService.Domain.Events;

/// <summary>
/// Integration event raised when an employee's salary is changed.
/// </summary>
/// <param name="EmployeeId">The unique identifier of the employee.</param>
/// <param name="CompensationRecordId">The unique identifier of the compensation record.</param>
/// <param name="NewSalary">The new salary amount.</param>
/// <param name="PreviousSalary">The previous salary amount.</param>
/// <param name="ChangePercentage">The percentage of change.</param>
/// <param name="EffectiveDate">The date the change takes effect.</param>
/// <param name="ChangeReason">The reason for the change.</param>
public record SalaryChangedEvent(
    Guid EmployeeId,
    Guid CompensationRecordId,
    decimal NewSalary,
    decimal PreviousSalary,
    decimal ChangePercentage,
    DateTime EffectiveDate,
    string ChangeReason);