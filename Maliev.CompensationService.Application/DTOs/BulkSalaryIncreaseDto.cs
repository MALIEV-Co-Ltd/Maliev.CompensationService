using System.ComponentModel.DataAnnotations;

namespace Maliev.CompensationService.Application.DTOs;

/// <summary>
/// Data transfer object for requesting a bulk salary increase.
/// </summary>
public class BulkSalaryIncreaseDto
{
    /// <summary>
    /// Gets or sets the optional identifier of the department to apply the increase to.
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// Gets or sets the percentage increase to apply (e.g., 5.5 for 5.5%).
    /// </summary>
    [Required]
    [Range(0, 100, ErrorMessage = "Increase percentage must be between 0 and 100")]
    public decimal PercentageIncrease { get; set; }

    /// <summary>
    /// Gets or sets the reason for the bulk increase.
    /// </summary>
    [Required]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date when the increases should become effective.
    /// </summary>
    [Required]
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to only preview the changes.
    /// </summary>
    public bool PreviewOnly { get; set; }

    /// <summary>
    /// Gets or sets the optional URL to notify when the job completes.
    /// </summary>
    [Url]
    public string? WebhookUrl { get; set; }
}