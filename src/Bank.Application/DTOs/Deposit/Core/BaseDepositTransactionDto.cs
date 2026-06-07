using Bank.Domain.Enums;

namespace Bank.Application.DTOs.Deposit.Core;

/// <summary>
/// Base deposit transaction properties shared across DTOs
/// </summary>
public abstract class BaseDepositTransactionDto
{
    // Core Transaction Information
    public string TransactionReference { get; set; } = string.Empty;
    public DepositTransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public TransactionStatus Status { get; set; }

    // Interest Period Information
    public DateTime? InterestPeriodStart { get; set; }
    public DateTime? InterestPeriodEnd { get; set; }
    public decimal? InterestRate { get; set; }
    public int? InterestDays { get; set; }

    // Penalty Information
    public WithdrawalPenaltyType? PenaltyType { get; set; }
    public string? PenaltyReason { get; set; }
}
