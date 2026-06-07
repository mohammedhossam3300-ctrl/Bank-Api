using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Bank.Domain.Enums;
using Bank.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Bank.Application.Services;

public class AccountLifecycleService : IAccountLifecycleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeeCalculationService _feeCalculationService;
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<AccountLifecycleService> _logger;

    public AccountLifecycleService(
        IUnitOfWork unitOfWork,
        IFeeCalculationService feeCalculationService,
        IAuditLogService auditLogService,
        ILogger<AccountLifecycleService> logger)
    {
        _unitOfWork = unitOfWork;
        _feeCalculationService = feeCalculationService;
        _auditLogService = auditLogService;
        _logger = logger;
    }

    public async Task<bool> CloseAccountAsync(Guid accountId, string reason, Guid userId)
    {
        try
        {
            var account = await _unitOfWork.Repository<Account>().GetByIdAsync(accountId);
            if (account == null)
            {
                _logger.LogWarning("Account {AccountId} not found for closure", accountId);
                return false;
            }

            if (account.Status == AccountStatus.Closed)
            {
                _logger.LogWarning("Account {AccountId} is already closed", accountId);
                return false;
            }

            // Check for pending transactions or holds
            if (account.HasHolds)
            {
                _logger.LogWarning("Cannot close account {AccountId} with active holds", accountId);
                return false;
            }

            // Calculate and apply any final fees
            await ApplyAccountFeesAsync(accountId, userId);

            // Update account status
            var previousStatus = account.Status;
            account.Status = AccountStatus.Closed;
            account.ClosedDate = DateTime.UtcNow;
            account.ClosureReason = reason;
            account.UpdatedAt = DateTime.UtcNow;
            account.UpdatedBy = userId.ToString();

            // Add status history
            var statusHistory = new AccountStatusHistory
            {
                AccountId = accountId,
                FromStatus = previousStatus,
                ToStatus = AccountStatus.Closed,
                Reason = reason,
                ChangedByUserId = userId
            };

            await _unitOfWork.Repository<AccountStatusHistory>().AddAsync(statusHistory);
            _unitOfWork.Repository<Account>().Update(account);

            await _auditLogService.LogUserActionAsync(
                userId,
                "ACCOUNT_CLOSED",
                "Account",
                accountId.ToString(),
                null,
                $"Reason: {reason}");

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Account {AccountId} closed successfully", accountId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing account {AccountId}", accountId);
            return false;
        }
    }

    public async Task<bool> ReopenAccountAsync(Guid accountId, Guid userId)
    {
        try
        {
            var account = await _unitOfWork.Repository<Account>().GetByIdAsync(accountId);
            if (account == null)
            {
                _logger.LogWarning("Account {AccountId} not found for reopening", accountId);
                return false;
            }

            if (account.Status != AccountStatus.Closed)
            {
                _logger.LogWarning("Account {AccountId} is not closed", accountId);
                return false;
            }

            // Update account status
            var previousStatus = account.Status;
            account.Status = AccountStatus.Active;
            account.ClosedDate = null;
            account.ClosureReason = null;
            account.UpdatedAt = DateTime.UtcNow;
            account.UpdatedBy = userId.ToString();

            // Add status history
            var statusHistory = new AccountStatusHistory
            {
                AccountId = accountId,
                FromStatus = previousStatus,
                ToStatus = AccountStatus.Active,
                Reason = "Account reopened",
                ChangedByUserId = userId
            };

            await _unitOfWork.Repository<AccountStatusHistory>().AddAsync(statusHistory);
            _unitOfWork.Repository<Account>().Update(account);

            await _auditLogService.LogUserActionAsync(
                userId,
                "ACCOUNT_REOPENED",
                "Account",
                accountId.ToString(),
                null,
                "Account reopened");

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Account {AccountId} reopened successfully", accountId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reopening account {AccountId}", accountId);
            return false;
        }
    }

    public async Task<bool> MarkAccountDormantAsync(Guid accountId)
    {
        try
        {
            var account = await _unitOfWork.Repository<Account>().GetByIdAsync(accountId);
            if (account == null)
            {
                _logger.LogWarning("Account {AccountId} not found for dormancy marking", accountId);
                return false;
            }

            if (account.Status == AccountStatus.Dormant)
            {
                _logger.LogWarning("Account {AccountId} is already dormant", accountId);
                return false;
            }

            // Update account status
            var previousStatus = account.Status;
            account.MarkAsDormant();
            account.UpdatedAt = DateTime.UtcNow;
            account.UpdatedBy = "SYSTEM";

            // Add status history
            var statusHistory = new AccountStatusHistory
            {
                AccountId = accountId,
                FromStatus = previousStatus,
                ToStatus = AccountStatus.Dormant,
                Reason = "Account marked dormant due to inactivity",
                ChangedByUserId = Guid.Empty // System action
            };

            await _unitOfWork.Repository<AccountStatusHistory>().AddAsync(statusHistory);
            _unitOfWork.Repository<Account>().Update(account);

            await _auditLogService.LogSystemEventAsync(
                "ACCOUNT_MARKED_DORMANT",
                "Account",
                accountId.ToString(),
                "Account marked dormant due to inactivity");

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Account {AccountId} marked as dormant", accountId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking account {AccountId} as dormant", accountId);
            return false;
        }
    }

    public async Task<bool> ReactivateAccountAsync(Guid accountId, Guid userId)
    {
        try
        {
            var account = await _unitOfWork.Repository<Account>().GetByIdAsync(accountId);
            if (account == null)
            {
                _logger.LogWarning("Account {AccountId} not found for reactivation", accountId);
                return false;
            }

            if (account.Status != AccountStatus.Dormant)
            {
                _logger.LogWarning("Account {AccountId} is not dormant", accountId);
                return false;
            }

            // Update account status
            var previousStatus = account.Status;
            account.Status = AccountStatus.Active;
            account.DormancyDate = null;
            account.UpdateActivity();
            account.UpdatedAt = DateTime.UtcNow;
            account.UpdatedBy = userId.ToString();

            // Add status history
            var statusHistory = new AccountStatusHistory
            {
                AccountId = accountId,
                FromStatus = previousStatus,
                ToStatus = AccountStatus.Active,
                Reason = "Account reactivated by user",
                ChangedByUserId = userId
            };

            await _unitOfWork.Repository<AccountStatusHistory>().AddAsync(statusHistory);
            _unitOfWork.Repository<Account>().Update(account);

            await _auditLogService.LogUserActionAsync(
                userId,
                "ACCOUNT_REACTIVATED",
                "Account",
                accountId.ToString(),
                null,
                "Account reactivated from dormant status");

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Account {AccountId} reactivated successfully", accountId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reactivating account {AccountId}", accountId);
            return false;
        }
    }

    public async Task<bool> SuspendAccountAsync(Guid accountId, string reason, Guid userId)
    {
        try
        {
            var account = await _unitOfWork.Repository<Account>().GetByIdAsync(accountId);
            if (account == null)
            {
                _logger.LogWarning("Account {AccountId} not found for suspension", accountId);
                return false;
            }

            if (account.Status == AccountStatus.Suspended)
            {
                _logger.LogWarning("Account {AccountId} is already suspended", accountId);
                return false;
            }

            // Update account status
            var previousStatus = account.Status;
            account.Status = AccountStatus.Suspended;
            account.UpdatedAt = DateTime.UtcNow;
            account.UpdatedBy = userId.ToString();

            // Add status history
            var statusHistory = new AccountStatusHistory
            {
                AccountId = accountId,
                FromStatus = previousStatus,
                ToStatus = AccountStatus.Suspended,
                Reason = reason,
                ChangedByUserId = userId
            };

            await _unitOfWork.Repository<AccountStatusHistory>().AddAsync(statusHistory);
            _unitOfWork.Repository<Account>().Update(account);

            await _auditLogService.LogUserActionAsync(
                userId,
                "ACCOUNT_SUSPENDED",
                "Account",
                accountId.ToString(),
                null,
                $"Reason: {reason}");

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Account {AccountId} suspended successfully", accountId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error suspending account {AccountId}", accountId);
            return false;
        }
    }

    public async Task<bool> UnsuspendAccountAsync(Guid accountId, Guid userId)
    {
        try
        {
            var account = await _unitOfWork.Repository<Account>().GetByIdAsync(accountId);
            if (account == null)
            {
                _logger.LogWarning("Account {AccountId} not found for unsuspension", accountId);
                return false;
            }

            if (account.Status != AccountStatus.Suspended)
            {
                _logger.LogWarning("Account {AccountId} is not suspended", accountId);
                return false;
            }

            // Update account status
            var previousStatus = account.Status;
            account.Status = AccountStatus.Active;
            account.UpdatedAt = DateTime.UtcNow;
            account.UpdatedBy = userId.ToString();

            // Add status history
            var statusHistory = new AccountStatusHistory
            {
                AccountId = accountId,
                FromStatus = previousStatus,
                ToStatus = AccountStatus.Active,
                Reason = "Account unsuspended",
                ChangedByUserId = userId
            };

            await _unitOfWork.Repository<AccountStatusHistory>().AddAsync(statusHistory);
            _unitOfWork.Repository<Account>().Update(account);

            await _auditLogService.LogUserActionAsync(
                userId,
                "ACCOUNT_UNSUSPENDED",
                "Account",
                accountId.ToString(),
                null,
                "Account unsuspended");

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Account {AccountId} unsuspended successfully", accountId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unsuspending account {AccountId}", accountId);
            return false;
        }
    }

    public async Task<bool> FreezeAccountAsync(Guid accountId, string reason, Guid userId)
    {
        try
        {
            var account = await _unitOfWork.Repository<Account>().GetByIdAsync(accountId);
            if (account == null)
            {
                _logger.LogWarning("Account {AccountId} not found for freezing", accountId);
                return false;
            }

            if (account.Status == AccountStatus.Frozen)
            {
                _logger.LogWarning("Account {AccountId} is already frozen", accountId);
                return false;
            }

            // Update account status
            var previousStatus = account.Status;
            account.Status = AccountStatus.Frozen;
            account.UpdatedAt = DateTime.UtcNow;
            account.UpdatedBy = userId.ToString();

            // Add status history
            var statusHistory = new AccountStatusHistory
            {
                AccountId = accountId,
                FromStatus = previousStatus,
                ToStatus = AccountStatus.Frozen,
                Reason = reason,
                ChangedByUserId = userId
            };

            await _unitOfWork.Repository<AccountStatusHistory>().AddAsync(statusHistory);
            _unitOfWork.Repository<Account>().Update(account);

            await _auditLogService.LogUserActionAsync(
                userId,
                "ACCOUNT_FROZEN",
                "Account",
                accountId.ToString(),
                null,
                $"Reason: {reason}");

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Account {AccountId} frozen successfully", accountId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error freezing account {AccountId}", accountId);
            return false;
        }
    }

    public async Task<bool> UnfreezeAccountAsync(Guid accountId, Guid userId)
    {
        try
        {
            var account = await _unitOfWork.Repository<Account>().GetByIdAsync(accountId);
            if (account == null)
            {
                _logger.LogWarning("Account {AccountId} not found for unfreezing", accountId);
                return false;
            }

            if (account.Status != AccountStatus.Frozen)
            {
                _logger.LogWarning("Account {AccountId} is not frozen", accountId);
                return false;
            }

            // Update account status
            var previousStatus = account.Status;
            account.Status = AccountStatus.Active;
            account.UpdatedAt = DateTime.UtcNow;
            account.UpdatedBy = userId.ToString();

            // Add status history
            var statusHistory = new AccountStatusHistory
            {
                AccountId = accountId,
                FromStatus = previousStatus,
                ToStatus = AccountStatus.Active,
                Reason = "Account unfrozen",
                ChangedByUserId = userId
            };

            await _unitOfWork.Repository<AccountStatusHistory>().AddAsync(statusHistory);
            _unitOfWork.Repository<Account>().Update(account);

            await _auditLogService.LogUserActionAsync(
                userId,
                "ACCOUNT_UNFROZEN",
                "Account",
                accountId.ToString(),
                null,
                "Account unfrozen");

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Account {AccountId} unfrozen successfully", accountId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unfreezing account {AccountId}", accountId);
            return false;
        }
    }

    public async Task<bool> ApplyHoldAsync(Guid accountId, decimal amount, string reason, Guid userId)
    {
        try
        {
            var account = await _unitOfWork.Repository<Account>().GetByIdAsync(accountId);
            if (account == null)
            {
                _logger.LogWarning("Account {AccountId} not found for hold application", accountId);
                return false;
            }

            if (amount <= 0)
            {
                _logger.LogWarning("Invalid hold amount {Amount} for account {AccountId}", amount, accountId);
                return false;
            }

            // Create hold
            var hold = new AccountHold
            {
                AccountId = accountId,
                Type = AccountHoldType.Administrative,
                Amount = amount,
                Description = reason,
                PlacedDate = DateTime.UtcNow,
                PlacedByUserId = userId
            };

            await _unitOfWork.Repository<AccountHold>().AddAsync(hold);

            // Update account
            account.HasHolds = true;
            account.UpdatedAt = DateTime.UtcNow;
            account.UpdatedBy = userId.ToString();

            _unitOfWork.Repository<Account>().Update(account);

            await _auditLogService.LogUserActionAsync(
                userId,
                "HOLD_APPLIED",
                "Account",
                accountId.ToString(),
                null,
                $"Hold amount: {amount:C}, Reason: {reason}");

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Hold applied to account {AccountId} for amount {Amount}", accountId, amount);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying hold to account {AccountId}", accountId);
            return false;
        }
    }

    public async Task<bool> ReleaseHoldAsync(Guid holdId, Guid userId)
    {
        try
        {
            var hold = await _unitOfWork.Repository<AccountHold>().GetByIdAsync(holdId);
            if (hold == null)
            {
                _logger.LogWarning("Hold {HoldId} not found for release", holdId);
                return false;
            }

            if (!hold.IsActive)
            {
                _logger.LogWarning("Hold {HoldId} is already released", holdId);
                return false;
            }

            // Release hold
            hold.Release(userId);

            _unitOfWork.Repository<AccountHold>().Update(hold);

            // Check if account has any other active holds
            var account = await _unitOfWork.Repository<Account>().GetByIdAsync(hold.AccountId);
            if (account != null)
            {
                var activeHolds = await _unitOfWork.Repository<AccountHold>()
                    .FindAsync(h => h.AccountId == hold.AccountId && h.IsActive);
                
                account.HasHolds = activeHolds.Any();
                account.UpdatedAt = DateTime.UtcNow;
                account.UpdatedBy = userId.ToString();

                _unitOfWork.Repository<Account>().Update(account);
            }

            await _auditLogService.LogUserActionAsync(
                userId,
                "HOLD_RELEASED",
                "Account",
                hold.AccountId.ToString(),
                null,
                $"Hold ID: {holdId}");

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Hold {HoldId} released successfully", holdId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error releasing hold {HoldId}", holdId);
            return false;
        }
    }

    public async Task<bool> AddRestrictionAsync(Guid accountId, AccountRestrictionType restrictionType, string reason, Guid userId)
    {
        try
        {
            var account = await _unitOfWork.Repository<Account>().GetByIdAsync(accountId);
            if (account == null)
            {
                _logger.LogWarning("Account {AccountId} not found for restriction addition", accountId);
                return false;
            }

            // Check if restriction already exists
            var existingRestriction = await _unitOfWork.Repository<AccountRestriction>()
                .FirstOrDefaultAsync(r => r.AccountId == accountId && r.Type == restrictionType && r.IsActive);

            if (existingRestriction != null)
            {
                _logger.LogWarning("Restriction {RestrictionType} already exists for account {AccountId}", restrictionType, accountId);
                return false;
            }

            // Create restriction
            var restriction = new AccountRestriction
            {
                AccountId = accountId,
                Type = restrictionType,
                Description = reason,
                AppliedDate = DateTime.UtcNow,
                AppliedByUserId = userId
            };

            await _unitOfWork.Repository<AccountRestriction>().AddAsync(restriction);

            // Update account
            account.HasRestrictions = true;
            account.UpdatedAt = DateTime.UtcNow;
            account.UpdatedBy = userId.ToString();

            _unitOfWork.Repository<Account>().Update(account);

            await _auditLogService.LogUserActionAsync(
                userId,
                "RESTRICTION_ADDED",
                "Account",
                accountId.ToString(),
                null,
                $"Restriction: {restrictionType}, Reason: {reason}");

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Restriction {RestrictionType} added to account {AccountId}", restrictionType, accountId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding restriction to account {AccountId}", accountId);
            return false;
        }
    }

    public async Task<bool> RemoveRestrictionAsync(Guid restrictionId, Guid userId)
    {
        try
        {
            var restriction = await _unitOfWork.Repository<AccountRestriction>().GetByIdAsync(restrictionId);
            if (restriction == null)
            {
                _logger.LogWarning("Restriction {RestrictionId} not found for removal", restrictionId);
                return false;
            }

            if (!restriction.IsActive)
            {
                _logger.LogWarning("Restriction {RestrictionId} is already removed", restrictionId);
                return false;
            }

            // Remove restriction
            restriction.Remove(userId);

            _unitOfWork.Repository<AccountRestriction>().Update(restriction);

            // Check if account has any other active restrictions
            var account = await _unitOfWork.Repository<Account>().GetByIdAsync(restriction.AccountId);
            if (account != null)
            {
                var activeRestrictions = await _unitOfWork.Repository<AccountRestriction>()
                    .FindAsync(r => r.AccountId == restriction.AccountId && r.IsActive);
                
                account.HasRestrictions = activeRestrictions.Any();
                account.UpdatedAt = DateTime.UtcNow;
                account.UpdatedBy = userId.ToString();

                _unitOfWork.Repository<Account>().Update(account);
            }

            await _auditLogService.LogUserActionAsync(
                userId,
                "RESTRICTION_REMOVED",
                "Account",
                restriction.AccountId.ToString(),
                null,
                $"Restriction ID: {restrictionId}");

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Restriction {RestrictionId} removed successfully", restrictionId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing restriction {RestrictionId}", restrictionId);
            return false;
        }
    }

    public async Task<decimal> CalculateAccountFeesAsync(Guid accountId, DateTime fromDate, DateTime toDate)
    {
        try
        {
            var account = await _unitOfWork.Repository<Account>().GetByIdAsync(accountId);
            if (account == null)
            {
                _logger.LogWarning("Account {AccountId} not found for fee calculation", accountId);
                return 0;
            }

            decimal totalFees = 0;

            // Calculate maintenance fees
            totalFees += await _feeCalculationService.CalculateMaintenanceFeeAsync(account, fromDate, toDate);

            // Calculate inactivity fees if applicable
            var daysSinceLastActivity = (DateTime.UtcNow - account.LastActivityDate).Days;
            if (daysSinceLastActivity > 90) // 3 months
            {
                totalFees += await _feeCalculationService.CalculateInactivityFeeAsync(account, daysSinceLastActivity);
            }

            return totalFees;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating fees for account {AccountId}", accountId);
            return 0;
        }
    }

    public async Task<bool> ApplyAccountFeesAsync(Guid accountId, Guid userId)
    {
        try
        {
            var account = await _unitOfWork.Repository<Account>().GetByIdAsync(accountId);
            if (account == null)
            {
                _logger.LogWarning("Account {AccountId} not found for fee application", accountId);
                return false;
            }

            var fromDate = account.LastFeeCalculationDate ?? account.OpenedDate;
            var toDate = DateTime.UtcNow;

            var totalFees = await CalculateAccountFeesAsync(accountId, fromDate, toDate);

            if (totalFees > 0)
            {
                // Create fee record
                var fee = new AccountFee
                {
                    AccountId = accountId,
                    Type = FeeType.MonthlyMaintenance,
                    Amount = totalFees,
                    Description = $"Monthly maintenance fee for period {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}",
                    AppliedDate = DateTime.UtcNow
                };

                await _unitOfWork.Repository<AccountFee>().AddAsync(fee);

                // Update account balance and fee calculation date
                account.Balance -= totalFees;
                account.LastFeeCalculationDate = DateTime.UtcNow;
                account.UpdatedAt = DateTime.UtcNow;
                account.UpdatedBy = userId.ToString();

                _unitOfWork.Repository<Account>().Update(account);

                await _auditLogService.LogUserActionAsync(
                    userId,
                    "FEES_APPLIED",
                    "Account",
                    accountId.ToString(),
                    null,
                    $"Fees applied: {totalFees:C}");

                _logger.LogInformation("Fees {Amount} applied to account {AccountId}", totalFees, accountId);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying fees to account {AccountId}", accountId);
            return false;
        }
    }

    public async Task<List<Account>> GetDormantAccountsAsync(int daysSinceLastActivity)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysSinceLastActivity);
            var accounts = await _unitOfWork.Repository<Account>()
                .FindAsync(a => a.Status == AccountStatus.Active && 
                               a.LastActivityDate < cutoffDate);

            return accounts.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dormant accounts");
            return new List<Account>();
        }
    }

    public async Task<List<Account>> GetAccountsForFeeProcessingAsync()
    {
        try
        {
            var accounts = await _unitOfWork.Repository<Account>()
                .FindAsync(a => a.Status == AccountStatus.Active && 
                               (a.LastFeeCalculationDate == null || 
                                a.LastFeeCalculationDate < DateTime.UtcNow.AddMonths(-1)));

            return accounts.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting accounts for fee processing");
            return new List<Account>();
        }
    }

    public async Task<bool> ProcessMonthlyMaintenanceFeesAsync()
    {
        try
        {
            var accounts = await GetAccountsForFeeProcessingAsync();
            var processedCount = 0;

            foreach (var account in accounts)
            {
                if (await ApplyAccountFeesAsync(account.Id, Guid.Empty)) // System process
                {
                    processedCount++;
                }
            }

            await _auditLogService.LogSystemEventAsync(
                "MONTHLY_FEES_PROCESSED",
                "System",
                "FeeProcessing",
                $"Processed {processedCount} accounts");

            _logger.LogInformation("Monthly maintenance fees processed for {Count} accounts", processedCount);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing monthly maintenance fees");
            return false;
        }
    }
}