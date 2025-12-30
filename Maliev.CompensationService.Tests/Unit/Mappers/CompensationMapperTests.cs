using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Mappers;
using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Domain.Enums;
using Xunit;

namespace Maliev.CompensationService.Tests.Unit.Mappers;

public class CompensationMapperTests
{
    [Fact]
    public void ToDto_ShouldMapFieldsCorrectly()
    {
        // Arrange
        var record = new CompensationRecord
        {
            Id = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            EffectiveDate = DateTime.UtcNow,
            BaseSalary = 50000,
            Currency = "USD",
            CompensationType = CompensationType.Salary,
            BonusPercentage = 10,
            CommissionRate = 5,
            ChangeReason = "Promotion"
        };

        // Act
        var dto = record.ToDto();

        // Assert
        Assert.Equal(record.Id, dto.Id);
        Assert.Equal(record.EmployeeId, dto.EmployeeId);
        Assert.Equal(record.EffectiveDate, dto.EffectiveDate);
        Assert.Equal(record.BaseSalary, dto.BaseSalary);
        Assert.Equal(record.Currency, dto.Currency);
        Assert.Equal(record.CompensationType, dto.CompensationType);
        Assert.Equal(record.BonusPercentage, dto.BonusPercentage);
        Assert.Equal(record.CommissionRate, dto.CommissionRate);
        Assert.Equal(record.ChangeReason, dto.ChangeReason);
    }
}
