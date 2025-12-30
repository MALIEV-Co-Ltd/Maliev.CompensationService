using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.CompensationService.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="BenefitsEnrollment"/> entity
/// </summary>
public class BenefitsEnrollmentConfiguration : IEntityTypeConfiguration<BenefitsEnrollment>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<BenefitsEnrollment> builder)
    {
        builder.ToTable("benefits_enrollments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.EmployeeId)
            .HasColumnName("employee_id")
            .IsRequired();

        builder.Property(e => e.BenefitId)
            .HasColumnName("benefit_id")
            .IsRequired();

        builder.Property(e => e.EnrollmentDate)
            .HasColumnName("enrollment_date")
            .IsRequired();

        builder.Property(e => e.TerminationDate)
            .HasColumnName("termination_date");

        builder.Property(e => e.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasDefaultValue(EnrollmentStatus.Active);

        builder.Property(e => e.EmployeeContribution)
            .HasColumnName("employee_contribution");

        builder.Property(e => e.CoverageLevel)
            .HasColumnName("coverage_level")
            .HasMaxLength(50);

        builder.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .IsRequired();

        builder.Property(e => e.ModifiedDate)
            .HasColumnName("modified_date");

        builder.HasOne(e => e.Benefit)
            .WithMany(b => b.Enrollments)
            .HasForeignKey(e => e.BenefitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.EmployeeId)
            .HasDatabaseName("idx_benefits_enroll_employee");

        builder.HasIndex(e => new { e.EmployeeId, e.Status })
            .HasDatabaseName("idx_benefits_enroll_status");

        builder.HasIndex(e => e.BenefitId)
            .HasDatabaseName("idx_benefits_enroll_benefit");

        builder.HasIndex(e => new { e.EmployeeId, e.BenefitId, e.Status })
            .IsUnique()
            .HasFilter("status = 0"); // Only one active enrollment per benefit
    }
}
