using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Application.Queries;
using Maliev.CompensationService.Application.Queries.Handlers;
using Maliev.CompensationService.Domain.Entities;
using Moq;
using Xunit;

namespace Maliev.CompensationService.Tests.Unit.Queries;

public class GetCompensationAnalysisQueryHandlerTests
{
    private readonly Mock<ICompensationRepository> _repositoryMock;
    private readonly GetCompensationAnalysisQueryHandler _handler;

    public GetCompensationAnalysisQueryHandlerTests()
    {
        _repositoryMock = new Mock<ICompensationRepository>();
        _handler = new GetCompensationAnalysisQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnCalculatedStats_WhenRecordsExist()
    {
        // Arrange
        var records = new List<CompensationRecord>
        {
            new CompensationRecord { BaseSalary = 1000 },
            new CompensationRecord { BaseSalary = 2000 },
            new CompensationRecord { BaseSalary = 3000 }
        };

        _repositoryMock.Setup(r => r.GetAllCurrentAsync(It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(records);

        var query = new GetCompensationAnalysisQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.TotalEmployees);
        Assert.Equal(6000, result.TotalAnnualBudget);
        Assert.Equal(2000, result.AverageSalary);
        Assert.Equal(1000, result.MinimumSalary);
        Assert.Equal(3000, result.MaximumSalary);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoRecordsExist()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllCurrentAsync(It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CompensationRecord>());

        var query = new GetCompensationAnalysisQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(0, result.TotalEmployees);
        Assert.Equal(0, result.TotalAnnualBudget);
    }
}
