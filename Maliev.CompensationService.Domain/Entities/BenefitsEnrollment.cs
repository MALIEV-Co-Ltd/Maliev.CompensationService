using Maliev.CompensationService.Domain.Enums;

namespace Maliev.CompensationService.Domain.Entities;

/// <summary>
/// Represents an employee's enrollment in a specific benefit program
/// </summary>
public class BenefitsEnrollment
{
    /// <summary>
    /// Unique identifier for the enrollment
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Unique identifier for the employee
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Unique identifier for the benefit program
    /// </summary>
    public Guid BenefitId { get; set; }

    /// <summary>
    /// The date when the employee enrolled in the benefit
    /// </summary>
    public DateTime EnrollmentDate { get; set; }

    /// <summary>
    /// The date when the enrollment was terminated, if applicable
    /// </summary>
    public DateTime? TerminationDate { get; set; }

    /// <summary>
    /// The current status of the enrollment
    /// </summary>
    public EnrollmentStatus Status { get; set; }

    /// <summary>
    /// The actual contribution amount from the employee for this enrollment
    /// </summary>
    public decimal? EmployeeContribution { get; set; }

    /// <summary>
    /// The level of coverage selected (e.g., Individual, Family)
    /// </summary>
    public string? CoverageLevel { get; set; }

    /// <summary>
    /// The date and time when this enrollment record was created
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// The date and time when this record was last modified
    /// </summary>
    public DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// The associated benefit program
    /// </summary>
    public Benefit Benefit { get; set; } = null!;

    /// <summary>
    /// List of dependents covered under this enrollment
    /// </summary>
    public ICollection<Dependent> Dependents { get; set; } = new List<Dependent>();
}