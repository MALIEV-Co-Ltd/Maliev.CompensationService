using Microsoft.EntityFrameworkCore;
using Maliev.CompensationService.Application.Interfaces;

namespace Maliev.CompensationService.Infrastructure.Data;

/// <summary>
/// Database context for the Compensation Service.
/// </summary>
public class CompensationDbContext : DbContext
{
    private readonly IEncryptionService _encryptionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompensationDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    /// <param name="encryptionService">The encryption service for sensitive data.</param>
    public CompensationDbContext(
        DbContextOptions<CompensationDbContext> options,
        IEncryptionService encryptionService)
        : base(options)
    {
        _encryptionService = encryptionService;
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var decimalConverter = new EncryptionValueConverter(_encryptionService);
        var stringConverter = new StringEncryptionValueConverter(_encryptionService);

        // Manually apply configurations that need dependencies
        modelBuilder.ApplyConfiguration(new Configurations.CompensationRecordConfiguration(decimalConverter));
        modelBuilder.ApplyConfiguration(new Configurations.SalaryHistoryConfiguration(decimalConverter));
        modelBuilder.ApplyConfiguration(new Configurations.BenefitConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.BenefitsEnrollmentConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.DependentConfiguration(stringConverter));
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