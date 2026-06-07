using Bank.Domain.Entities;
using Card = Bank.Domain.Entities.Card;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Card entity
/// </summary>
public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.ToTable("Cards");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CardNumber)
            .IsRequired()
            .HasMaxLength(19)
            .HasComment("Encrypted card number");

        builder.Property(c => c.MaskedCardNumber)
            .IsRequired()
            .HasMaxLength(19)
            .HasComment("Masked card number for display");

        builder.Property(c => c.SecurityCode)
            .IsRequired()
            .HasMaxLength(255)
            .HasComment("Encrypted security code");

        builder.Property(c => c.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.ExpiryDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(c => c.IssueDate)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(c => c.ActivationChannel)
            .HasConversion<int?>();

        builder.Property(c => c.DailyLimit)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(5000m);

        builder.Property(c => c.MonthlyLimit)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(50000m);

        builder.Property(c => c.AtmDailyLimit)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(2000m);

        builder.Property(c => c.ContactlessEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.OnlineTransactionsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.InternationalTransactionsEnabled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.PinHash)
            .HasMaxLength(255);

        builder.Property(c => c.FailedPinAttempts)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(c => c.LastBlockReason)
            .HasConversion<int?>();

        builder.Property(c => c.BlockedMerchantCategories)
            .HasMaxLength(1000)
            .HasComment("JSON array of blocked merchant categories");

        builder.Property(c => c.CardName)
            .HasMaxLength(50);

        // Relationships
        builder.HasOne(c => c.Customer)
            .WithMany()
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Account)
            .WithMany()
            .HasForeignKey(c => c.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Transactions)
            .WithOne(t => t.Card)
            .HasForeignKey(t => t.CardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.StatusHistory)
            .WithOne(h => h.Card)
            .HasForeignKey(h => h.CardId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(c => c.CardNumber)
            .IsUnique()
            .HasDatabaseName("IX_Cards_CardNumber");

        builder.HasIndex(c => c.CustomerId)
            .HasDatabaseName("IX_Cards_CustomerId");

        builder.HasIndex(c => c.AccountId)
            .HasDatabaseName("IX_Cards_AccountId");

        builder.HasIndex(c => c.Status)
            .HasDatabaseName("IX_Cards_Status");

        builder.HasIndex(c => c.ExpiryDate)
            .HasDatabaseName("IX_Cards_ExpiryDate");

        builder.HasIndex(c => new { c.CustomerId, c.Status })
            .HasDatabaseName("IX_Cards_CustomerId_Status");
    }
}

