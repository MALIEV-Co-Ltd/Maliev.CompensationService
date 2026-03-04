using Moq;

namespace Maliev.CompensationService.Tests.Unit.Consumers;

public class EmployeeCreatedEventConsumerTests
{
    [Fact]
    public void Consumer_CanBeInstantiated()
    {
        var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<Maliev.CompensationService.Infrastructure.Consumers.EmployeeCreatedEventConsumer>>();
        var consumer = new Maliev.CompensationService.Infrastructure.Consumers.EmployeeCreatedEventConsumer(mockLogger.Object);
        
        Assert.NotNull(consumer);
    }
}

public class EmployeeTerminatedEventConsumerTests
{
    [Fact]
    public void Consumer_CanBeInstantiated()
    {
        var mockBenefitsRepo = new Mock<Maliev.CompensationService.Application.Interfaces.IBenefitsRepository>();
        var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<Maliev.CompensationService.Infrastructure.Consumers.EmployeeTerminatedEventConsumer>>();
        var consumer = new Maliev.CompensationService.Infrastructure.Consumers.EmployeeTerminatedEventConsumer(mockBenefitsRepo.Object, mockLogger.Object);
        
        Assert.NotNull(consumer);
    }
}
