namespace Maliev.CompensationService.Domain.Entities;

/// <summary>
/// Represents a historical record of a compensation change for an employee
/// </summary>
public class SalaryHistory
{
    /// <summary>
    /// Unique identifier for the salary history record
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Unique identifier for the employee
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Unique identifier for the associated compensation record
    /// </summary>
    public Guid CompensationRecordId { get; set; }

    /// <summary>
    /// The salary amount before the change (stored encrypted)
    /// </summary>
    public decimal PreviousSalary { get; set; }

    /// <summary>
    /// The new salary amount after the change (stored encrypted)
    /// </summary>
    public decimal NewSalary { get; set; }

    /// <summary>
    /// The absolute amount of change in salary
    /// </summary>
    public decimal ChangeAmount { get; set; }

    /// <summary>
    /// The percentage of change relative to the previous salary
    /// </summary>
    public decimal ChangePercentage { get; set; }

    /// <summary>
    /// Indicates if the salary increase is considered high (e.g., > 25%)
    /// </summary>
    public bool IsHighIncrease { get; set; }

    /// <summary>
    /// The date when this salary change becomes effective
    /// </summary>
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// The type of change (e.g., Promotion, Merit, Initial)
    /// </summary>
    public string ChangeType { get; set; } = string.Empty;

    /// <summary>
    /// The identifier of the user who performed the change
    /// </summary>
    public Guid ChangedBy { get; set; }

    /// <summary>
    /// The date and time when this history record was created
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// The associated compensation record
    /// </summary>
    public CompensationRecord CompensationRecord { get; set; } = null!;
}