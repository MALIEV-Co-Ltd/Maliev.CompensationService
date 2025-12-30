using Maliev.CompensationService.Domain.Enums;

namespace Maliev.CompensationService.Application.DTOs;

/// <summary>
/// Data transfer object for benefit enrollment details
/// </summary>
public class BenefitsEnrollmentDto
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
    /// Name of the enrolled benefit
    /// </summary>
    public string BenefitName { get; set; } = string.Empty;

    /// <summary>
    /// The date when the employee enrolled
    /// </summary>
    public DateTime EnrollmentDate { get; set; }

    /// <summary>
    /// The date when the enrollment ended, if applicable
    /// </summary>
    public DateTime? TerminationDate { get; set; }

    /// <summary>
    /// The current status of the enrollment
    /// </summary>
    public EnrollmentStatus Status { get; set; }

    /// <summary>
    /// The actual contribution amount from the employee
    /// </summary>
    public decimal? EmployeeContribution { get; set; }

    /// <summary>
    /// The level of coverage selected
    /// </summary>
    public string? CoverageLevel { get; set; }

    /// <summary>
    /// List of dependents covered under this enrollment
    /// </summary>
    public IEnumerable<DependentDto> Dependents { get; set; } = new List<DependentDto>();
}
