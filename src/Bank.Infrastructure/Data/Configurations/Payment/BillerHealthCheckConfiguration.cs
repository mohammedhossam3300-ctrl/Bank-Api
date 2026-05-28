using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for BillerHealthCheck entity
/// </summary>
public class BillerHealthCheckConfiguration : IEntityTypeConfiguration<BillerHealthCheck>
{
    public void Configure(EntityTypeBuilder<BillerHealthCheck> builder)
    {
        // Table name
        builder.ToTable("BillerHealthChecks");

        // Indexes for performance
        builder.HasIndex(bhc => bhc.BillerId)
            .HasDatabaseName("IX_BillerHealthChecks_BillerId");

        builder.HasIndex(bhc => bhc.CheckDate)
            .HasDatabaseName("IX_BillerHealthChecks_CheckDate");

        builder.HasIndex(bhc => bhc.IsHealthy)
            .HasDatabaseName("IX_BillerHealthChecks_IsHealthy");

        builder.HasIndex(bhc => new { bhc.BillerId, bhc.CheckDate })
            .HasDatabaseName("IX_BillerHealthChecks_BillerId_CheckDate");

        // String property configurations
        builder.Property(bhc => bhc.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(bhc => bhc.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(bhc => bhc.HealthMetricsJson)
            .HasColumnType("TEXT");

        // Default values
        builder.Property(bhc => bhc.IsHealthy)
            .HasDefaultValue(true);

        builder.Property(bhc => bhc.ConsecutiveFailures)
            .HasDefaultValue(0);

        // Relationships
        builder.HasOne(bhc => bhc.Biller)
            .WithMany()
            .HasForeignKey(bhc => bhc.BillerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

