using Microsoft.AspNetCore.Mvc;

namespace Maliev.CompensationService.Api.Controllers;

/// <summary>
/// Base controller for shared functionality
/// </summary>
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Retrieves the unique identifier of the currently authenticated user
    /// </summary>
    /// <returns>The user identifier, or <see cref="Guid.Empty"/> if not authenticated</returns>
    protected Guid GetCurrentUserId()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var subClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(subClaim, out var userId))
            {
                return userId;
            }
        }
        return Guid.Empty;
    }
}