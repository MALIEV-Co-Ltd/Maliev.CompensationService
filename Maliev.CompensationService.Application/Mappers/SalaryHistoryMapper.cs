using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Domain.Entities;

namespace Maliev.CompensationService.Application.Mappers;

/// <summary>
/// Mapper for salary history related entities and DTOs
/// </summary>
public static class SalaryHistoryMapper
{
    /// <summary>
    /// Maps a <see cref="SalaryHistory"/> entity to a <see cref="SalaryHistoryDto"/>
    /// </summary>
    /// <param name="history">The entity to map</param>
    /// <returns>The mapped DTO</returns>
    public static SalaryHistoryDto ToDto(this SalaryHistory history)
    {
        return new SalaryHistoryDto
        {
            Id = history.Id,
            EmployeeId = history.EmployeeId,
            PreviousSalary = history.PreviousSalary,
            NewSalary = history.NewSalary,
            ChangeAmount = history.ChangeAmount,
            ChangePercentage = history.ChangePercentage,
            IsHighIncrease = history.IsHighIncrease,
            EffectiveDate = history.EffectiveDate,
            ChangeType = history.ChangeType,
            ChangedBy = history.ChangedBy
        };
    }
}
