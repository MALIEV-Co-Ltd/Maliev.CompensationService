using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Common.Mediator;

namespace Maliev.CompensationService.Application.Commands;

/// <summary>
/// Command to apply a bulk salary increase to multiple employees.
/// </summary>
public class BulkSalaryIncreaseCommand : IRequest<BulkSalaryIncreaseResultDto>
{
    /// <summary>
    /// Gets or sets the unique identifier of the department to apply the increase to.
    /// If null, applies to all departments.
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// Gets or sets the percentage increase to apply (e.g., 5.0 for 5%).
    /// </summary>
    public decimal PercentageIncrease { get; set; }

    /// <summary>
    /// Gets or sets the reason for the bulk increase.
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date when the increase becomes effective.
    /// </summary>
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to only preview the changes without committing them.
    /// </summary>
    public bool PreviewOnly { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user who initiated the bulk operation.
    /// </summary>
    public Guid InitiatedByUserId { get; set; }
}

/// <summary>
/// Result of a bulk salary increase operation.
/// </summary>
public class BulkSalaryIncreaseResultDto
{
    /// <summary>
    /// Gets or sets the total number of employees processed in the bulk operation.
    /// </summary>
    public int TotalEmployeesProcessed { get; set; }

    /// <summary>
    /// Gets or sets the total amount of salary increase across all processed employees.
    /// </summary>
    public decimal TotalIncreaseAmount { get; set; }

    /// <summary>
    /// Gets or sets the currency used for the increase amount.
    /// </summary>
    public string Currency { get; set; } = string.Empty;
}