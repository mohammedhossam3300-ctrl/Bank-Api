using Bank.Domain.Entities;
using Card = Bank.Domain.Entities.Card;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for CardTransaction entity
/// </summary>
public class CardTransactionConfiguration : IEntityTypeConfiguration<CardTransaction>
{
    public void Configure(EntityTypeBuilder<CardTransaction> builder)
    {
        builder.ToTable("CardTransactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.AuthorizationCode)
            .HasMaxLength(20);

        builder.Property(t => t.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        builder.Property(t => t.TransactionType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.TransactionDate)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(t => t.SettlementDate);

        builder.Property(t => t.MerchantName)
            .HasMaxLength(100);

        builder.Property(t => t.MerchantId)
            .HasMaxLength(50);

        builder.Property(t => t.MerchantCategory)
            .HasConversion<int>();

        builder.Property(t => t.MerchantCountry)
            .HasMaxLength(50);

        builder.Property(t => t.IsContactless)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.IsOnline)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.IsInternational)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.Description)
            .HasMaxLength(200);

        builder.Property(t => t.Reference)
            .HasMaxLength(100);

        builder.Property(t => t.DeclineReason)
            .HasMaxLength(100);

        builder.Property(t => t.ProcessorResponse)
            .HasMaxLength(200);

        builder.Property(t => t.Fees)
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.FeeBreakdown)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(t => t.Card)
            .WithMany()
            .HasForeignKey(t => t.CardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.OriginalTransaction)
            .WithMany(t => t.RelatedTransactions)
            .HasForeignKey(t => t.OriginalTransactionId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(t => t.CardId)
            .HasDatabaseName("IX_CardTransactions_CardId");

        builder.HasIndex(t => t.TransactionDate)
            .HasDatabaseName("IX_CardTransactions_TransactionDate");

        builder.HasIndex(t => t.Status)
            .HasDatabaseName("IX_CardTransactions_Status");

        builder.HasIndex(t => t.TransactionType)
            .HasDatabaseName("IX_CardTransactions_TransactionType");

        builder.HasIndex(t => t.MerchantId)
            .HasDatabaseName("IX_CardTransactions_MerchantId");

        builder.HasIndex(t => t.SettlementDate)
            .HasDatabaseName("IX_CardTransactions_SettlementDate");

        builder.HasIndex(t => new { t.CardId, t.TransactionDate })
            .HasDatabaseName("IX_CardTransactions_CardId_TransactionDate");

        builder.HasIndex(t => new { t.CardId, t.Status })
            .HasDatabaseName("IX_CardTransactions_CardId_Status");

        builder.HasIndex(t => t.AuthorizationCode)
            .HasDatabaseName("IX_CardTransactions_AuthorizationCode")
            .HasFilter("\"AuthorizationCode\" IS NOT NULL");
    }
}

