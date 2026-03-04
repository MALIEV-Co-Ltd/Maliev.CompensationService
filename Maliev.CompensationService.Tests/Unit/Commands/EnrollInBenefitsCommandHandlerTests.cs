using Maliev.CompensationService.Application.Commands;
using Maliev.CompensationService.Application.Commands.Handlers;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Domain.Enums;
using Moq;
using Xunit;

namespace Maliev.CompensationService.Tests.Unit.Commands;

public class EnrollInBenefitsCommandHandlerTests
{
    private readonly Mock<IBenefitsRepository> _benefitsRepoMock;
    private readonly EnrollInBenefitsCommandHandler _handler;

    public EnrollInBenefitsCommandHandlerTests()
    {
        _benefitsRepoMock = new Mock<IBenefitsRepository>();
        _handler = new EnrollInBenefitsCommandHandler(_benefitsRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateEnrollment_WhenValid()
    {
        var employeeId = Guid.NewGuid();
        var benefitId = Guid.NewGuid();

        var dto = new EnrollInBenefitDto
        {
            EnrollmentDate = DateTime.UtcNow,
            EmployeeContribution = 500,
            CoverageLevel = "Standard",
            Dependents = new List<DependentDto>
            {
                new DependentDto
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    Relationship = DependentRelationship.Spouse,
                    DateOfBirth = new DateTime(1992, 5, 10)
                }
            }
        };

        var command = new EnrollInBenefitsCommand(employeeId, benefitId, dto);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(employeeId, result.EmployeeId);
        Assert.Equal(benefitId, result.BenefitId);
        Assert.Single(result.Dependents);
        _benefitsRepoMock.Verify(r => r.AddEnrollmentAsync(It.IsAny<Domain.Entities.BenefitsEnrollment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateEnrollmentWithNoDependents_WhenEmptyList()
    {
        var employeeId = Guid.NewGuid();
        var benefitId = Guid.NewGuid();

        var dto = new EnrollInBenefitDto
        {
            EnrollmentDate = DateTime.UtcNow,
            EmployeeContribution = 300,
            CoverageLevel = "Basic",
            Dependents = new List<DependentDto>()
        };

        var command = new EnrollInBenefitsCommand(employeeId, benefitId, dto);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Dependents);
    }
}
