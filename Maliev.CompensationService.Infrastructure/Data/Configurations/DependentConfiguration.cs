using Maliev.CompensationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.CompensationService.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="Dependent"/> entity
/// </summary>
public class DependentConfiguration : IEntityTypeConfiguration<Dependent>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Dependent> builder)
    {
        builder.ToTable("dependents");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.BenefitsEnrollmentId)
            .HasColumnName("benefits_enrollment_id")
            .IsRequired();

        builder.Property(e => e.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Relationship)
            .HasColumnName("relationship")
            .IsRequired();

        builder.Property(e => e.DateOfBirth)
            .HasColumnName("date_of_birth")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(e => e.NationalId)
            .HasColumnName("national_id");

        builder.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .IsRequired();

        builder.Property(e => e.ModifiedDate)
            .HasColumnName("modified_date");

        builder.HasOne(e => e.Enrollment)
            .WithMany(en => en.Dependents)
            .HasForeignKey(e => e.BenefitsEnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.BenefitsEnrollmentId)
            .HasDatabaseName("idx_dependents_enrollment");
    }
}
