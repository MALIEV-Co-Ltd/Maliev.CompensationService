using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Application.Queries;
using Maliev.CompensationService.Application.Queries.Handlers;
using Maliev.CompensationService.Domain.Entities;
using Moq;
using Xunit;

namespace Maliev.CompensationService.Tests.Unit.Queries;

public class GetEmployeeBenefitsQueryHandlerTests
{
    private readonly Mock<IBenefitsRepository> _repositoryMock;
    private readonly GetEmployeeBenefitsQueryHandler _handler;

    public GetEmployeeBenefitsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IBenefitsRepository>();
        _handler = new GetEmployeeBenefitsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnDtos_WhenEnrollmentsExist()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var enrollments = new List<BenefitsEnrollment>
        {
            new BenefitsEnrollment { Id = Guid.NewGuid(), EmployeeId = employeeId, Benefit = new Benefit { Name = "B1" } },
            new BenefitsEnrollment { Id = Guid.NewGuid(), EmployeeId = employeeId, Benefit = new Benefit { Name = "B2" } }
        };

        _repositoryMock.Setup(r => r.GetEnrollmentsByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(enrollments);

        var query = new GetEmployeeBenefitsQuery(employeeId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count());
        _repositoryMock.Verify(r => r.GetEnrollmentsByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
