using Maliev.CompensationService.Application.Commands;
using Maliev.CompensationService.Application.Commands.Handlers;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Domain.Enums;
using Maliev.CompensationService.Domain.Events;
using MassTransit;
using Moq;
using Xunit;

namespace Maliev.CompensationService.Tests.Unit.Commands;

public class EnrollInBenefitCommandHandlerTests
{
    private readonly Mock<IBenefitsRepository> _repositoryMock;
    private readonly Mock<IPublishEndpoint> _publishMock;
    private readonly EnrollInBenefitCommandHandler _handler;

    public EnrollInBenefitCommandHandlerTests()
    {
        _repositoryMock = new Mock<IBenefitsRepository>();
        _publishMock = new Mock<IPublishEndpoint>();
        _handler = new EnrollInBenefitCommandHandler(_repositoryMock.Object, _publishMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateEnrollment_WhenNotAlreadyEnrolled()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var benefitId = Guid.NewGuid();
        
        _repositoryMock.Setup(r => r.GetEnrollmentsByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BenefitsEnrollment>());

        _repositoryMock.Setup(r => r.GetEnrollmentByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BenefitsEnrollment { Id = Guid.NewGuid(), EmployeeId = employeeId, BenefitId = benefitId });

        var dto = new EnrollInBenefitDto
        {
            BenefitId = benefitId,
            EnrollmentDate = DateTime.UtcNow,
            EmployeeContribution = 100,
            CoverageLevel = "Individual",
            Dependents = new List<DependentDto>()
        };

        var command = new EnrollInBenefitCommand(employeeId, dto, Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _repositoryMock.Verify(r => r.AddEnrollmentAsync(It.IsAny<BenefitsEnrollment>(), It.IsAny<CancellationToken>()), Times.Once);
        _publishMock.Verify(p => p.Publish(It.IsAny<BenefitEnrolledEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenAlreadyEnrolled()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var benefitId = Guid.NewGuid();
        
        _repositoryMock.Setup(r => r.GetEnrollmentsByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BenefitsEnrollment> 
            { 
                new BenefitsEnrollment { BenefitId = benefitId, Status = EnrollmentStatus.Active } 
            });

        var dto = new EnrollInBenefitDto { BenefitId = benefitId };
        var command = new EnrollInBenefitCommand(employeeId, dto, Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
