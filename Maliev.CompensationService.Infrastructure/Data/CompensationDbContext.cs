using Microsoft.EntityFrameworkCore;

namespace Maliev.CompensationService.Infrastructure.Data;

/// <summary>
/// Database context for the Compensation Service.
/// </summary>
public class CompensationDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompensationDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public CompensationDbContext(DbContextOptions<CompensationDbContext> options)
        : base(options)
    {
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Manually apply configurations
        modelBuilder.ApplyConfiguration(new Configurations.CompensationRecordConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.SalaryHistoryConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.BenefitConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.BenefitsEnrollmentConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.DependentConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.BulkJobConfiguration());
    }

    /// <inheritdoc/>
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        // Configure default string length
        configurationBuilder.Properties<string>()
            .HaveMaxLength(500);
    }
}
