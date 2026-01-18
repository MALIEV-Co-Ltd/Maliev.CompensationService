using Maliev.CompensationService.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Maliev.CompensationService.Application.DTOs;

/// <summary>
/// Data transfer object for recording a compensation change
/// </summary>
public class RecordCompensationChangeDto
{
    /// <summary>
    /// The new base salary amount
    /// </summary>
    [Required]
    public decimal NewBaseSalary { get; set; }

    /// <summary>
    /// Currency code (e.g., USD)
    /// </summary>
    [Required]
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Type of compensation
    /// </summary>
    [Required]
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
    /// Date when the change becomes effective
    /// </summary>
    [Required]
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Type of change (e.g., Promotion, Merit)
    /// </summary>
    [Required]
    public string ChangeType { get; set; } = string.Empty;

    /// <summary>
    /// Reason for the change
    /// </summary>
    public string? ChangeReason { get; set; }
}