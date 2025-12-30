namespace Maliev.CompensationService.Domain.Enums;

/// <summary>
/// Represents the current status of a benefits enrollment
/// </summary>
public enum EnrollmentStatus
{
    /// <summary>
    /// Enrollment is active and in effect
    /// </summary>
    Active = 0,

    /// <summary>
    /// Enrollment is pending (e.g., waiting period)
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Enrollment has been terminated
    /// </summary>
    Terminated = 2,

    /// <summary>
    /// Enrollment is temporarily on hold
    /// </summary>
    OnHold = 3
}
