using Maliev.CompensationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.CompensationService.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="Benefit"/> entity
/// </summary>
public class BenefitConfiguration : IEntityTypeConfiguration<Benefit>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Benefit> builder)
    {
        builder.ToTable("benefits");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasColumnName("description");

        builder.Property(e => e.BenefitType)
            .HasColumnName("benefit_type")
            .IsRequired();

        builder.Property(e => e.EmployerContribution)
            .HasColumnName("employer_contribution");

        builder.Property(e => e.EmployeeContribution)
            .HasColumnName("employee_contribution");

        builder.Property(e => e.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .IsRequired();

        builder.Property(e => e.ModifiedDate)
            .HasColumnName("modified_date");

        builder.HasIndex(e => e.IsActive)
            .HasDatabaseName("idx_benefits_active")
            .HasFilter("is_active = true");

        builder.HasIndex(e => e.BenefitType)
            .HasDatabaseName("idx_benefits_type");
    }
}
