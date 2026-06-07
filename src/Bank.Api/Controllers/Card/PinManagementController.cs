using Bank.Api.Helpers;
using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers.Card;

/// <summary>
/// Controller for managing card PIN operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PinManagementController : ControllerBase
{
    private readonly IPinManagementService _pinManagementService;
    private readonly ILogger<PinManagementController> _logger;

    public PinManagementController(
        IPinManagementService pinManagementService,
        ILogger<PinManagementController> logger)
    {
        _pinManagementService = pinManagementService;
        _logger = logger;
    }

    /// <summary>
    /// Set PIN for a card (initial PIN setup)
    /// </summary>
    [HttpPost("set-pin")]
    public async Task<ActionResult<PinOperationResponse>> SetPin([FromBody] SetPinRequest request)
    {
        try
        {
            var userId = this.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            var result = await _pinManagementService.SetPinAsync(request, userId);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting PIN for card {CardId}", request.CardId);
            return StatusCode(500, new PinOperationResponse
            {
                Success = false,
                Message = "An error occurred while setting PIN"
            });
        }
    }

    /// <summary>
    /// Change existing PIN for a card
    /// </summary>
    [HttpPost("change-pin")]
    public async Task<ActionResult<PinOperationResponse>> ChangePin([FromBody] SetPinRequest request)
    {
        try
        {
            var userId = this.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            var result = await _pinManagementService.ChangePinAsync(request, userId);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing PIN for card {CardId}", request.CardId);
            return StatusCode(500, new PinOperationResponse
            {
                Success = false,
                Message = "An error occurred while changing PIN"
            });
        }
    }

    /// <summary>
    /// Reset PIN for a card (requires verification)
    /// </summary>
    [HttpPost("reset-pin")]
    public async Task<ActionResult<PinOperationResponse>> ResetPin([FromBody] ResetPinRequest request)
    {
        try
        {
            var userId = this.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            var result = await _pinManagementService.ResetPinAsync(request, userId);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting PIN for card {CardId}", request.CardId);
            return StatusCode(500, new PinOperationResponse
            {
                Success = false,
                Message = "An error occurred while resetting PIN"
            });
        }
    }

    /// <summary>
    /// Verify PIN for a card
    /// </summary>
    [HttpPost("verify-pin")]
    public async Task<ActionResult<PinVerificationResult>> VerifyPin([FromBody] VerifyPinRequest request)
    {
        try
        {
            var userId = this.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            var result = await _pinManagementService.VerifyPinAsync(request, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying PIN for card {CardId}", request.CardId);
            return StatusCode(500, new PinVerificationResult
            {
                IsValid = false,
                Message = "An error occurred while verifying PIN"
            });
        }
    }

    /// <summary>
    /// Generate verification code for PIN reset
    /// </summary>
    [HttpPost("generate-verification-code")]
    public async Task<ActionResult<object>> GenerateVerificationCode([FromBody] GenerateVerificationCodeRequest request)
    {
        try
        {
            var userId = this.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            await _pinManagementService.GenerateVerificationCodeAsync(
                request.CardId, 
                request.VerificationMethod, 
                userId);

            return Ok(new { success = true, message = "Verification code sent successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating verification code for card {CardId}", request.CardId);
            return StatusCode(500, new { success = false, message = "An error occurred while generating verification code" });
        }
    }

    /// <summary>
    /// Unblock card
    /// </summary>
    [HttpPost("unblock-card")]
    public async Task<ActionResult<PinOperationResponse>> UnblockCard([FromBody] UnblockCardRequest request)
    {
        try
        {
            var userId = this.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            var result = await _pinManagementService.UnblockCardAsync(request.CardId, userId);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unblocking card {CardId}", request.CardId);
            return StatusCode(500, new PinOperationResponse
            {
                Success = false,
                Message = "An error occurred while unblocking card"
            });
        }
    }

    /// <summary>
    /// Check if card has PIN set
    /// </summary>
    [HttpGet("has-pin/{cardId}")]
    public async Task<ActionResult<object>> HasPinSet(string cardId)
    {
        try
        {
            var userId = this.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            var hasPinSet = await _pinManagementService.HasPinSetAsync(cardId, userId);
            return Ok(new { hasPinSet });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking PIN status for card {CardId}", cardId);
            return StatusCode(500, new { error = "An error occurred while checking PIN status" });
        }
    }
}

/// <summary>
/// Request to generate verification code
/// </summary>
public class GenerateVerificationCodeRequest
{
    public string CardId { get; set; } = string.Empty;
    public string VerificationMethod { get; set; } = string.Empty; // SMS, Email
}

/// <summary>
/// Request to unblock card
/// </summary>
public class UnblockCardRequest
{
    public string CardId { get; set; } = string.Empty;
}