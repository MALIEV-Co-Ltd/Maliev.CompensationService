namespace Maliev.CompensationService.Domain.Authorization;

/// <summary>
/// Constants for Compensation Service permissions.
/// Follows GCP-style naming: {service}.{resource}.{action}
/// </summary>
public static class CompensationPermissions
{
    /// <summary>Permission to read compensation data.</summary>
    public const string Read = "compensation.compensations.read";

    /// <summary>Permission to read sensitive compensation data (salary, personal IDs).</summary>
    public const string ReadSensitive = "compensation.compensations.read_sensitive";

    /// <summary>Permission to update compensation data.</summary>
    public const string Update = "compensation.compensations.update";

    /// <summary>Permission for administrative operations.</summary>
    public const string Admin = "compensation.admin.manage";

    /// <summary>Permission to view reports.</summary>
    public const string Reports = "compensation.reports.view";

    /// <summary>
    /// Collection of all permissions for easy registration.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, string> All = new Dictionary<string, string>
    {
        { Read, "Read employee compensation and history" },
        { ReadSensitive, "Read sensitive salary and personal data" },
        { Update, "Update employee compensation and benefits" },
        { Admin, "Perform administrative bulk operations" },
        { Reports, "Generate compensation and budget reports" }
    };
}
