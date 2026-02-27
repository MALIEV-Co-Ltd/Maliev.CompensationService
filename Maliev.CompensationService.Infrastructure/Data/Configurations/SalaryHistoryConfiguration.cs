using Maliev.CompensationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.CompensationService.Infrastructure.Data.Configurations;

/// <summary>
/// Entity framework configuration for the <see cref="SalaryHistory"/> entity.
/// </summary>
public class SalaryHistoryConfiguration : IEntityTypeConfiguration<SalaryHistory>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<SalaryHistory> builder)
    {
        builder.ToTable("salary_histories");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.EmployeeId)
            .HasColumnName("employee_id")
            .IsRequired();

        builder.Property(e => e.CompensationRecordId)
            .HasColumnName("compensation_record_id")
            .IsRequired();

        builder.Property(e => e.PreviousSalary)
            .HasColumnName("previous_salary")
            .IsRequired();

        builder.Property(e => e.NewSalary)
            .HasColumnName("new_salary")
            .IsRequired();

        builder.Property(e => e.ChangeAmount)
            .HasColumnName("change_amount")
            .IsRequired();

        builder.Property(e => e.ChangePercentage)
            .HasColumnName("change_percentage")
            .IsRequired();

        builder.Property(e => e.IsHighIncrease)
            .HasColumnName("is_high_increase")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.EffectiveDate)
            .HasColumnName("effective_date")
            .IsRequired();

        builder.Property(e => e.ChangeType)
            .HasColumnName("change_type")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.ChangedBy)
            .HasColumnName("changed_by")
            .IsRequired();

        builder.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .IsRequired();

        builder.HasOne(e => e.CompensationRecord)
            .WithMany(r => r.SalaryHistories)
            .HasForeignKey(e => e.CompensationRecordId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.EmployeeId, e.EffectiveDate })
            .HasDatabaseName("idx_salary_hist_employee");
    }
}
