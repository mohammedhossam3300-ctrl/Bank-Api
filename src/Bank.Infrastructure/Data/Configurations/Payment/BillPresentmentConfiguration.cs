using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for BillPresentment entity
/// </summary>
public class BillPresentmentConfiguration : IEntityTypeConfiguration<BillPresentment>
{
    public void Configure(EntityTypeBuilder<BillPresentment> builder)
    {
        // Table name
        builder.ToTable("BillPresentments");

        // Indexes for performance
        builder.HasIndex(bp => bp.CustomerId)
            .HasDatabaseName("IX_BillPresentments_CustomerId");

        builder.HasIndex(bp => bp.BillerId)
            .HasDatabaseName("IX_BillPresentments_BillerId");

        builder.HasIndex(bp => bp.Status)
            .HasDatabaseName("IX_BillPresentments_Status");

        builder.HasIndex(bp => bp.DueDate)
            .HasDatabaseName("IX_BillPresentments_DueDate");

        builder.HasIndex(bp => bp.ExternalBillId)
            .IsUnique()
            .HasDatabaseName("IX_BillPresentments_ExternalBillId");

        builder.HasIndex(bp => new { bp.CustomerId, bp.Status })
            .HasDatabaseName("IX_BillPresentments_CustomerId_Status");

        // String property configurations
        builder.Property(bp => bp.AccountNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(bp => bp.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        builder.Property(bp => bp.BillNumber)
            .HasMaxLength(100);

        builder.Property(bp => bp.ExternalBillId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(bp => bp.LineItemsJson)
            .HasColumnType("TEXT");

        // Decimal configurations
        builder.Property(bp => bp.AmountDue)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(bp => bp.MinimumPayment)
            .IsRequired()
            .HasPrecision(18, 2);

        // Enum configurations
        builder.Property(bp => bp.Status)
            .HasConversion<int>()
            .HasDefaultValue(Bank.Domain.Enums.BillPresentmentStatus.Pending);

        // Relationships
        builder.HasOne(bp => bp.Customer)
            .WithMany()
            .HasForeignKey(bp => bp.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(bp => bp.Biller)
            .WithMany()
            .HasForeignKey(bp => bp.BillerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(bp => bp.Payment)
            .WithMany()
            .HasForeignKey(bp => bp.PaymentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

