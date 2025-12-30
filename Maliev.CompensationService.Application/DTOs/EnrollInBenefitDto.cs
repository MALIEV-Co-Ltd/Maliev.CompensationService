using System.ComponentModel.DataAnnotations;

namespace Maliev.CompensationService.Application.DTOs;

/// <summary>
/// Data transfer object for enrolling an employee in a benefit
/// </summary>
public class EnrollInBenefitDto
{
    /// <summary>
    /// Unique identifier for the benefit program
    /// </summary>
    [Required]
    public Guid BenefitId { get; set; }

    /// <summary>
    /// The date when the employee enrollment should start
    /// </summary>
    [Required]
    public DateTime EnrollmentDate { get; set; }

    /// <summary>
    /// The actual contribution amount from the employee
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Contribution must be a positive value")]
    public decimal? EmployeeContribution { get; set; }

    /// <summary>
    /// The level of coverage selected
    /// </summary>
    [MaxLength(50)]
    public string? CoverageLevel { get; set; }

    /// <summary>
    /// List of dependents to be covered under this enrollment
    /// </summary>
    public IEnumerable<DependentDto> Dependents { get; set; } = new List<DependentDto>();
}
