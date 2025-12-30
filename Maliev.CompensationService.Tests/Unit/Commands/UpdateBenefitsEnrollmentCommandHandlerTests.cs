using Maliev.CompensationService.Application.Commands;
using Maliev.CompensationService.Application.Commands.Handlers;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Domain.Entities;
using Moq;
using Xunit;

namespace Maliev.CompensationService.Tests.Unit.Commands;

public class UpdateBenefitsEnrollmentCommandHandlerTests
{
    private readonly Mock<IBenefitsRepository> _repositoryMock;
    private readonly UpdateBenefitsEnrollmentCommandHandler _handler;

    public UpdateBenefitsEnrollmentCommandHandlerTests()
    {
        _repositoryMock = new Mock<IBenefitsRepository>();
        _handler = new UpdateBenefitsEnrollmentCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateEnrollment_WhenExists()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var enrollmentId = Guid.NewGuid();
        var enrollment = new BenefitsEnrollment { Id = enrollmentId, EmployeeId = employeeId };

        _repositoryMock.Setup(r => r.GetEnrollmentByIdAsync(enrollmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(enrollment);

        var dto = new UpdateBenefitsEnrollmentDto
        {
            EmployeeContribution = 150,
            CoverageLevel = "Family",
            Dependents = new List<DependentDto>()
        };

        var command = new UpdateBenefitsEnrollmentCommand(enrollmentId, dto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _repositoryMock.Verify(r => r.UpdateEnrollmentAsync(It.IsAny<BenefitsEnrollment>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}