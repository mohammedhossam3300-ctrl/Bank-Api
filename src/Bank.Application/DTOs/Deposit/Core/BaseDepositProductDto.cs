using Bank.Domain.Enums;

namespace Bank.Application.DTOs.Deposit.Core;

/// <summary>
/// Base deposit product properties shared across DTOs and requests
/// </summary>
public abstract class BaseDepositProductDto
{
    // Core Product Information
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DepositProductType ProductType { get; set; }

    // Term Configuration
    public int? MinimumTermDays { get; set; }
    public int? MaximumTermDays { get; set; }
    public int? DefaultTermDays { get; set; }

    // Balance Limits
    public decimal MinimumBalance { get; set; }
    public decimal? MaximumBalance { get; set; }
    public decimal MinimumOpeningBalance { get; set; }

    // Interest Settings
    public decimal BaseInterestRate { get; set; }
    public InterestCalculationMethod InterestCalculationMethod { get; set; }
    public InterestCompoundingFrequency CompoundingFrequency { get; set; }
    public bool HasTieredRates { get; set; }

    // Withdrawal & Penalties
    public bool AllowPartialWithdrawals { get; set; }
    public WithdrawalPenaltyType PenaltyType { get; set; }
    public decimal? PenaltyAmount { get; set; }
    public decimal? PenaltyPercentage { get; set; }
    public int? PenaltyFreeDays { get; set; }

    // Maturity Settings
    public MaturityAction DefaultMaturityAction { get; set; }
    public bool AllowAutoRenewal { get; set; }
    public int? AutoRenewalNoticeDays { get; set; }

    // Promotional Rates
    public DateTime? PromotionalRateStartDate { get; set; }
    public DateTime? PromotionalRateEndDate { get; set; }
    public decimal? PromotionalRate { get; set; }
}
