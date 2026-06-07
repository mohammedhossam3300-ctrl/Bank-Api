using Bank.Domain.Entities;
using Card = Bank.Domain.Entities.Card;
using Bank.Domain.Enums;
using Bank.Domain.Interfaces;
using Bank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bank.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Card entity
/// </summary>
public class CardRepository : Repository<Card>, ICardRepository
{
    private new readonly BankDbContext _context;
    private readonly ILogger<CardRepository> _logger;

    public CardRepository(BankDbContext context, ILogger<CardRepository> logger) : base(context)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Card?> GetCardWithAccountAsync(Guid cardId)
    {
        try
        {
            return await _context.Cards
                .Include(c => c.Account)
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(c => c.Id == cardId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting card with account for card ID {CardId}", cardId);
            return null;
        }
    }

    public async Task<List<Card>> GetCardsByCustomerIdAsync(Guid customerId)
    {
        try
        {
            return await _context.Cards
                .Include(c => c.Account)
                .Where(c => c.CustomerId == customerId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cards for customer {CustomerId}", customerId);
            return new List<Card>();
        }
    }

    public async Task<List<Card>> GetCardsByAccountIdAsync(Guid accountId)
    {
        try
        {
            return await _context.Cards
                .Include(c => c.Customer)
                .Where(c => c.AccountId == accountId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            // Do not log account ID - use generic message
            _logger.LogError(ex, "Error getting cards for account");
            return new List<Card>();
        }
    }

    public async Task<bool> IsCardNumberUniqueAsync(string cardNumber)
    {
        try
        {
            // Hash the card number for comparison to avoid clear text storage
            var hashedCardNumber = HashCardNumber(cardNumber);
            return !await _context.Cards
                .AnyAsync(c => c.CardNumber == hashedCardNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking card number uniqueness");
            return false;
        }
    }

    public async Task<Card?> GetCardByCardNumberAsync(string cardNumber)
    {
        try
        {
            // Hash the card number for comparison to avoid clear text storage
            var hashedCardNumber = HashCardNumber(cardNumber);
            return await _context.Cards
                .Include(c => c.Account)
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(c => c.CardNumber == hashedCardNumber);
        }
        catch (Exception ex)
        {
            // Do not log the card number - log generic message only
            _logger.LogError(ex, "Error getting card by card number");
            return null;
        }
    }

    /// <summary>
    /// Hash card number for secure comparison without exposing clear text
    /// </summary>
    private static string HashCardNumber(string cardNumber)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(cardNumber));
            return Convert.ToBase64String(hashedBytes);
        }
    }
    public async Task<List<Card>> GetCardsExpiringWithinDaysAsync(int days)
    {
        try
        {
            var expiryDate = DateTime.UtcNow.AddDays(days);
            return await _context.Cards
                .Include(c => c.Customer)
                .Include(c => c.Account)
                .Where(c => c.ExpiryDate <= expiryDate && c.Status == CardStatus.Active)
                .OrderBy(c => c.ExpiryDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cards expiring within {Days} days", days);
            return new List<Card>();
        }
    }

    public async Task<List<Card>> GetCardsExpiringInDaysAsync(int daysBeforeExpiry)
    {
        try
        {
            var targetDate = DateTime.UtcNow.AddDays(daysBeforeExpiry);
            var startDate = targetDate.Date;
            var endDate = startDate.AddDays(1);
            
            return await _context.Cards
                .Include(c => c.Customer)
                .Include(c => c.Account)
                .Where(c => c.ExpiryDate >= startDate && 
                           c.ExpiryDate < endDate && 
                           c.Status == CardStatus.Active)
                .OrderBy(c => c.ExpiryDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cards expiring in {Days} days", daysBeforeExpiry);
            return new List<Card>();
        }
    }

    public async Task<List<Card>> GetBlockedCardsByCustomerIdAsync(Guid customerId)
    {
        try
        {
            return await _context.Cards
                .Include(c => c.Account)
                .Where(c => c.CustomerId == customerId && 
                           (c.Status == CardStatus.Blocked || 
                            c.Status == CardStatus.Lost || 
                            c.Status == CardStatus.Stolen))
                .OrderByDescending(c => c.LastBlockedDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting blocked cards for customer {CustomerId}", customerId);
            return new List<Card>();
        }
    }

    public async Task<List<Card>> GetActiveCardsByCustomerIdAsync(Guid customerId)
    {
        try
        {
            return await _context.Cards
                .Include(c => c.Account)
                .Where(c => c.CustomerId == customerId && c.Status == CardStatus.Active)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active cards for customer {CustomerId}", customerId);
            return new List<Card>();
        }
    }

    public async Task UpdateCardStatusAsync(Guid cardId, CardStatus newStatus, string? reason = null, Guid? changedBy = null)
    {
        try
        {
            var card = await _context.Cards.FindAsync(cardId);
            if (card == null)
            {
                _logger.LogWarning("Card {CardId} not found for status update", cardId);
                return;
            }

            var previousStatus = card.Status;
            card.Status = newStatus;

            // Create status history record
            var statusHistory = new CardStatusHistory
            {
                CardId = cardId,
                PreviousStatus = previousStatus,
                NewStatus = newStatus,
                Reason = reason,
                ChangedBy = changedBy,
                ChangeDate = DateTime.UtcNow
            };

            _context.CardStatusHistories.Add(statusHistory);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Card {CardId} status updated from {PreviousStatus} to {NewStatus}", 
                cardId, previousStatus, newStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating card status for card {CardId}", cardId);
            throw;
        }
    }
}

