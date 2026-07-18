using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Mappers;
using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Domain.Enums;
using Xunit;

namespace Maliev.CompensationService.Tests.Unit.Mappers;

public class BenefitsMapperTests
{
    [Fact]
    public void ToDto_ShouldMapBenefitCorrectly()
    {
        // Arrange
        var benefit = new Benefit
        {
            Id = Guid.NewGuid(),
            Name = "Health",
            BenefitType = BenefitType.HealthInsurance,
            IsActive = true
        };

        // Act
        var dto = benefit.ToDto();

        // Assert
        Assert.Equal(benefit.Id, dto.Id);
        Assert.Equal(benefit.Name, dto.Name);
        Assert.Equal(benefit.BenefitType, dto.BenefitType);
    }

    [Fact]
    public void ToDto_ShouldMapEnrollmentWithDependents()
    {
        // Arrange
        var enrollment = new BenefitsEnrollment
        {
            Id = Guid.NewGuid(),
            Benefit = new Benefit { Name = "Dental" },
            Dependents = new List<Dependent>
            {
                new Dependent { FirstName = "John", LastName = "Doe" }
            }
        };

        // Act
        var dto = enrollment.ToDto();

        // Assert
        Assert.Equal("Dental", dto.BenefitName);
        Assert.Single(dto.Dependents);
        Assert.Equal("John", dto.Dependents.First().FirstName);
    }
}
