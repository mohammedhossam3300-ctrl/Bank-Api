using Bank.Domain.Entities;
using Card = Bank.Domain.Entities.Card;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for CardStatusHistory entity
/// </summary>
public class CardStatusHistoryConfiguration : IEntityTypeConfiguration<CardStatusHistory>
{
    public void Configure(EntityTypeBuilder<CardStatusHistory> builder)
    {
        builder.ToTable("CardStatusHistories");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.PreviousStatus)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(h => h.NewStatus)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(h => h.Reason)
            .HasMaxLength(200);

        builder.Property(h => h.ChangeDate)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(h => h.Notes)
            .HasMaxLength(500);

        builder.Property(h => h.Channel)
            .HasMaxLength(50);

        builder.Property(h => h.IpAddress)
            .HasMaxLength(45);

        // Relationships
        builder.HasOne(h => h.Card)
            .WithMany(c => c.StatusHistory)
            .HasForeignKey(h => h.CardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(h => h.ChangedByUser)
            .WithMany()
            .HasForeignKey(h => h.ChangedBy)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(h => h.CardId)
            .HasDatabaseName("IX_CardStatusHistories_CardId");

        builder.HasIndex(h => h.ChangeDate)
            .HasDatabaseName("IX_CardStatusHistories_ChangeDate");

        builder.HasIndex(h => h.ChangedBy)
            .HasDatabaseName("IX_CardStatusHistories_ChangedBy");

        builder.HasIndex(h => new { h.CardId, h.ChangeDate })
            .HasDatabaseName("IX_CardStatusHistories_CardId_ChangeDate");
    }
}

