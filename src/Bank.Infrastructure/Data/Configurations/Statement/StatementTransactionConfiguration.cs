using Bank.Domain.Entities;
using Bank.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for StatementTransaction entity
/// </summary>
public class StatementTransactionConfiguration : IEntityTypeConfiguration<StatementTransaction>
{
    public void Configure(EntityTypeBuilder<StatementTransaction> builder)
    {
        builder.ToTable("StatementTransactions");

        builder.HasIndex(st => st.StatementId)
            .HasDatabaseName("IX_StatementTransactions_StatementId");

        builder.HasIndex(st => st.TransactionId)
            .HasDatabaseName("IX_StatementTransactions_TransactionId");

        builder.HasIndex(st => st.TransactionDate)
            .HasDatabaseName("IX_StatementTransactions_TransactionDate");

        builder.HasIndex(st => st.Type)
            .HasDatabaseName("IX_StatementTransactions_Type");

        builder.HasIndex(st => st.Category)
            .HasDatabaseName("IX_StatementTransactions_Category");

        builder.HasIndex(st => new { st.StatementId, st.TransactionDate })
            .HasDatabaseName("IX_StatementTransactions_StatementId_TransactionDate");

        builder.Property(st => st.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(st => st.Reference)
            .HasMaxLength(100);

        builder.Property(st => st.Category)
            .HasMaxLength(100);

        builder.Property(st => st.Memo)
            .HasMaxLength(200);

        builder.Property(st => st.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(st => st.RunningBalance)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(st => st.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(st => st.Statement)
            .WithMany(s => s.Transactions)
            .HasForeignKey(st => st.StatementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(st => st.Transaction)
            .WithMany()
            .HasForeignKey(st => st.TransactionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
