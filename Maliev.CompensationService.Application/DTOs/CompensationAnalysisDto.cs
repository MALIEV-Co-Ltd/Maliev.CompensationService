namespace Maliev.CompensationService.Application.DTOs;

/// <summary>
/// Data transfer object for compensation analysis report
/// </summary>
public class CompensationAnalysisDto
{
    /// <summary>
    /// Total number of employees in the analysis
    /// </summary>
    public int TotalEmployees { get; set; }

    /// <summary>
    /// Total annual budget for the group
    /// </summary>
    public decimal TotalAnnualBudget { get; set; }

    /// <summary>
    /// Average annual salary
    /// </summary>
    public decimal AverageSalary { get; set; }

    /// <summary>
    /// Minimum annual salary in the group
    /// </summary>
    public decimal MinimumSalary { get; set; }

    /// <summary>
    /// Maximum annual salary in the group
    /// </summary>
    public decimal MaximumSalary { get; set; }

    /// <summary>
    /// Currency code used for the analysis
    /// </summary>
    public string Currency { get; set; } = "USD";
}
