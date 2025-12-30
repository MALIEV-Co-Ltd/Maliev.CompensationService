using Maliev.CompensationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.CompensationService.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="BulkJob"/> entity
/// </summary>
public class BulkJobConfiguration : IEntityTypeConfiguration<BulkJob>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<BulkJob> builder)
    {
        builder.ToTable("bulk_jobs");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.JobType)
            .HasColumnName("job_type")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(e => e.Parameters)
            .HasColumnName("parameters")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(e => e.SuccessCount)
            .HasColumnName("success_count")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(e => e.FailureCount)
            .HasColumnName("failure_count")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(e => e.ErrorDetails)
            .HasColumnName("error_details")
            .HasColumnType("jsonb");

        builder.Property(e => e.StartedBy)
            .HasColumnName("started_by")
            .IsRequired();

        builder.Property(e => e.StartedAt)
            .HasColumnName("started_at")
            .IsRequired();

        builder.Property(e => e.CompletedAt)
            .HasColumnName("completed_at");

        builder.Property(e => e.WebhookUrl)
            .HasColumnName("webhook_url");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("idx_bulk_jobs_status");

        builder.HasIndex(e => e.StartedBy)
            .HasDatabaseName("idx_bulk_jobs_started_by");

        builder.HasIndex(e => e.StartedAt)
            .HasDatabaseName("idx_bulk_jobs_started_at");
    }
}
