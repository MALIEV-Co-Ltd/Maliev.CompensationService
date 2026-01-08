using Maliev.Aspire.ServiceDefaults.IAM;
using Maliev.CompensationService.Domain.Authorization;
using Microsoft.Extensions.Logging;

namespace Maliev.CompensationService.Infrastructure.Services;

/// <summary>
/// Service to register compensation permissions with the central IAM service on startup
/// </summary>
public class CompensationIAMRegistrationService : IAMRegistrationService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompensationIAMRegistrationService"/> class
    /// </summary>
    /// <param name="httpClientFactory">Factory for creating HTTP clients.</param>
    /// <param name="tokenProvider">Service account token provider.</param>
    /// <param name="logger">Logger instance.</param>
    public CompensationIAMRegistrationService(IHttpClientFactory httpClientFactory, IServiceAccountTokenProvider tokenProvider, ILogger<CompensationIAMRegistrationService> logger)
        : base(httpClientFactory, tokenProvider, logger, "compensation")
    {
    }

    /// <inheritdoc />
    protected override IEnumerable<PermissionRegistration> GetPermissions()
    {
        return new[]
        {
            new PermissionRegistration { PermissionId = CompensationPermissions.Read, Description = "Read employee compensation and history" },
            new PermissionRegistration { PermissionId = CompensationPermissions.Update, Description = "Update employee compensation and benefits" },
            new PermissionRegistration { PermissionId = CompensationPermissions.Admin, Description = "Perform administrative bulk operations" },
            new PermissionRegistration { PermissionId = CompensationPermissions.Reports, Description = "Generate compensation and budget reports" }
        };
    }

    /// <inheritdoc />
    protected override IEnumerable<RoleRegistration> GetPredefinedRoles()
    {
        return Enumerable.Empty<RoleRegistration>();
    }
}