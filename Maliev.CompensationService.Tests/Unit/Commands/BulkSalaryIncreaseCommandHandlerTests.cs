using Maliev.CompensationService.Application.Commands;
using Maliev.CompensationService.Application.Commands.Handlers;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Domain.Enums;
using Maliev.MessagingContracts;
using Maliev.MessagingContracts.Contracts.Compensation;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Maliev.CompensationService.Tests.Unit.Commands;

public class BulkSalaryIncreaseCommandHandlerTests
{
    private readonly Mock<ICompensationRepository> _compRepoMock;
    private readonly Mock<ISalaryHistoryRepository> _historyRepoMock;
    private readonly Mock<IBulkJobRepository> _bulkJobRepoMock;
    private readonly Mock<IPublishEndpoint> _publishMock;
    private readonly Mock<ILogger<BulkSalaryIncreaseCommandHandler>> _loggerMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly BulkSalaryIncreaseCommandHandler _handler;

    public BulkSalaryIncreaseCommandHandlerTests()
    {
        _compRepoMock = new Mock<ICompensationRepository>();
        _historyRepoMock = new Mock<ISalaryHistoryRepository>();
        _bulkJobRepoMock = new Mock<IBulkJobRepository>();
        _publishMock = new Mock<IPublishEndpoint>();
        _loggerMock = new Mock<ILogger<BulkSalaryIncreaseCommandHandler>>();
        _configMock = new Mock<IConfiguration>();

        var configSection = new Mock<IConfigurationSection>();
        configSection.Setup(s => s.Value).Returns("25");
        _configMock.Setup(c => c.GetSection("CompensationSettings:HighIncreaseThreshold")).Returns(configSection.Object);

        _handler = new BulkSalaryIncreaseCommandHandler(
            _compRepoMock.Object,
            _historyRepoMock.Object,
            _bulkJobRepoMock.Object,
            _publishMock.Object,
            _loggerMock.Object,
            _configMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPreviewResult_WhenPreviewOnlyIsTrue()
    {
        var departmentId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        
        var records = new List<CompensationRecord>
        {
            new CompensationRecord
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                DepartmentId = departmentId,
                BaseSalary = 50000,
                Currency = "USD",
                IsCurrent = true
            }
        };

        _compRepoMock.Setup(r => r.GetAllCurrentAsync(departmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(records);

        var command = new BulkSalaryIncreaseCommand
        {
            DepartmentId = departmentId,
            PercentageIncrease = 10,
            EffectiveDate = DateTime.UtcNow.AddDays(30),
            Reason = "Annual review",
            InitiatedByUserId = Guid.NewGuid(),
            PreviewOnly = true
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.TotalEmployeesProcessed == 1);
        _bulkJobRepoMock.Verify(r => r.AddAsync(It.IsAny<BulkJob>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldProcessBulkIncrease_WhenNotPreview()
    {
        var departmentId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var changedBy = Guid.NewGuid();
        
        var records = new List<CompensationRecord>
        {
            new CompensationRecord
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                DepartmentId = departmentId,
                BaseSalary = 50000,
                Currency = "USD",
                CompensationType = CompensationType.Salary,
                IsCurrent = true
            }
        };

        _compRepoMock.Setup(r => r.GetAllCurrentAsync(departmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(records);
        _compRepoMock.Setup(r => r.AddAsync(It.IsAny<CompensationRecord>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _compRepoMock.Setup(r => r.UpdateAsync(It.IsAny<CompensationRecord>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _bulkJobRepoMock.Setup(r => r.AddAsync(It.IsAny<BulkJob>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _bulkJobRepoMock.Setup(r => r.UpdateAsync(It.IsAny<BulkJob>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _historyRepoMock.Setup(r => r.AddAsync(It.IsAny<SalaryHistory>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new BulkSalaryIncreaseCommand
        {
            DepartmentId = departmentId,
            PercentageIncrease = 10,
            EffectiveDate = DateTime.UtcNow.AddDays(30),
            Reason = "Annual review",
            InitiatedByUserId = changedBy,
            PreviewOnly = false
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.TotalEmployeesProcessed);
        _compRepoMock.Verify(r => r.AddAsync(It.IsAny<CompensationRecord>(), It.IsAny<CancellationToken>()), Times.Once);
        _historyRepoMock.Verify(r => r.AddAsync(It.IsAny<SalaryHistory>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldHandleNoEmployees_WhenNoRecordsFound()
    {
        var departmentId = Guid.NewGuid();

        _compRepoMock.Setup(r => r.GetAllCurrentAsync(departmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CompensationRecord>());
        _bulkJobRepoMock.Setup(r => r.AddAsync(It.IsAny<BulkJob>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _bulkJobRepoMock.Setup(r => r.UpdateAsync(It.IsAny<BulkJob>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new BulkSalaryIncreaseCommand
        {
            DepartmentId = departmentId,
            PercentageIncrease = 10,
            EffectiveDate = DateTime.UtcNow.AddDays(30),
            Reason = "Annual review",
            InitiatedByUserId = Guid.NewGuid(),
            PreviewOnly = false
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(0, result.TotalEmployeesProcessed);
    }
}
