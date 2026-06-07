using Bank.Api.Helpers;
using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bank.Api.Controllers.Card;

/// <summary>
/// Controller for card management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CardController : ControllerBase
{
    private readonly ICardService _cardService;

    public CardController(ICardService cardService)
    {
        _cardService = cardService;
    }

    /// <summary>
    /// Issue a new card for a customer account
    /// </summary>
    [HttpPost("issue")]
    public async Task<ActionResult<CardIssuanceResult>> IssueCard([FromBody] CardIssuanceRequest request)
    {
        var userId = GetCurrentUserId();
        if (request.CustomerId != userId)
        {
            return Forbid("You can only issue cards for your own account");
        }

        var result = await _cardService.IssueCardAsync(request);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Activate a card
    /// </summary>
    [HttpPost("{cardId}/activate")]
    public async Task<ActionResult<CardActivationResult>> ActivateCard(Guid cardId, [FromBody] CardActivationRequest request)
    {
        var userId = GetCurrentUserId();
        if (request.CustomerId != userId)
        {
            return Forbid("You can only activate your own cards");
        }

        request.CardId = cardId;
        var result = await _cardService.ActivateCardAsync(request);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Block a card
    /// </summary>
    [HttpPost("{cardId}/block")]
    public async Task<ActionResult<CardBlockResult>> BlockCard(Guid cardId, [FromBody] CardBlockRequest request)
    {
        var userId = GetCurrentUserId();
        if (request.CustomerId != userId)
        {
            return Forbid("You can only block your own cards");
        }

        request.CardId = cardId;
        var result = await _cardService.BlockCardAsync(request);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Unblock a card
    /// </summary>
    [HttpPost("{cardId}/unblock")]
    public async Task<ActionResult<CardBlockResult>> UnblockCard(Guid cardId, [FromBody] CardUnblockRequest request)
    {
        var userId = GetCurrentUserId();
        if (request.CustomerId != userId)
        {
            return Forbid("You can only unblock your own cards");
        }

        request.CardId = cardId;
        var result = await _cardService.UnblockCardAsync(request);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
    /// <summary>
    /// Update card spending limits
    /// </summary>
    [HttpPut("{cardId}/limits")]
    public async Task<ActionResult<CardLimitUpdateResult>> UpdateLimits(Guid cardId, [FromBody] CardLimitUpdateRequest request)
    {
        var userId = GetCurrentUserId();
        if (request.CustomerId != userId)
        {
            return Forbid("You can only update limits for your own cards");
        }

        request.CardId = cardId;
        var result = await _cardService.UpdateLimitsAsync(request);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Get card details
    /// </summary>
    [HttpGet("{cardId}")]
    public async Task<ActionResult<CardDetailsDto>> GetCardDetails(Guid cardId)
    {
        var userId = GetCurrentUserId();
        var cardDetails = await _cardService.GetCardDetailsAsync(cardId, userId);
        
        if (cardDetails == null)
        {
            return NotFound("Card not found");
        }

        return Ok(cardDetails);
    }

    /// <summary>
    /// Get all cards for the current customer
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CardSummaryDto>>> GetCustomerCards()
    {
        var userId = GetCurrentUserId();
        var cards = await _cardService.GetCustomerCardsAsync(userId);
        
        return Ok(cards);
    }

    /// <summary>
    /// Get card transactions with filtering and pagination
    /// </summary>
    [HttpGet("{cardId}/transactions")]
    public async Task<ActionResult<PagedResult<CardTransactionDto>>> GetCardTransactions(
        Guid cardId,
        [FromQuery] CardTransactionFilterRequest request)
    {
        var userId = GetCurrentUserId();
        
        // Merge path and query parameters
        var searchRequest = new CardTransactionSearchRequest
        {
            CardId = cardId,
            CustomerId = userId,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            MinAmount = request.MinAmount,
            MaxAmount = request.MaxAmount,
            MerchantName = request.MerchantName,
            IsInternational = request.IsInternational,
            Page = request.Page,
            PageSize = Math.Min(request.PageSize, 100), // Limit page size
            SortBy = request.SortBy,
            SortDescending = request.SortDescending
        };

        // Parse enums if provided
        if (Enum.TryParse<Domain.Enums.CardTransactionType>(request.TransactionType, true, out var txType))
            searchRequest.TransactionType = txType;
        
        if (Enum.TryParse<Domain.Enums.CardTransactionStatus>(request.Status, true, out var txStatus))
            searchRequest.Status = txStatus;
        
        if (Enum.TryParse<Domain.Enums.MerchantCategory>(request.MerchantCategory, true, out var category))
            searchRequest.MerchantCategory = category;

        var result = await _cardService.GetCardTransactionsAsync(searchRequest);
        
        return Ok(result);
    }
    /// <summary>
    /// Change card PIN
    /// </summary>
    [HttpPost("{cardId}/change-pin")]
    public async Task<ActionResult<CardPinChangeResult>> ChangePin(Guid cardId, [FromBody] CardPinChangeRequest request)
    {
        var userId = GetCurrentUserId();
        if (request.CustomerId != userId)
        {
            return Forbid("You can only change PIN for your own cards");
        }

        request.CardId = cardId;
        var result = await _cardService.ChangePinAsync(request);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Reset card PIN
    /// </summary>
    [HttpPost("{cardId}/reset-pin")]
    public async Task<ActionResult<CardPinResetResult>> ResetPin(Guid cardId, [FromBody] CardPinResetRequest request)
    {
        var userId = GetCurrentUserId();
        if (request.CustomerId != userId)
        {
            return Forbid("You can only reset PIN for your own cards");
        }

        request.CardId = cardId;
        var result = await _cardService.ResetPinAsync(request);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Update merchant category restrictions
    /// </summary>
    [HttpPut("{cardId}/merchant-restrictions")]
    public async Task<ActionResult<CardMerchantRestrictionsResult>> UpdateMerchantRestrictions(
        Guid cardId, 
        [FromBody] CardMerchantRestrictionsRequest request)
    {
        var userId = GetCurrentUserId();
        if (request.CustomerId != userId)
        {
            return Forbid("You can only update restrictions for your own cards");
        }

        request.CardId = cardId;
        var result = await _cardService.UpdateMerchantRestrictionsAsync(request);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Update contactless payment settings
    /// </summary>
    [HttpPut("{cardId}/contactless")]
    public async Task<ActionResult<CardContactlessResult>> UpdateContactlessSettings(
        Guid cardId, 
        [FromBody] CardContactlessRequest request)
    {
        var userId = GetCurrentUserId();
        if (request.CustomerId != userId)
        {
            return Forbid("You can only update settings for your own cards");
        }

        request.CardId = cardId;
        var result = await _cardService.UpdateContactlessSettingsAsync(request);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Update online transactions settings
    /// </summary>
    [HttpPut("{cardId}/online-transactions")]
    public async Task<ActionResult<CardOnlineTransactionsResult>> UpdateOnlineTransactions(
        Guid cardId, 
        [FromBody] CardOnlineTransactionsRequest request)
    {
        var userId = GetCurrentUserId();
        if (request.CustomerId != userId)
        {
            return Forbid("You can only update settings for your own cards");
        }

        request.CardId = cardId;
        var result = await _cardService.UpdateOnlineTransactionsAsync(request);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Update international transactions settings
    /// </summary>
    [HttpPut("{cardId}/international-transactions")]
    public async Task<ActionResult<CardInternationalTransactionsResult>> UpdateInternationalTransactions(
        Guid cardId, 
        [FromBody] CardInternationalTransactionsRequest request)
    {
        var userId = GetCurrentUserId();
        if (request.CustomerId != userId)
        {
            return Forbid("You can only update settings for your own cards");
        }

        request.CardId = cardId;
        var result = await _cardService.UpdateInternationalTransactionsAsync(request);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Get card usage statistics
    /// </summary>
    [HttpGet("{cardId}/usage-stats")]
    public async Task<ActionResult<CardUsageStatsDto>> GetCardUsageStats(
        Guid cardId,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var userId = GetCurrentUserId();
        
        // Default to last 30 days if no dates provided
        var from = fromDate ?? DateTime.UtcNow.AddDays(-30);
        var to = toDate ?? DateTime.UtcNow;

        var stats = await _cardService.GetCardUsageStatsAsync(cardId, userId, from, to);
        
        return Ok(stats);
    }

    private Guid GetCurrentUserId() => this.GetCurrentUserIdRequired();
}