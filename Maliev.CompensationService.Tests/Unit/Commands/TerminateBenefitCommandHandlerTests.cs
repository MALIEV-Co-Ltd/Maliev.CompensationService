using Maliev.CompensationService.Application.Commands;
using Maliev.CompensationService.Application.Commands.Handlers;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Domain.Enums;
using Maliev.CompensationService.Domain.Events;
using MassTransit;
using Moq;
using Xunit;

namespace Maliev.CompensationService.Tests.Unit.Commands;

public class TerminateBenefitCommandHandlerTests
{
    private readonly Mock<IBenefitsRepository> _repositoryMock;
    private readonly Mock<IPublishEndpoint> _publishMock;
    private readonly TerminateBenefitCommandHandler _handler;

    public TerminateBenefitCommandHandlerTests()
    {
        _repositoryMock = new Mock<IBenefitsRepository>();
        _publishMock = new Mock<IPublishEndpoint>();
        _handler = new TerminateBenefitCommandHandler(_repositoryMock.Object, _publishMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldTerminateEnrollment_WhenExists()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var enrollmentId = Guid.NewGuid();
        var enrollment = new BenefitsEnrollment { Id = enrollmentId, EmployeeId = employeeId, Status = EnrollmentStatus.Active };

        _repositoryMock.Setup(r => r.GetEnrollmentByIdAsync(enrollmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(enrollment);

        var terminationDate = DateTime.UtcNow;
        var command = new TerminateBenefitCommand(employeeId, enrollmentId, terminationDate);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(EnrollmentStatus.Terminated, enrollment.Status);
        Assert.Equal(terminationDate, enrollment.TerminationDate);
        _repositoryMock.Verify(r => r.UpdateEnrollmentAsync(It.IsAny<BenefitsEnrollment>(), It.IsAny<CancellationToken>()), Times.Once);
        _publishMock.Verify(p => p.Publish(It.IsAny<BenefitsEnrollmentUpdatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
