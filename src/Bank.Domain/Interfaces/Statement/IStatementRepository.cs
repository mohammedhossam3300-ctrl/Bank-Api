using Bank.Domain.Entities;
using Bank.Domain.Enums;

namespace Bank.Domain.Interfaces;

/// <summary>
/// Repository interface for AccountStatement entities
/// </summary>
public interface IStatementRepository : IRepository<AccountStatement>
{
    /// <summary>
    /// Get statements for a specific account
    /// </summary>
    Task<List<AccountStatement>> GetByAccountIdAsync(Guid accountId, int? limit = null);
    
    /// <summary>
    /// Get statements by date range
    /// </summary>
    Task<List<AccountStatement>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Get statements by status
    /// </summary>
    Task<List<AccountStatement>> GetByStatusAsync(StatementStatus status);
    
    /// <summary>
    /// Get the next statement sequence number for an account
    /// </summary>
    Task<int> GetNextStatementSequenceAsync(Guid accountId, DateTime statementDate);
    
    /// <summary>
    /// Check if statement exists for account and period
    /// </summary>
    Task<bool> ExistsForPeriodAsync(Guid accountId, DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Get statements pending delivery
    /// </summary>
    Task<List<AccountStatement>> GetPendingDeliveryAsync();
    
    /// <summary>
    /// Get statements by delivery method
    /// </summary>
    Task<List<AccountStatement>> GetByDeliveryMethodAsync(StatementDeliveryMethod deliveryMethod);
}
