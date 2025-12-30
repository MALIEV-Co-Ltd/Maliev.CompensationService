using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Domain.Entities;

namespace Maliev.CompensationService.Application.Mappers;

/// <summary>
/// Mapper for benefits related entities and DTOs
/// </summary>
public static class BenefitsMapper
{
    /// <summary>
    /// Maps a <see cref="Benefit"/> entity to a <see cref="BenefitDto"/>
    /// </summary>
    public static BenefitDto ToDto(this Benefit benefit)
    {
        return new BenefitDto
        {
            Id = benefit.Id,
            Name = benefit.Name,
            Description = benefit.Description,
            BenefitType = benefit.BenefitType,
            EmployerContribution = benefit.EmployerContribution,
            EmployeeContribution = benefit.EmployeeContribution,
            WaitingPeriodDays = benefit.WaitingPeriodDays,
            IsActive = benefit.IsActive
        };
    }

    /// <summary>
    /// Maps a <see cref="BenefitsEnrollment"/> entity to a <see cref="BenefitsEnrollmentDto"/>
    /// </summary>
    public static BenefitsEnrollmentDto ToDto(this BenefitsEnrollment enrollment)
    {
        return new BenefitsEnrollmentDto
        {
            Id = enrollment.Id,
            EmployeeId = enrollment.EmployeeId,
            BenefitId = enrollment.BenefitId,
            BenefitName = enrollment.Benefit?.Name ?? string.Empty,
            EnrollmentDate = enrollment.EnrollmentDate,
            TerminationDate = enrollment.TerminationDate,
            Status = enrollment.Status,
            EmployeeContribution = enrollment.EmployeeContribution,
            CoverageLevel = enrollment.CoverageLevel,
            Dependents = enrollment.Dependents.Select(d => d.ToDto())
        };
    }

    /// <summary>
    /// Maps a <see cref="Dependent"/> entity to a <see cref="DependentDto"/>
    /// </summary>
    public static DependentDto ToDto(this Dependent dependent)
    {
        return new DependentDto
        {
            Id = dependent.Id,
            FirstName = dependent.FirstName,
            LastName = dependent.LastName,
            Relationship = dependent.Relationship,
            DateOfBirth = dependent.DateOfBirth,
            NationalId = dependent.NationalId
        };
    }

    /// <summary>
    /// Maps a <see cref="DependentDto"/> to a <see cref="Dependent"/> entity
    /// </summary>
    public static Dependent ToEntity(this DependentDto dto, Guid enrollmentId)
    {
        return new Dependent
        {
            Id = dto.Id ?? Guid.NewGuid(),
            BenefitsEnrollmentId = enrollmentId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Relationship = dto.Relationship,
            DateOfBirth = dto.DateOfBirth,
            NationalId = dto.NationalId,
            CreatedDate = DateTime.UtcNow
        };
    }
}
