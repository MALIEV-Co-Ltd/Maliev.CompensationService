using Maliev.CompensationService.Domain.Enums;

namespace Maliev.CompensationService.Application.DTOs;

/// <summary>
/// Data transfer object for compensation record details
/// </summary>
public class CompensationRecordDto
{
    /// <summary>
    /// Unique identifier for the record
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Unique identifier for the employee
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Unique identifier for the department
    /// </summary>
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// Date when the compensation became effective
    /// </summary>
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Base salary amount
    /// </summary>
    public decimal BaseSalary { get; set; }

    /// <summary>
    /// Currency code (e.g., USD)
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Type of compensation (Salary, Hourly, etc.)
    /// </summary>
    public CompensationType CompensationType { get; set; }

    /// <summary>
    /// Annual bonus percentage
    /// </summary>
    public decimal? BonusPercentage { get; set; }

    /// <summary>
    /// Commission rate percentage
    /// </summary>
    public decimal? CommissionRate { get; set; }

    /// <summary>
    /// Reason for the compensation entry
    /// </summary>
    public string? ChangeReason { get; set; }
}
