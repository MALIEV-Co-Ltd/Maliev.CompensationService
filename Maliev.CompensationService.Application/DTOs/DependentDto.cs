using Maliev.CompensationService.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Maliev.CompensationService.Application.DTOs;

/// <summary>
/// Data transfer object for dependent details
/// </summary>
public class DependentDto
{
    /// <summary>
    /// Unique identifier for the dependent (can be null for new dependents)
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// First name of the dependent
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name of the dependent
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Relationship of the dependent to the employee
    /// </summary>
    [Required]
    public DependentRelationship Relationship { get; set; }

    /// <summary>
    /// Date of birth of the dependent
    /// </summary>
    [Required]
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// National identification number
    /// </summary>
    [MaxLength(50)]
    public string? NationalId { get; set; }
}
