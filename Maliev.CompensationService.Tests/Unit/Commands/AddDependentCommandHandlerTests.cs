using Maliev.CompensationService.Application.Commands;
using Maliev.CompensationService.Application.Commands.Handlers;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Domain.Enums;
using Moq;
using Xunit;

namespace Maliev.CompensationService.Tests.Unit.Commands;

public class AddDependentCommandHandlerTests
{
    private readonly Mock<IBenefitsRepository> _benefitsRepoMock;
    private readonly AddDependentCommandHandler _handler;

    public AddDependentCommandHandlerTests()
    {
        _benefitsRepoMock = new Mock<IBenefitsRepository>();
        _handler = new AddDependentCommandHandler(_benefitsRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldAddDependent_WhenEnrollmentExists()
    {
        var enrollmentId = Guid.NewGuid();
        var enrollment = new BenefitsEnrollment
        {
            Id = enrollmentId,
            EmployeeId = Guid.NewGuid()
        };

        _benefitsRepoMock.Setup(r => r.GetEnrollmentByIdAsync(enrollmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(enrollment);

        var dependentDto = new DependentDto
        {
            FirstName = "John",
            LastName = "Doe",
            Relationship = DependentRelationship.Spouse,
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var command = new AddDependentCommand(enrollmentId, dependentDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
        _benefitsRepoMock.Verify(r => r.AddDependentAsync(It.IsAny<Dependent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenEnrollmentNotFound()
    {
        var enrollmentId = Guid.NewGuid();

        _benefitsRepoMock.Setup(r => r.GetEnrollmentByIdAsync(enrollmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BenefitsEnrollment?)null);

        var dependentDto = new DependentDto
        {
            FirstName = "John",
            LastName = "Doe",
            Relationship = DependentRelationship.Child,
            DateOfBirth = new DateTime(2010, 5, 15)
        };

        var command = new AddDependentCommand(enrollmentId, dependentDto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
