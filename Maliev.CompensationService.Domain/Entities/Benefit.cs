using Maliev.CompensationService.Domain.Enums;

namespace Maliev.CompensationService.Domain.Entities;

/// <summary>
/// Represents a benefit program offered by the organization
/// </summary>
public class Benefit
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
    /// Indicates if the benefit is currently available for enrollment
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// The date and time when this benefit record was created
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// The date and time when this record was last modified
    /// </summary>
    public DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// List of employee enrollments for this benefit
    /// </summary>
    public ICollection<BenefitsEnrollment> Enrollments { get; set; } = new List<BenefitsEnrollment>();
}
