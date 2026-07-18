using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Mappers;
using Maliev.CompensationService.Domain.Entities;
using Xunit;

namespace Maliev.CompensationService.Tests.Unit.Mappers;

public class SalaryHistoryMapperTests
{
    [Fact]
    public void ToDto_ShouldMapFieldsCorrectly()
    {
        // Arrange
        var history = new SalaryHistory
        {
            Id = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            PreviousSalary = 50000,
            NewSalary = 55000,
            ChangeAmount = 5000,
            ChangePercentage = 10,
            EffectiveDate = DateTime.UtcNow,
            ChangeType = "Merit",
            ChangedBy = Guid.NewGuid(),
            CreatedDate = DateTime.UtcNow
        };

        // Act
        var dto = history.ToDto();

        // Assert
        Assert.Equal(history.Id, dto.Id);
        Assert.Equal(history.EmployeeId, dto.EmployeeId);
        Assert.Equal(history.PreviousSalary, dto.PreviousSalary);
        Assert.Equal(history.NewSalary, dto.NewSalary);
        Assert.Equal(history.ChangeAmount, dto.ChangeAmount);
        Assert.Equal(history.ChangePercentage, dto.ChangePercentage);
        Assert.Equal(history.EffectiveDate, dto.EffectiveDate);
        Assert.Equal(history.ChangeType, dto.ChangeType);
        Assert.Equal(history.ChangedBy, dto.ChangedBy);
    }
}
