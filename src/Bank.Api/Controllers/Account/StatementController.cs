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
    private readonly ILogger<StatementController> _logger;

    public StatementController(
        IStatementService statementService,
        ILogger<StatementController> logger)
    {
        _statementService = statementService;
        _logger = logger;
    }

    /// <summary>
    /// Generate a statement for an account
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<StatementGenerationResult>> GenerateStatement([FromBody] GenerateStatementRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _statementService.GenerateStatementAsync(request, userId);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating statement");
            return StatusCode(500, "An error occurred while generating the statement");
        }
    }

    /// <summary>
    /// Generate a consolidated statement for multiple accounts
    /// </summary>
    [HttpPost("generate-consolidated")]
    public async Task<ActionResult<StatementGenerationResult>> GenerateConsolidatedStatement([FromBody] ConsolidatedStatementRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _statementService.GenerateConsolidatedStatementAsync(request, userId);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating consolidated statement");
            return StatusCode(500, "An error occurred while generating the consolidated statement");
        }
    }

    /// <summary>
    /// Get statement by ID
    /// </summary>
    [HttpGet("{statementId:guid}")]
    public async Task<ActionResult<StatementDto>> GetStatement(Guid statementId)
    {
        try
        {
            var statement = await _statementService.GetStatementByIdAsync(statementId);
            
            if (statement == null)
            {
                return NotFound("Statement not found");
            }
            
            return Ok(statement);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving statement {StatementId}", statementId);
            return StatusCode(500, "An error occurred while retrieving the statement");
        }
    }

    /// <summary>
    /// Search statements with criteria
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<StatementSearchResult>> SearchStatements([FromBody] StatementSearchCriteria criteria)
    {
        try
        {
            var result = await _statementService.SearchStatementsAsync(criteria);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching statements");
            return StatusCode(500, "An error occurred while searching statements");
        }
    }

    /// <summary>
    /// Get statements for a specific account
    /// </summary>
    [HttpGet("account/{accountId:guid}")]
    public async Task<ActionResult<List<StatementDto>>> GetAccountStatements(Guid accountId, [FromQuery] int? limit = null)
    {
        try
        {
            var statements = await _statementService.GetAccountStatementsAsync(accountId, limit);
            return Ok(statements);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving statements for account {AccountId}", accountId);
            return StatusCode(500, "An error occurred while retrieving account statements");
        }
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
        try
        {
            var summary = await _statementService.GetStatementSummaryAsync(accountId, startDate, endDate);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating statement summary for account {AccountId}", accountId);
            return StatusCode(500, "An error occurred while generating the statement summary");
        }
    }

    /// <summary>
    /// Download statement file
    /// </summary>
    [HttpGet("{statementId:guid}/download")]
    public async Task<IActionResult> DownloadStatement(Guid statementId)
    {
        try
        {
            var (content, fileName, contentType) = await _statementService.DownloadStatementAsync(statementId);
            return File(content, contentType, fileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound("Statement file not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading statement {StatementId}", statementId);
            return StatusCode(500, "An error occurred while downloading the statement");
        }
    }

    /// <summary>
    /// Deliver statement via specified method
    /// </summary>
    [HttpPost("{statementId:guid}/deliver")]
    public async Task<ActionResult<bool>> DeliverStatement(
        Guid statementId, 
        [FromBody] DeliverStatementRequest request)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error delivering statement {StatementId}", statementId);
            return StatusCode(500, "An error occurred while delivering the statement");
        }
    }

    /// <summary>
    /// Get statement delivery status
    /// </summary>
    [HttpGet("{statementId:guid}/delivery-status")]
    public async Task<ActionResult<StatementDeliveryStatus>> GetDeliveryStatus(Guid statementId)
    {
        try
        {
            var status = await _statementService.GetDeliveryStatusAsync(statementId);
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting delivery status for statement {StatementId}", statementId);
            return StatusCode(500, "An error occurred while retrieving delivery status");
        }
    }

    /// <summary>
    /// Regenerate existing statement
    /// </summary>
    [HttpPost("{statementId:guid}/regenerate")]
    public async Task<ActionResult<StatementGenerationResult>> RegenerateStatement(Guid statementId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _statementService.RegenerateStatementAsync(statementId, userId);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error regenerating statement {StatementId}", statementId);
            return StatusCode(500, "An error occurred while regenerating the statement");
        }
    }

    /// <summary>
    /// Cancel statement generation
    /// </summary>
    [HttpPost("{statementId:guid}/cancel")]
    public async Task<ActionResult<bool>> CancelStatementGeneration(Guid statementId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _statementService.CancelStatementGenerationAsync(statementId, userId);
            
            if (result)
            {
                return Ok(new { Success = true, Message = "Statement generation cancelled" });
            }
            
            return BadRequest(new { Success = false, Message = "Failed to cancel statement generation" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling statement {StatementId}", statementId);
            return StatusCode(500, "An error occurred while cancelling statement generation");
        }
    }

    /// <summary>
    /// Get available statement templates
    /// </summary>
    [HttpGet("templates")]
    public async Task<ActionResult<List<StatementTemplate>>> GetAvailableTemplates()
    {
        try
        {
            var templates = await _statementService.GetAvailableTemplatesAsync();
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving statement templates");
            return StatusCode(500, "An error occurred while retrieving statement templates");
        }
    }

    /// <summary>
    /// Validate statement request
    /// </summary>
    [HttpPost("validate")]
    public async Task<ActionResult<ValidationResult>> ValidateStatementRequest([FromBody] GenerateStatementRequest request)
    {
        try
        {
            var (isValid, errors) = await _statementService.ValidateStatementRequestAsync(request);
            
            return Ok(new ValidationResult
            {
                IsValid = isValid,
                Errors = errors
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating statement request");
            return StatusCode(500, "An error occurred while validating the statement request");
        }
    }

    /// <summary>
    /// Get available transaction categories for filtering
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<List<string>>> GetTransactionCategories()
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transaction categories");
            return StatusCode(500, "An error occurred while retrieving transaction categories");
        }
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
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating statement statistics for account {AccountId}", accountId);
            return StatusCode(500, "An error occurred while generating statement statistics");
        }
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

