namespace Maliev.CompensationService.Application.DTOs;

/// <summary>
/// Data transfer object for salary history details
/// </summary>
public class SalaryHistoryDto
{
    /// <summary>
    /// Unique identifier for the history record
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Unique identifier for the employee
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Salary amount before the change
    /// </summary>
    public decimal PreviousSalary { get; set; }

    /// <summary>
    /// New salary amount after the change
    /// </summary>
    public decimal NewSalary { get; set; }

    /// <summary>
    /// Absolute change amount
    /// </summary>
    public decimal ChangeAmount { get; set; }

    /// <summary>
    /// Percentage change
    /// </summary>
    public decimal ChangePercentage { get; set; }

    /// <summary>
    /// Indicates if the increase was high (e.g., > 25%)
    /// </summary>
    public bool IsHighIncrease { get; set; }

    /// <summary>
    /// Effective date of the change
    /// </summary>
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Type of change (e.g., Promotion, Merit)
    /// </summary>
    public string ChangeType { get; set; } = string.Empty;

    /// <summary>
    /// Identifier of the user who performed the change
    /// </summary>
    public Guid ChangedBy { get; set; }
}