using Maliev.CompensationService.Domain.Enums;

namespace Maliev.CompensationService.Domain.Entities;

/// <summary>
/// Represents an employee's compensation record at a specific point in time
/// </summary>
public class CompensationRecord
{
    /// <summary>
    /// Unique identifier for the compensation record
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Unique identifier for the employee
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Unique identifier for the department the employee belongs to
    /// </summary>
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// The date when this compensation becomes effective
    /// </summary>
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// The base salary amount (stored encrypted)
    /// </summary>
    public decimal BaseSalary { get; set; }

    /// <summary>
    /// The currency of the compensation (e.g., USD)
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// The type of compensation (Salary, Hourly, etc.)
    /// </summary>
    public CompensationType CompensationType { get; set; }

    /// <summary>
    /// Annual bonus percentage, if applicable
    /// </summary>
    public decimal? BonusPercentage { get; set; }

    /// <summary>
    /// Commission rate percentage, if applicable
    /// </summary>
    public decimal? CommissionRate { get; set; }

    /// <summary>
    /// The reason for the compensation change
    /// </summary>
    public string? ChangeReason { get; set; }

    /// <summary>
    /// The identifier of the user who approved this compensation
    /// </summary>
    public Guid? ApprovedBy { get; set; }

    /// <summary>
    /// Indicates if this is the current active compensation record for the employee
    /// </summary>
    public bool IsCurrent { get; set; }

    /// <summary>
    /// The date and time when this record was created
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// The date and time when this record was last modified
    /// </summary>
    public DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// Row version for optimistic concurrency control
    /// </summary>
    public uint RowVersion { get; set; }

    /// <summary>
    /// Historical changes associated with this compensation record
    /// </summary>
    public ICollection<SalaryHistory> SalaryHistories { get; set; } = new List<SalaryHistory>();
}