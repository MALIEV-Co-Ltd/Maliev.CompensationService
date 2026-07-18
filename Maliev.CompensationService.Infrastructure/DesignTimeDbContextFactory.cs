using Maliev.CompensationService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Maliev.CompensationService.Infrastructure;

/// <summary>
/// Design-time factory for EF Core migrations.
/// Uses environment variable CompensationDbContext for connection string.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CompensationDbContext>
{
    public CompensationDbContext CreateDbContext(string[] args)
    {
        // Prefer environment variable for connection string, fallback to design-time default if not set
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__CompensationDbContext")
            ?? "Host=localhost;Database=compensation_design;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<CompensationDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new CompensationDbContext(optionsBuilder.Options);
    }
}
