using Maliev.CompensationService.Api.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Moq;

namespace Maliev.CompensationService.Tests.Unit.Filters;

public class SalaryLoggingFilterTests
{
    [Fact]
    public async Task OnActionExecutionAsync_LogsActionNameAndUserId()
    {
        var mockLogger = new Mock<ILogger<SalaryLoggingFilter>>();
        var filter = new SalaryLoggingFilter(mockLogger.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.User = new System.Security.Claims.ClaimsPrincipal();

        var actionContext = new ActionContext(
            httpContext,
            new Microsoft.AspNetCore.Routing.RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor
            {
                DisplayName = "TestController::TestAction"
            });

        var executingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            controller: null!);

        var nextCalled = false;
        ActionExecutionDelegate next = async () =>
        {
            nextCalled = true;
            return new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), null!);
        };

        await filter.OnActionExecutionAsync(executingContext, next);

        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Executing action")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        Assert.True(nextCalled);
    }

    [Fact]
    public async Task OnActionExecutionAsync_LogsAnonymousUser_WhenNotAuthenticated()
    {
        var mockLogger = new Mock<ILogger<SalaryLoggingFilter>>();
        var filter = new SalaryLoggingFilter(mockLogger.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.User = new System.Security.Claims.ClaimsPrincipal();

        var actionContext = new ActionContext(
            httpContext,
            new Microsoft.AspNetCore.Routing.RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor
            {
                DisplayName = "AnonymousController::AnonymousAction"
            });

        var executingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            controller: null!);

        ActionExecutionDelegate next2 = async () =>
        {
            return new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), null!);
        };

        await filter.OnActionExecutionAsync(executingContext, next2);

        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Anonymous")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
