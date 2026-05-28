using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for PaymentRetry entity
/// </summary>
public class PaymentRetryConfiguration : IEntityTypeConfiguration<PaymentRetry>
{
    public void Configure(EntityTypeBuilder<PaymentRetry> builder)
    {
        // Table name
        builder.ToTable("PaymentRetries");

        // Indexes for performance
        builder.HasIndex(pr => pr.PaymentId)
            .HasDatabaseName("IX_PaymentRetries_PaymentId");

        builder.HasIndex(pr => pr.NextRetryDate)
            .HasDatabaseName("IX_PaymentRetries_NextRetryDate");

        builder.HasIndex(pr => pr.Status)
            .HasDatabaseName("IX_PaymentRetries_Status");

        builder.HasIndex(pr => pr.IsMaxRetriesReached)
            .HasDatabaseName("IX_PaymentRetries_IsMaxRetriesReached");

        builder.HasIndex(pr => new { pr.PaymentId, pr.AttemptNumber })
            .IsUnique()
            .HasDatabaseName("IX_PaymentRetries_PaymentId_AttemptNumber");

        // String property configurations
        builder.Property(pr => pr.FailureReason)
            .HasMaxLength(1000);

        builder.Property(pr => pr.RetryMetadataJson)
            .HasColumnType("TEXT");

        // Enum configurations
        builder.Property(pr => pr.Status)
            .HasConversion<int>();

        // Default values
        builder.Property(pr => pr.IsMaxRetriesReached)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(pr => pr.Payment)
            .WithMany()
            .HasForeignKey(pr => pr.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

