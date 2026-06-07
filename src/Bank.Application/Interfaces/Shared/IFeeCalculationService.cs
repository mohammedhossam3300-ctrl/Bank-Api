using Bank.Domain.Entities;
using Bank.Domain.Enums;

namespace Bank.Application.Interfaces;

public interface IFeeCalculationService
{
    Task<decimal> CalculateMaintenanceFeeAsync(Account account, DateTime fromDate, DateTime toDate);
    Task<decimal> CalculateOverdraftFeeAsync(Account account, decimal overdraftAmount);
    Task<decimal> CalculateInactivityFeeAsync(Account account, int daysSinceLastActivity);
    Task<decimal> CalculateEarlyClosureFeeAsync(Account account);
    Task<decimal> CalculateMinimumBalanceFeeAsync(Account account, decimal minimumBalance);
    Task<FeeSchedule> GetFeeScheduleAsync(AccountType accountType);
    Task<List<AccountFee>> GetPendingFeesAsync(Guid accountId);
    Task<bool> WaiveFeeAsync(Guid feeId, string reason, Guid userId);
    Task<decimal> CalculateTotalFeesAsync(Guid accountId, DateTime fromDate, DateTime toDate);
}
