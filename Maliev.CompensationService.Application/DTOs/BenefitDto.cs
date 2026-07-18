using Maliev.CompensationService.Domain.Enums;

namespace Maliev.CompensationService.Application.DTOs;

/// <summary>
/// Data transfer object for benefit program details
/// </summary>
public class BenefitDto
{
    /// <summary>
    /// Unique identifier for the benefit
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the benefit program
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the benefit
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Category of the benefit
    /// </summary>
    public BenefitType BenefitType { get; set; }

    /// <summary>
    /// Default contribution amount from the employer
    /// </summary>
    public decimal? EmployerContribution { get; set; }

    /// <summary>
    /// Default contribution amount from the employee
    /// </summary>
    public decimal? EmployeeContribution { get; set; }

    /// <summary>
    /// The number of days an employee must wait before the benefit becomes active
    /// </summary>
    public int WaitingPeriodDays { get; set; }

    /// <summary>
    /// Indicates if the benefit is currently active
    /// </summary>
    public bool IsActive { get; set; }
}
