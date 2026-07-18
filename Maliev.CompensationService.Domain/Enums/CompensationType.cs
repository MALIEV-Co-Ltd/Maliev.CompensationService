namespace Maliev.CompensationService.Domain.Enums;

/// <summary>
/// Represents the type of compensation structure for an employee
/// </summary>
public enum CompensationType
{
    /// <summary>
    /// Fixed annual salary
    /// </summary>
    Salary = 0,

    /// <summary>
    /// Hourly wage with variable hours
    /// </summary>
    Hourly = 1,

    /// <summary>
    /// Contract-based compensation
    /// </summary>
    Contract = 2,

    /// <summary>
    /// Commission-based compensation
    /// </summary>
    Commission = 3
}
