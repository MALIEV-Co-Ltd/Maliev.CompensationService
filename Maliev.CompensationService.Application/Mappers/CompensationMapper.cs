using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Domain.Entities;

namespace Maliev.CompensationService.Application.Mappers;

/// <summary>
/// Mapper for compensation related entities and DTOs
/// </summary>
public static class CompensationMapper
{
    /// <summary>
    /// Maps a <see cref="CompensationRecord"/> entity to a <see cref="CompensationRecordDto"/>
    /// </summary>
    /// <param name="record">The entity to map</param>
    /// <returns>The mapped DTO</returns>
    public static CompensationRecordDto ToDto(this CompensationRecord record)
    {
        return new CompensationRecordDto
        {
            Id = record.Id,
            EmployeeId = record.EmployeeId,
            DepartmentId = record.DepartmentId,
            EffectiveDate = record.EffectiveDate,
            BaseSalary = record.BaseSalary,
            Currency = record.Currency,
            CompensationType = record.CompensationType,
            BonusPercentage = record.BonusPercentage,
            CommissionRate = record.CommissionRate,
            ChangeReason = record.ChangeReason
        };
    }
}