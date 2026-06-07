using Bank.Api.Constants;
using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Bank.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bank.Api.Controllers.Account;

/// <summary>
/// Controller for account statement operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatementController : ControllerBase
{
    private readonly IStatementService _statementService;

    public StatementController(
        IStatementService statementService)
    {
        _statementService = statementService;
    }

    /// <summary>
    /// Generate a statement for an account
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<StatementGenerationResult>> GenerateStatement([FromBody] GenerateStatementRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _statementService.GenerateStatementAsync(request, userId);
        
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Generate a consolidated statement for multiple accounts
    /// </summary>
    [HttpPost("generate-consolidated")]
    public async Task<ActionResult<StatementGenerationResult>> GenerateConsolidatedStatement([FromBody] ConsolidatedStatementRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _statementService.GenerateConsolidatedStatementAsync(request, userId);
        
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Get statement by ID
    /// </summary>
    [HttpGet("{statementId:guid}")]
    public async Task<ActionResult<StatementDto>> GetStatement(Guid statementId)
    {
        var statement = await _statementService.GetStatementByIdAsync(statementId);
        
        if (statement == null)
        {
            return NotFound("Statement not found");
        }
        
        return Ok(statement);
    }

    /// <summary>
    /// Search statements with criteria
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<StatementSearchResult>> SearchStatements([FromBody] StatementSearchCriteria criteria)
    {
        var result = await _statementService.SearchStatementsAsync(criteria);
        return Ok(result);
    }

    /// <summary>
    /// Get statements for a specific account
    /// </summary>
    [HttpGet("account/{accountId:guid}")]
    public async Task<ActionResult<List<StatementDto>>> GetAccountStatements(Guid accountId, [FromQuery] int? limit = null)
    {
        var statements = await _statementService.GetAccountStatementsAsync(accountId, limit);
        return Ok(statements);
    }

    /// <summary>
    /// Get statement summary for dashboard
    /// </summary>
    [HttpGet("summary/{accountId:guid}")]
    public async Task<ActionResult<StatementSummary>> GetStatementSummary(
        Guid accountId, 
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate)
    {
        var summary = await _statementService.GetStatementSummaryAsync(accountId, startDate, endDate);
        return Ok(summary);
    }

    /// <summary>
    /// Download statement file
    /// </summary>
    [HttpGet("{statementId:guid}/download")]
    public async Task<IActionResult> DownloadStatement(Guid statementId)
    {
        var (content, fileName, contentType) = await _statementService.DownloadStatementAsync(statementId);
        return File(content, contentType, fileName);
    }

    /// <summary>
    /// Deliver statement via specified method
    /// </summary>
    [HttpPost("{statementId:guid}/deliver")]
    public async Task<ActionResult<bool>> DeliverStatement(
        Guid statementId, 
        [FromBody] DeliverStatementRequest request)
    {
        var result = await _statementService.DeliverStatementAsync(
            statementId, 
            request.DeliveryMethod, 
            request.DeliveryAddress);
        
        if (result)
        {
            return Ok(new { Success = true, Message = "Statement delivered successfully" });
        }
        
        return BadRequest(new { Success = false, Message = "Failed to deliver statement" });
    }

    /// <summary>
    /// Get statement delivery status
    /// </summary>
    [HttpGet("{statementId:guid}/delivery-status")]
    public async Task<ActionResult<StatementDeliveryStatus>> GetDeliveryStatus(Guid statementId)
    {
        var status = await _statementService.GetDeliveryStatusAsync(statementId);
        return Ok(status);
    }

    /// <summary>
    /// Regenerate existing statement
    /// </summary>
    [HttpPost("{statementId:guid}/regenerate")]
    public async Task<ActionResult<StatementGenerationResult>> RegenerateStatement(Guid statementId)
    {
        var userId = GetCurrentUserId();
        var result = await _statementService.RegenerateStatementAsync(statementId, userId);
        
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Cancel statement generation
    /// </summary>
    [HttpPost("{statementId:guid}/cancel")]
    public async Task<ActionResult<bool>> CancelStatementGeneration(Guid statementId)
    {
        var userId = GetCurrentUserId();
        var result = await _statementService.CancelStatementGenerationAsync(statementId, userId);
        
        if (result)
        {
            return Ok(new { Success = true, Message = "Statement generation cancelled" });
        }
        
        return BadRequest(new { Success = false, Message = "Failed to cancel statement generation" });
    }

    /// <summary>
    /// Get available statement templates
    /// </summary>
    [HttpGet("templates")]
    public async Task<ActionResult<List<StatementTemplate>>> GetAvailableTemplates()
    {
        var templates = await _statementService.GetAvailableTemplatesAsync();
        return Ok(templates);
    }

    /// <summary>
    /// Validate statement request
    /// </summary>
    [HttpPost("validate")]
    public async Task<ActionResult<ValidationResult>> ValidateStatementRequest([FromBody] GenerateStatementRequest request)
    {
        var (isValid, errors) = await _statementService.ValidateStatementRequestAsync(request);
        
        return Ok(new ValidationResult
        {
            IsValid = isValid,
            Errors = errors
        });
    }

    /// <summary>
    /// Get available transaction categories for filtering
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<List<string>>> GetTransactionCategories()
    {
        await Task.CompletedTask; // Simulate async operation
        
        var categories = new List<string>
        {
            "Income",
            "Food & Dining",
            "Transportation",
            "Utilities",
            "Housing",
            "Healthcare",
            "Fees & Charges",
            "Interest",
            "Transfers",
            "ATM & Cash",
            "Shopping",
            "Entertainment",
            "Other"
        };
        
        return Ok(categories);
    }

    /// <summary>
    /// Get statement statistics for a specific account and period
    /// </summary>
    [HttpGet("statistics/{accountId:guid}")]
    public async Task<ActionResult<StatementStatistics>> GetStatementStatistics(
        Guid accountId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var summary = await _statementService.GetStatementSummaryAsync(accountId, startDate, endDate);
        
        var statistics = new StatementStatistics
        {
            AccountId = accountId,
            PeriodStart = startDate,
            PeriodEnd = endDate,
            TransactionCount = summary.TransactionCount,
            TotalIncome = summary.TotalIncome,
            TotalExpenses = summary.TotalExpenses,
            NetChange = summary.NetChange,
            CategoryBreakdown = summary.CategoryBreakdown,
            MonthlyBreakdown = summary.MonthlyBreakdown
        };
        
        return Ok(statistics);
    }

    #region Private Helper Methods

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        throw new UnauthorizedAccessException("Invalid user ID");
    }

    #endregion
}

