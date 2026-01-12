using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Maliev.CompensationService.Domain.Authorization;

namespace Maliev.CompensationService.Tests.Integration.Controllers;

public class TestAuthHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, System.Guid.NewGuid().ToString()),
            new Claim("permission", CompensationPermissions.Read),
            new Claim("permission", CompensationPermissions.ReadSensitive),
            new Claim("permission", CompensationPermissions.Update),
            new Claim("permission", CompensationPermissions.Admin),
            new Claim("permission", CompensationPermissions.Reports)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = context.Resource as HttpContext ?? (context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext)?.HttpContext;
        if (httpContext != null)
        {
            httpContext.User = principal;
        }

        foreach (var requirement in context.PendingRequirements.ToList())
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}
