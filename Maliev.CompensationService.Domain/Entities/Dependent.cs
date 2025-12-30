using Maliev.CompensationService.Domain.Enums;

namespace Maliev.CompensationService.Domain.Entities;

/// <summary>
/// Represents a family member covered under an employee's benefit enrollment
/// </summary>
public class Dependent
{
    /// <summary>
    /// Unique identifier for the dependent
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Unique identifier for the associated benefit enrollment
    /// </summary>
    public Guid BenefitsEnrollmentId { get; set; }

    /// <summary>
    /// First name of the dependent
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name of the dependent
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Relationship of the dependent to the employee
    /// </summary>
    public DependentRelationship Relationship { get; set; }

    /// <summary>
    /// Date of birth of the dependent
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// National identification number (stored encrypted)
    /// </summary>
    public string? NationalId { get; set; }

    /// <summary>
    /// The date and time when this record was created
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// The date and time when this record was last modified
    /// </summary>
    public DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// The associated benefit enrollment
    /// </summary>
    public BenefitsEnrollment Enrollment { get; set; } = null!;
}