using Bank.Api.Constants;
using Bank.Api.Helpers;
using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bank.Api.Controllers.Payment;

/// <summary>
/// Controller for managing beneficiaries (payees) for fund transfers
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BeneficiaryController : ControllerBase
{
    private readonly IBeneficiaryService _beneficiaryService;
    private readonly ILogger<BeneficiaryController> _logger;

    public BeneficiaryController(
        IBeneficiaryService beneficiaryService,
        ILogger<BeneficiaryController> logger)
    {
        _beneficiaryService = beneficiaryService;
        _logger = logger;
    }

    /// <summary>
    /// Add a new beneficiary
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BeneficiaryResult>> AddBeneficiary([FromBody] AddBeneficiaryRequest request)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var result = await _beneficiaryService.AddBeneficiaryAsync(currentUserId, request);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding beneficiary");
            return StatusCode(500, new BeneficiaryResult 
            { 
                Success = false, 
                Message = "An error occurred while adding beneficiary" 
            });
        }
    }

    /// <summary>
    /// Update beneficiary information
    /// </summary>
    [HttpPut("{beneficiaryId}")]
    public async Task<ActionResult<BeneficiaryResult>> UpdateBeneficiary(Guid beneficiaryId, [FromBody] UpdateBeneficiaryRequest request)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var result = await _beneficiaryService.UpdateBeneficiaryAsync(beneficiaryId, request, currentUserId);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating beneficiary {BeneficiaryId}", beneficiaryId);
            return StatusCode(500, new BeneficiaryResult 
            { 
                Success = false, 
                Message = "An error occurred while updating beneficiary" 
            });
        }
    }

    /// <summary>
    /// Get beneficiary by ID
    /// </summary>
    [HttpGet("{beneficiaryId}")]
    public async Task<ActionResult<BeneficiaryDto>> GetBeneficiary(Guid beneficiaryId)
    {
        try
        {
            var beneficiary = await _beneficiaryService.GetBeneficiaryByIdAsync(beneficiaryId);
            
            if (beneficiary == null)
            {
                return NotFound(ErrorMessages.AccountNotFound);
            }

            // Verify user has access to this beneficiary
            var currentUserId = GetCurrentUserId();
            if (beneficiary.CustomerId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid(ErrorMessages.YouDontHaveAccessToThisAccount);
            }

            return Ok(beneficiary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving beneficiary {BeneficiaryId}", beneficiaryId);
            return StatusCode(500, "An error occurred while retrieving beneficiary");
        }
    }

    /// <summary>
    /// Get all beneficiaries for current user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<BeneficiaryDto>>> GetMyBeneficiaries([FromQuery] bool activeOnly = true)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var beneficiaries = await _beneficiaryService.GetCustomerBeneficiariesAsync(currentUserId, activeOnly);
            
            return Ok(beneficiaries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving beneficiaries for user {UserId}", GetCurrentUserId());
            return StatusCode(500, "An error occurred while retrieving beneficiaries");
        }
    }

    /// <summary>
    /// Search beneficiaries with criteria
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<BeneficiarySearchResult>> SearchBeneficiaries([FromBody] BeneficiarySearchCriteria criteria)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            
            // Ensure user can only search their own beneficiaries unless admin
            if (criteria.CustomerId != currentUserId && !User.IsInRole("Admin"))
            {
                criteria.CustomerId = currentUserId;
            }

            var result = await _beneficiaryService.SearchBeneficiariesAsync(criteria);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching beneficiaries");
            return StatusCode(500, "An error occurred while searching beneficiaries");
        }
    }

    /// <summary>
    /// Verify a beneficiary account
    /// </summary>
    [HttpPost("{beneficiaryId}/verify")]
    public async Task<ActionResult<BeneficiaryVerificationResult>> VerifyBeneficiary(Guid beneficiaryId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var result = await _beneficiaryService.VerifyBeneficiaryAsync(beneficiaryId, currentUserId);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying beneficiary {BeneficiaryId}", beneficiaryId);
            return StatusCode(500, new BeneficiaryVerificationResult 
            { 
                Success = false, 
                Message = "An error occurred while verifying beneficiary" 
            });
        }
    }

    /// <summary>
    /// Archive a beneficiary
    /// </summary>
    [HttpPost("{beneficiaryId}/archive")]
    public async Task<ActionResult<bool>> ArchiveBeneficiary(Guid beneficiaryId, [FromBody] string reason)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var success = await _beneficiaryService.ArchiveBeneficiaryAsync(beneficiaryId, reason, currentUserId);
            
            if (success)
            {
                return Ok(new { Success = true, Message = "Beneficiary archived successfully" });
            }

            return BadRequest(new { Success = false, Message = "Failed to archive beneficiary" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving beneficiary {BeneficiaryId}", beneficiaryId);
            return StatusCode(500, new { Success = false, Message = "An error occurred while archiving beneficiary" });
        }
    }

    /// <summary>
    /// Reactivate an archived beneficiary
    /// </summary>
    [HttpPost("{beneficiaryId}/reactivate")]
    public async Task<ActionResult<bool>> ReactivateBeneficiary(Guid beneficiaryId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var success = await _beneficiaryService.ReactivateBeneficiaryAsync(beneficiaryId, currentUserId);
            
            if (success)
            {
                return Ok(new { Success = true, Message = "Beneficiary reactivated successfully" });
            }

            return BadRequest(new { Success = false, Message = "Failed to reactivate beneficiary" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reactivating beneficiary {BeneficiaryId}", beneficiaryId);
            return StatusCode(500, new { Success = false, Message = "An error occurred while reactivating beneficiary" });
        }
    }

    /// <summary>
    /// Get transfer history for a beneficiary
    /// </summary>
    [HttpGet("{beneficiaryId}/transfers")]
    public async Task<ActionResult<BeneficiaryTransferHistory>> GetTransferHistory(
        Guid beneficiaryId, 
        [FromQuery] DateTime? fromDate = null, 
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var history = await _beneficiaryService.GetTransferHistoryAsync(beneficiaryId, fromDate, toDate);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transfer history for beneficiary {BeneficiaryId}", beneficiaryId);
            return StatusCode(500, "An error occurred while retrieving transfer history");
        }
    }

    /// <summary>
    /// Get beneficiary statistics for current user
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<BeneficiaryStatistics>> GetBeneficiaryStatistics()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var statistics = await _beneficiaryService.GetBeneficiaryStatisticsAsync(currentUserId);
            
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving beneficiary statistics for user {UserId}", GetCurrentUserId());
            return StatusCode(500, "An error occurred while retrieving statistics");
        }
    }

    /// <summary>
    /// Validate account details before adding beneficiary
    /// </summary>
    [HttpPost("validate")]
    public async Task<ActionResult<BeneficiaryVerificationResult>> ValidateAccountDetails([FromBody] AddBeneficiaryRequest request)
    {
        try
        {
            var result = await _beneficiaryService.ValidateAccountDetailsAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating account details");
            return StatusCode(500, new BeneficiaryVerificationResult 
            { 
                Success = false, 
                Message = "An error occurred while validating account details" 
            });
        }
    }

    /// <summary>
    /// Update transfer limits for a beneficiary
    /// </summary>
    [HttpPut("{beneficiaryId}/limits")]
    public async Task<ActionResult<bool>> UpdateTransferLimits(
        Guid beneficiaryId, 
        [FromBody] UpdateTransferLimitsRequest request)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var success = await _beneficiaryService.UpdateTransferLimitsAsync(
                beneficiaryId, 
                request.DailyLimit, 
                request.MonthlyLimit, 
                request.SingleLimit, 
                currentUserId);
            
            if (success)
            {
                return Ok(new { Success = true, Message = "Transfer limits updated successfully" });
            }

            return BadRequest(new { Success = false, Message = "Failed to update transfer limits" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating transfer limits for beneficiary {BeneficiaryId}", beneficiaryId);
            return StatusCode(500, new { Success = false, Message = "An error occurred while updating transfer limits" });
        }
    }

    /// <summary>
    /// Check if beneficiary can receive transfers
    /// </summary>
    [HttpGet("{beneficiaryId}/can-receive-transfers")]
    public async Task<ActionResult<bool>> CanReceiveTransfers(Guid beneficiaryId)
    {
        try
        {
            var canReceive = await _beneficiaryService.CanReceiveTransfersAsync(beneficiaryId);
            return Ok(new { CanReceiveTransfers = canReceive });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking transfer eligibility for beneficiary {BeneficiaryId}", beneficiaryId);
            return StatusCode(500, "An error occurred while checking transfer eligibility");
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value 
            ?? User.FindFirst("id")?.Value;

        return userIdClaim != null && Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}

