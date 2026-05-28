using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for PaymentReceipt entity
/// </summary>
public class PaymentReceiptConfiguration : IEntityTypeConfiguration<PaymentReceipt>
{
    public void Configure(EntityTypeBuilder<PaymentReceipt> builder)
    {
        // Table name
        builder.ToTable("PaymentReceipts");

        // Indexes for performance
        builder.HasIndex(pr => pr.PaymentId)
            .IsUnique()
            .HasDatabaseName("IX_PaymentReceipts_PaymentId");

        builder.HasIndex(pr => pr.ReceiptNumber)
            .IsUnique()
            .HasDatabaseName("IX_PaymentReceipts_ReceiptNumber");

        builder.HasIndex(pr => pr.CustomerId)
            .HasDatabaseName("IX_PaymentReceipts_CustomerId");

        builder.HasIndex(pr => pr.ConfirmationNumber)
            .HasDatabaseName("IX_PaymentReceipts_ConfirmationNumber");

        builder.HasIndex(pr => pr.Status)
            .HasDatabaseName("IX_PaymentReceipts_Status");

        // String property configurations
        builder.Property(pr => pr.ReceiptNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pr => pr.CustomerName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pr => pr.BillerName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pr => pr.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        builder.Property(pr => pr.ConfirmationNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pr => pr.Reference)
            .HasMaxLength(100);

        builder.Property(pr => pr.ReceiptDataJson)
            .HasColumnType("TEXT");

        // Decimal configurations
        builder.Property(pr => pr.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(pr => pr.ProcessingFee)
            .HasPrecision(18, 2);

        // Enum configurations
        builder.Property(pr => pr.PaymentMethod)
            .HasConversion<int>();

        builder.Property(pr => pr.Status)
            .HasConversion<int>();

        // Relationships
        builder.HasOne(pr => pr.Payment)
            .WithMany()
            .HasForeignKey(pr => pr.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pr => pr.Customer)
            .WithMany()
            .HasForeignKey(pr => pr.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

