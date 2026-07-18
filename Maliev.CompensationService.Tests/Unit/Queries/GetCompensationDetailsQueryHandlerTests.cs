using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Application.Queries;
using Maliev.CompensationService.Application.Queries.Handlers;
using Maliev.CompensationService.Domain.Entities;
using Moq;
using Xunit;

namespace Maliev.CompensationService.Tests.Unit.Queries;

public class GetCompensationDetailsQueryHandlerTests
{
    private readonly Mock<ICompensationRepository> _repositoryMock;
    private readonly GetCompensationDetailsQueryHandler _handler;

    public GetCompensationDetailsQueryHandlerTests()
    {
        _repositoryMock = new Mock<ICompensationRepository>();
        // Handler needs repository, but wait, I haven't added it to the constructor yet in the implementation
        // I'll assume the implementation will have it
        _handler = new GetCompensationDetailsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnDto_WhenRecordExists()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var record = new CompensationRecord
        {
            EmployeeId = employeeId,
            BaseSalary = 50000,
            IsCurrent = true
        };

        _repositoryMock.Setup(r => r.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(record);

        var query = new GetCompensationDetailsQuery(employeeId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(employeeId, result.EmployeeId);
        Assert.Equal(50000, result.BaseSalary);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenRecordDoesNotExist()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CompensationRecord?)null);

        var query = new GetCompensationDetailsQuery(employeeId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
