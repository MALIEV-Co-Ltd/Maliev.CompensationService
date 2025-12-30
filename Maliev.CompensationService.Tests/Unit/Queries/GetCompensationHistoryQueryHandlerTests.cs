using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Application.Queries;
using Maliev.CompensationService.Application.Queries.Handlers;
using Maliev.CompensationService.Domain.Entities;
using Moq;
using Xunit;

namespace Maliev.CompensationService.Tests.Unit.Queries;

public class GetCompensationHistoryQueryHandlerTests
{
    private readonly Mock<ISalaryHistoryRepository> _repositoryMock;
    private readonly GetCompensationHistoryQueryHandler _handler;

    public GetCompensationHistoryQueryHandlerTests()
    {
        _repositoryMock = new Mock<ISalaryHistoryRepository>();
        _handler = new GetCompensationHistoryQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnDtos_WhenHistoryExists()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var history = new List<SalaryHistory>
        {
            new SalaryHistory { Id = Guid.NewGuid(), EmployeeId = employeeId, NewSalary = 60000, EffectiveDate = DateTime.UtcNow },
            new SalaryHistory { Id = Guid.NewGuid(), EmployeeId = employeeId, NewSalary = 50000, EffectiveDate = DateTime.UtcNow.AddYears(-1) }
        };

        _repositoryMock.Setup(r => r.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(history);

        var query = new GetCompensationHistoryQuery(employeeId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, r => r.NewSalary == 60000);
        Assert.Contains(result, r => r.NewSalary == 50000);
    }
}
