using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.CompensationService.Infrastructure.Data.Configurations;

/// <summary>
/// Entity framework configuration for the <see cref="CompensationRecord"/> entity.
/// </summary>
public class CompensationRecordConfiguration : IEntityTypeConfiguration<CompensationRecord>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<CompensationRecord> builder)
    {
        builder.ToTable("compensation_records");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.EmployeeId)
            .HasColumnName("employee_id")
            .IsRequired();

        builder.Property(e => e.DepartmentId)
            .HasColumnName("department_id")
            .IsRequired();

        builder.Property(e => e.EffectiveDate)
            .HasColumnName("effective_date")
            .IsRequired();

        builder.Property(e => e.BaseSalary)
            .HasColumnName("base_salary")
            .HasColumnType("character varying")
            .IsRequired();

        builder.Property(e => e.Currency)
            .HasColumnName("currency")
            .HasMaxLength(3)
            .IsRequired()
            .HasDefaultValue("USD");

        builder.Property(e => e.CompensationType)
            .HasColumnName("compensation_type")
            .IsRequired();

        builder.Property(e => e.BonusPercentage)
            .HasColumnName("bonus_percentage");

        builder.Property(e => e.CommissionRate)
            .HasColumnName("commission_rate");

        builder.Property(e => e.ChangeReason)
            .HasColumnName("change_reason")
            .HasMaxLength(500);

        builder.Property(e => e.ApprovedBy)
            .HasColumnName("approved_by");

        builder.Property(e => e.IsCurrent)
            .HasColumnName("is_current")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .IsRequired();

        builder.Property(e => e.ModifiedDate)
            .HasColumnName("modified_date");

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.HasIndex(e => e.EmployeeId)
            .HasDatabaseName("idx_comp_records_employee");

        builder.HasIndex(e => new { e.EmployeeId, e.IsCurrent })
            .HasDatabaseName("idx_comp_records_current")
            .IsUnique()
            .HasFilter("is_current = true");
    }
}
