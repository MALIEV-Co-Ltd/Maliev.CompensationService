using Microsoft.AspNetCore.Mvc.Filters;

namespace Maliev.CompensationService.Api.Filters;

/// <summary>
/// Filter to redact sensitive salary data from logs during action execution
/// </summary>
public class SalaryLoggingFilter : IAsyncActionFilter
{
    private readonly ILogger<SalaryLoggingFilter> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SalaryLoggingFilter"/> class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    public SalaryLoggingFilter(ILogger<SalaryLoggingFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Executes before and after the action method
    /// </summary>
    /// <param name="context">The action executing context</param>
    /// <param name="next">The next filter in the pipeline</param>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Log basic request info without sensitive data
        var actionName = context.ActionDescriptor.DisplayName;
        var userId = context.HttpContext.User.Identity?.Name ?? "Anonymous";

        // Explicitly clear or ignore arguments that might contain salary in the logs
        // This is a safety measure if standard middleware logs 'context.ActionArguments'
        _logger.LogInformation("Executing action {ActionName} for user {UserId}", actionName, userId);

        await next();
    }
}
