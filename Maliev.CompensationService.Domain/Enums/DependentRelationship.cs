namespace Maliev.CompensationService.Domain.Enums;

/// <summary>
/// Represents the relationship of a dependent to the employee
/// </summary>
public enum DependentRelationship
{
    /// <summary>
    /// Spouse or domestic partner
    /// </summary>
    Spouse = 0,

    /// <summary>
    /// Child (biological, adopted, or stepchild)
    /// </summary>
    Child = 1,

    /// <summary>
    /// Domestic partner
    /// </summary>
    DomesticPartner = 2,

    /// <summary>
    /// Parent or guardian
    /// </summary>
    Parent = 3,

    /// <summary>
    /// Other dependent relationship
    /// </summary>
    Other = 4
}
