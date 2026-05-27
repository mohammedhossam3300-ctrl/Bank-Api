using Bank.Domain.Entities;
using Bank.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for AccountStatement entity
/// </summary>
public class AccountStatementConfiguration : IEntityTypeConfiguration<AccountStatement>
{
    public void Configure(EntityTypeBuilder<AccountStatement> builder)
    {
        builder.ToTable("AccountStatements");

        builder.HasIndex(s => s.AccountId)
            .HasDatabaseName("IX_AccountStatements_AccountId");

        builder.HasIndex(s => s.StatementDate)
            .HasDatabaseName("IX_AccountStatements_StatementDate");

        builder.HasIndex(s => s.Status)
            .HasDatabaseName("IX_AccountStatements_Status");

        builder.HasIndex(s => new { s.AccountId, s.StatementDate })
            .HasDatabaseName("IX_AccountStatements_AccountId_StatementDate");

        builder.HasIndex(s => new { s.AccountId, s.Status })
            .HasDatabaseName("IX_AccountStatements_AccountId_Status");

        builder.HasIndex(s => s.PeriodStartDate)
            .HasDatabaseName("IX_AccountStatements_PeriodStartDate");

        builder.HasIndex(s => s.PeriodEndDate)
            .HasDatabaseName("IX_AccountStatements_PeriodEndDate");

        builder.Property(s => s.FilePath)
            .HasMaxLength(500);

        builder.Property(s => s.TotalDebits)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(s => s.TotalCredits)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(s => s.OpeningBalance)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(s => s.ClosingBalance)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(s => s.Status)
            .HasConversion<int>()
            .HasDefaultValue(StatementStatus.Generated);

        builder.Property(s => s.Format)
            .HasConversion<int>()
            .HasDefaultValue(StatementFormat.PDF);

        builder.HasOne(s => s.Account)
            .WithMany()
            .HasForeignKey(s => s.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Transactions)
            .WithOne(t => t.Statement)
            .HasForeignKey(t => t.StatementId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
