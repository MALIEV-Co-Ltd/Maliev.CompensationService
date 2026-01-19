using Maliev.CompensationService.Application.Commands;
using Maliev.CompensationService.Application.Commands.Handlers;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Maliev.CompensationService.Tests.Unit.Commands;

public class UndoArchiveCompensationCommandHandlerTests
{
    private readonly Mock<ICompensationRepository> _compRepositoryMock;
    private readonly Mock<ILogger<UndoArchiveCompensationCommandHandler>> _loggerMock;
    private readonly UndoArchiveCompensationCommandHandler _handler;

    public UndoArchiveCompensationCommandHandlerTests()
    {
        _compRepositoryMock = new Mock<ICompensationRepository>();
        _loggerMock = new Mock<ILogger<UndoArchiveCompensationCommandHandler>>();
        _handler = new UndoArchiveCompensationCommandHandler(
            _compRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldRestoreRecord_WhenNoCurrentExists()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var command = new UndoArchiveCompensationCommand { EmployeeId = employeeId };
        var mostRecent = new CompensationRecord
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            IsCurrent = false
        };

        _compRepositoryMock.Setup(r => r.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CompensationRecord?)null);
        _compRepositoryMock.Setup(r => r.GetMostRecentRecordAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mostRecent);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(mostRecent.IsCurrent);
        _compRepositoryMock.Verify(r => r.UpdateAsync(mostRecent, It.IsAny<CancellationToken>()), Times.Once);
    }
}
