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
    /// <param name="encryptionService">The encryption service.</param>
    public CompensationDbContext(DbContextOptions<CompensationDbContext> options, IEncryptionService encryptionService)
        : base(options)
    {
        _encryptionService = encryptionService;
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var decimalConverter = new DecimalEncryptionValueConverter(_encryptionService);
        var stringConverter = new EncryptionValueConverter(_encryptionService);

        // Manually apply configurations
        modelBuilder.ApplyConfiguration(new Configurations.CompensationRecordConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.SalaryHistoryConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.BenefitConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.BenefitsEnrollmentConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.DependentConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.BulkJobConfiguration());

        // Apply encryption converters to sensitive fields
        modelBuilder.Entity<Domain.Entities.CompensationRecord>()
            .Property(e => e.BaseSalary)
            .HasConversion(decimalConverter);

        modelBuilder.Entity<Domain.Entities.SalaryHistory>(entity =>
        {
            entity.Property(e => e.PreviousSalary).HasConversion(decimalConverter);
            entity.Property(e => e.NewSalary).HasConversion(decimalConverter);
        });

        modelBuilder.Entity<Domain.Entities.Dependent>()
            .Property(e => e.NationalId)
            .HasConversion(stringConverter);
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