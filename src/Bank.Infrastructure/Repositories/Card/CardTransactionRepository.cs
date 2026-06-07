using Bank.Application.DTOs;
using Bank.Domain.Entities;
using Card = Bank.Domain.Entities.Card;
using Bank.Domain.Enums;
using Bank.Domain.Interfaces;
using Bank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for card transaction operations
/// </summary>
public class CardTransactionRepository : ICardTransactionRepository
{
    private readonly BankDbContext _context;

    public CardTransactionRepository(BankDbContext context)
    {
        _context = context;
    }

    public async Task<CardTransaction> AddTransactionAsync(CardTransaction transaction)
    {
        _context.CardTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task UpdateTransactionAsync(CardTransaction transaction)
    {
        _context.CardTransactions.Update(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task<CardTransaction?> GetTransactionByIdAsync(Guid transactionId)
    {
        return await _context.CardTransactions
            .Include(t => t.Card)
            .FirstOrDefaultAsync(t => t.Id == transactionId);
    }

    public async Task<List<CardTransaction>> GetTransactionsByDateRangeAsync(Guid cardId, DateTime fromDate, DateTime toDate)
    {
        return await _context.CardTransactions
            .Where(t => t.CardId == cardId && 
                       t.TransactionDate >= fromDate && 
                       t.TransactionDate <= toDate)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<List<CardTransaction>> GetTodayTransactionsAsync(Guid cardId)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        
        return await _context.CardTransactions
            .Where(t => t.CardId == cardId && 
                       t.TransactionDate >= today && 
                       t.TransactionDate < tomorrow)
            .ToListAsync();
    }

    public async Task<(List<CardTransaction> transactions, int totalCount)> SearchTransactionsAsync(
        Guid cardId, 
        DateTime? fromDate = null, 
        DateTime? toDate = null, 
        CardTransactionType? type = null,
        CardTransactionStatus? status = null,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        string? merchantName = null,
        int page = 1,
        int pageSize = 20)
    {
        var query = _context.CardTransactions
            .Include(t => t.Card)
            .Where(t => t.CardId == cardId);

        // Apply filters
        if (fromDate.HasValue)
            query = query.Where(t => t.TransactionDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(t => t.TransactionDate <= toDate.Value);

        if (type.HasValue)
            query = query.Where(t => t.TransactionType == type.Value);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (minAmount.HasValue)
            query = query.Where(t => Math.Abs(t.Amount) >= minAmount.Value);

        if (maxAmount.HasValue)
            query = query.Where(t => Math.Abs(t.Amount) <= maxAmount.Value);

        if (!string.IsNullOrEmpty(merchantName))
            query = query.Where(t => t.MerchantName.Contains(merchantName));

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply pagination and ordering
        var transactions = await query
            .OrderByDescending(t => t.TransactionDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (transactions, totalCount);
    }

    public async Task<List<CardTransaction>> GetTransactionsForSettlementAsync(DateTime settlementDate, List<string>? transactionIds = null, CardNetwork? network = null)
    {
        var query = _context.CardTransactions
            .Include(t => t.Card)
            .Where(t => t.Status == CardTransactionStatus.Authorized || 
                       t.Status == CardTransactionStatus.Pending);

        if (transactionIds != null && transactionIds.Any())
        {
            var guids = transactionIds.Select(Guid.Parse).ToList();
            query = query.Where(t => guids.Contains(t.Id));
        }

        if (network.HasValue)
        {
            // Filter by network based on card number prefix (simplified)
            query = network.Value switch
            {
                CardNetwork.Visa => query.Where(t => t.Card!.CardNumber.StartsWith('4')),
                CardNetwork.Mastercard => query.Where(t => t.Card!.CardNumber.StartsWith('5')),
                CardNetwork.AmericanExpress => query.Where(t => t.Card!.CardNumber.StartsWith('3')),
                _ => query
            };
        }

        return await query.ToListAsync();
    }

    public async Task<List<CardTransaction>> GetSettledTransactionsByDateAsync(DateTime settlementDate)
    {
        return await _context.CardTransactions
            .Include(t => t.Card)
            .Where(t => t.SettlementDate.HasValue && 
                       t.SettlementDate.Value.Date == settlementDate.Date)
            .ToListAsync();
    }

    public async Task<CardAuthorization> AddAuthorizationAsync(CardAuthorization authorization)
    {
        _context.CardAuthorizations.Add(authorization);
        await _context.SaveChangesAsync();
        return authorization;
    }

    public async Task UpdateAuthorizationAsync(CardAuthorization authorization)
    {
        _context.CardAuthorizations.Update(authorization);
        await _context.SaveChangesAsync();
    }

    public async Task<CardAuthorization?> GetAuthorizationByCodeAsync(string authorizationCode)
    {
        return await _context.CardAuthorizations
            .Include(a => a.Card)
            .FirstOrDefaultAsync(a => a.AuthorizationCode == authorizationCode);
    }

    public async Task<CardStatement> AddStatementAsync(CardStatement statement)
    {
        _context.CardStatements.Add(statement);
        await _context.SaveChangesAsync();
        return statement;
    }

    public async Task<CardStatement?> GetStatementByIdAsync(Guid statementId)
    {
        return await _context.CardStatements
            .Include(s => s.Card)
            .FirstOrDefaultAsync(s => s.Id == statementId);
    }

    public async Task<List<CardStatement>> GetCardStatementsAsync(Guid cardId, int? limit = null)
    {
        IQueryable<CardStatement> query = _context.CardStatements
            .Where(s => s.CardId == cardId)
            .OrderByDescending(s => s.GeneratedDate);

        if (limit.HasValue)
            query = query.Take(limit.Value);

        return await query.ToListAsync();
    }
}


