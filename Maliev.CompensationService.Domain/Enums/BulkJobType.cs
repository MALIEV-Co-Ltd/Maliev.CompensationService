namespace Maliev.CompensationService.Domain.Enums;

/// <summary>
/// Represents the type of bulk operation being performed
/// </summary>
public enum BulkJobType
{
    /// <summary>
    /// Bulk salary increase operation
    /// </summary>
    SalaryIncrease = 0,

    /// <summary>
    /// Bulk bonus assignment
    /// </summary>
    BonusAssignment = 1,

    /// <summary>
    /// Bulk benefits enrollment
    /// </summary>
    BenefitsEnrollment = 2
}
