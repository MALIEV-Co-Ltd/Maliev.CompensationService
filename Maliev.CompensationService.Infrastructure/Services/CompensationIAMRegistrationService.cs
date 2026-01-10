using Maliev.Aspire.ServiceDefaults.IAM;
using Maliev.CompensationService.Domain.Authorization;
using Microsoft.Extensions.Configuration;
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
    /// <param name="configuration">The application configuration.</param>
    /// <param name="logger">Logger instance.</param>
    public CompensationIAMRegistrationService(
        IConfiguration configuration,
        ILogger<CompensationIAMRegistrationService> logger)
        : base(configuration, logger, "compensation")
    {
    }

    /// <inheritdoc />
    protected override IEnumerable<PermissionRegistration> GetPermissions()
    {
        return CompensationPermissions.All.Select(p => new PermissionRegistration
        {
            PermissionId = p.Key,
            Description = p.Value
        });
    }

    /// <inheritdoc />
    protected override IEnumerable<RoleRegistration> GetPredefinedRoles()
    {
        return Enumerable.Empty<RoleRegistration>();
    }
}

