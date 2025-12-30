using System.ComponentModel.DataAnnotations;

namespace Maliev.CompensationService.Application.DTOs;

/// <summary>
/// Data transfer object for updating an existing benefit enrollment
/// </summary>
public class UpdateBenefitsEnrollmentDto
{
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
    /// The current status of the enrollment
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// List of dependents covered under this enrollment
    /// </summary>
    public IEnumerable<DependentDto> Dependents { get; set; } = new List<DependentDto>();
}