using Moq;
using Maliev.MessagingContracts.Contracts.Compensation;
using Maliev.MessagingContracts.Contracts.Employee;
using MassTransit;

namespace Maliev.CompensationService.Tests.Unit.Consumers;

public class EmployeeCreatedEventConsumerTests
{
    [Fact]
    public void Consumer_CanBeInstantiated()
    {
        var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<Maliev.CompensationService.Infrastructure.Consumers.EmployeeCreatedEventConsumer>>();
        var consumer = new Maliev.CompensationService.Infrastructure.Consumers.EmployeeCreatedEventConsumer(mockLogger.Object);

        Assert.NotNull(consumer);
        Assert.Contains(typeof(IConsumer<EmployeeCreatedEvent>), consumer.GetType().GetInterfaces());
        Assert.Equal("Maliev.MessagingContracts", typeof(EmployeeCreatedEvent).Assembly.GetName().Name);
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
        Assert.Contains(typeof(IConsumer<EmployeeTerminatedEvent>), consumer.GetType().GetInterfaces());
        Assert.Equal("Maliev.MessagingContracts", typeof(EmployeeTerminatedEvent).Assembly.GetName().Name);
    }
}

public class UndoArchiveCompensationConsumerTests
{
    [Fact]
    public void Consumer_UsesCanonicalCompensationUndoContract()
    {
        var interfaces = typeof(Maliev.CompensationService.Infrastructure.Consumers.UndoArchiveCompensationConsumer).GetInterfaces();

        Assert.Contains(typeof(IConsumer<UndoArchiveCompensationCommand>), interfaces);
        Assert.Equal("Maliev.MessagingContracts", typeof(UndoArchiveCompensationCommand).Assembly.GetName().Name);
    }
}
