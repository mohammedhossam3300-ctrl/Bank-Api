using Bank.Domain.Entities;

namespace Bank.Domain.Interfaces;

/// <summary>
/// Repository interface for AccountLockout entity
/// </summary>
public interface IAccountLockoutRepository : IRepository<AccountLockout>
{
    /// <summary>
    /// Gets account lockout record by user ID
    /// </summary>
    Task<AccountLockout?> GetByUserIdAsync(Guid userId);

    /// <summary>
    /// Gets all currently locked accounts
    /// </summary>
    Task<List<AccountLockout>> GetCurrentlyLockedAccountsAsync();

    /// <summary>
    /// Gets expired lockouts that need cleanup
    /// </summary>
    Task<List<AccountLockout>> GetExpiredLockoutsAsync();

    /// <summary>
    /// Gets lockout history for a user
    /// </summary>
    Task<List<AccountLockout>> GetLockoutHistoryAsync(Guid userId);

    /// <summary>
    /// Gets all lockouts for statistics
    /// </summary>
    Task<List<AccountLockout>> GetAllLockoutsAsync();

    /// <summary>
    /// Gets lockouts by reason
    /// </summary>
    Task<List<AccountLockout>> GetLockoutsByReasonAsync(Domain.Enums.AccountLockoutReason reason);
}
