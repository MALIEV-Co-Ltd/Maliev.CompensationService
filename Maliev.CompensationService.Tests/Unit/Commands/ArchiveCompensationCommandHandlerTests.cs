using Maliev.CompensationService.Application.Commands;
using Maliev.CompensationService.Application.Commands.Handlers;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Domain.Entities;
using Maliev.MessagingContracts.Generated;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Maliev.CompensationService.Tests.Unit.Commands;

public class ArchiveCompensationCommandHandlerTests
{
    private readonly Mock<ICompensationRepository> _compRepositoryMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly Mock<ILogger<ArchiveCompensationCommandHandler>> _loggerMock;
    private readonly ArchiveCompensationCommandHandler _handler;

    public ArchiveCompensationCommandHandlerTests()
    {
        _compRepositoryMock = new Mock<ICompensationRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _loggerMock = new Mock<ILogger<ArchiveCompensationCommandHandler>>();
        _handler = new ArchiveCompensationCommandHandler(
            _compRepositoryMock.Object,
            _publishEndpointMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldArchiveRecordAndPublishEvent()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var command = new ArchiveCompensationCommand(employeeId, correlationId);
        var currentRecord = new CompensationRecord
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            IsCurrent = true
        };

        _compRepositoryMock.Setup(r => r.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentRecord);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.False(currentRecord.IsCurrent);
        _compRepositoryMock.Verify(r => r.UpdateAsync(currentRecord, It.IsAny<CancellationToken>()), Times.Once);
        _publishEndpointMock.Verify(p => p.Publish(It.IsAny<CompensationArchivedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
