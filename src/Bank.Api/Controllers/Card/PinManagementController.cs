using Bank.Api.Helpers;
using Bank.Api.Constants;
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

    public PinManagementController(
        IPinManagementService pinManagementService)
    {
        _pinManagementService = pinManagementService;
    }

    /// <summary>
    /// Set PIN for a card (initial PIN setup)
    /// </summary>
    [HttpPost("set-pin")]
    public async Task<ActionResult<PinOperationResponse>> SetPin([FromBody] SetPinRequest request)
    {
        var userId = this.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiConstants.UserNotAuthenticatedMessage);
        }

        var result = await _pinManagementService.SetPinAsync(request, userId);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Change existing PIN for a card
    /// </summary>
    [HttpPost("change-pin")]
    public async Task<ActionResult<PinOperationResponse>> ChangePin([FromBody] SetPinRequest request)
    {
        var userId = this.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiConstants.UserNotAuthenticatedMessage);
        }

        var result = await _pinManagementService.ChangePinAsync(request, userId);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Reset PIN for a card (requires verification)
    /// </summary>
    [HttpPost("reset-pin")]
    public async Task<ActionResult<PinOperationResponse>> ResetPin([FromBody] ResetPinRequest request)
    {
        var userId = this.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiConstants.UserNotAuthenticatedMessage);
        }

        var result = await _pinManagementService.ResetPinAsync(request, userId);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Verify PIN for a card
    /// </summary>
    [HttpPost("verify-pin")]
    public async Task<ActionResult<PinVerificationResult>> VerifyPin([FromBody] VerifyPinRequest request)
    {
        var userId = this.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiConstants.UserNotAuthenticatedMessage);
        }

        var result = await _pinManagementService.VerifyPinAsync(request, userId);
        return Ok(result);
    }

    /// <summary>
    /// Generate verification code for PIN reset
    /// </summary>
    [HttpPost("generate-verification-code")]
    public async Task<ActionResult<object>> GenerateVerificationCode([FromBody] GenerateVerificationCodeRequest request)
    {
        var userId = this.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiConstants.UserNotAuthenticatedMessage);
        }

        await _pinManagementService.GenerateVerificationCodeAsync(
            request.CardId, 
            request.VerificationMethod, 
            userId);

        return Ok(new { success = true, message = "Verification code sent successfully" });
    }

    /// <summary>
    /// Unblock card
    /// </summary>
    [HttpPost("unblock-card")]
    public async Task<ActionResult<PinOperationResponse>> UnblockCard([FromBody] UnblockCardRequest request)
    {
        var userId = this.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiConstants.UserNotAuthenticatedMessage);
        }

        var result = await _pinManagementService.UnblockCardAsync(request.CardId, userId);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Check if card has PIN set
    /// </summary>
    [HttpGet("has-pin/{cardId}")]
    public async Task<ActionResult<object>> HasPinSet(string cardId)
    {
        var userId = this.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiConstants.UserNotAuthenticatedMessage);
        }

        var hasPinSet = await _pinManagementService.HasPinSetAsync(cardId, userId);
        return Ok(new { hasPinSet });
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