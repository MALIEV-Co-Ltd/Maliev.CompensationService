namespace Maliev.CompensationService.Domain.Authorization;

/// <summary>
/// Defines permission constants for compensation service operations
/// </summary>
public static class CompensationPermissions
{
    /// <summary>
    /// Permission to read compensation data
    /// </summary>
    public const string Read = "compensation.records.read";

    /// <summary>
    /// Permission to update compensation records
    /// </summary>
    public const string Update = "compensation.records.update";

    /// <summary>
    /// Permission to perform administrative operations (bulk operations, etc.)
    /// </summary>
    public const string Admin = "compensation.admin.manage";

    /// <summary>
    /// Permission to generate and view compensation reports
    /// </summary>
    public const string Reports = "compensation.reports.view";
}