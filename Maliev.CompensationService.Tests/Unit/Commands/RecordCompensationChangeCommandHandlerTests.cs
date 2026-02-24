using Maliev.CompensationService.Application.Commands;
using Maliev.CompensationService.Application.Commands.Handlers;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Domain.Enums;
using Maliev.MessagingContracts.Generated;
using Maliev.MessagingContracts.Contracts.Compensation;
using MassTransit;
using Moq;
using Xunit;

namespace Maliev.CompensationService.Tests.Unit.Commands;

public class RecordCompensationChangeCommandHandlerTests
{
    private readonly Mock<ICompensationRepository> _compRepoMock;
    private readonly Mock<ISalaryHistoryRepository> _historyRepoMock;
    private readonly Mock<IPublishEndpoint> _publishMock;
    private readonly RecordCompensationChangeCommandHandler _handler;

    public RecordCompensationChangeCommandHandlerTests()
    {
        _compRepoMock = new Mock<ICompensationRepository>();
        _historyRepoMock = new Mock<ISalaryHistoryRepository>();
        _publishMock = new Mock<IPublishEndpoint>();
        _handler = new RecordCompensationChangeCommandHandler(
            _compRepoMock.Object,
            _historyRepoMock.Object,
            _publishMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateNewRecordAndHistory_WhenValid()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var changedBy = Guid.NewGuid();
        var currentRecord = new CompensationRecord
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            BaseSalary = 50000,
            IsCurrent = true
        };

        _compRepoMock.Setup(r => r.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentRecord);

        _compRepoMock.Setup(r => r.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task<SalaryHistoryDto>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<SalaryHistoryDto>>, CancellationToken>((action, ct) => action(ct));

        var dto = new RecordCompensationChangeDto
        {
            NewBaseSalary = 55000,
            Currency = "USD",
            CompensationType = CompensationType.Salary,
            EffectiveDate = DateTime.UtcNow.AddDays(1),
            ChangeType = "Merit",
            ChangeReason = "Good performance"
        };

        var command = new RecordCompensationChangeCommand(employeeId, dto, changedBy);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _compRepoMock.Verify(r => r.UpdateAsync(It.IsAny<CompensationRecord>()), Times.Once);
        _historyRepoMock.Verify(r => r.AddAsync(It.IsAny<SalaryHistory>()), Times.Once);
        _publishMock.Verify(p => p.Publish(It.IsAny<SalaryChangedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
