using Bank.Api.Constants;
using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Bank.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bank.Api.Controllers.Account;

/// <summary>
/// Controller for managing interest calculations and applications
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InterestCalculationController : ControllerBase
{
    private readonly IInterestCalculationService _interestCalculationService;
    private readonly IAccountService _accountService;

    public InterestCalculationController(
        IInterestCalculationService interestCalculationService,
        IAccountService accountService)
    {
        _interestCalculationService = interestCalculationService;
        _accountService = accountService;
    }

    /// <summary>
    /// Calculate simple interest for an account
    /// </summary>
    [HttpPost("calculate-simple")]
    public async Task<ActionResult<InterestCalculationResult>> CalculateSimpleInterest([FromBody] InterestCalculationRequest request)
    {
        var userId = GetCurrentUserId();
        
        // Verify user has access to the account
        var hasAccess = await _accountService.CanUserAccessAccountAsync(request.AccountId, userId);
        if (!hasAccess)
        {
            return Forbid(ErrorMessages.YouDontHaveAccessToThisAccount);
        }

        var account = await _accountService.GetAccountByIdAsync(request.AccountId);
        if (account == null)
        {
            return NotFound(ErrorMessages.AccountNotFound);
        }

        var interestAmount = await _interestCalculationService.CalculateSimpleInterestAsync(
            account, request.FromDate, request.ToDate);

        var result = new InterestCalculationResult
        {
            Success = true,
            InterestAmount = interestAmount,
            PrincipalAmount = account.Balance,
            InterestRate = account.InterestRate,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            DaysCalculated = (int)(request.ToDate - request.FromDate).TotalDays,
            CompoundingFrequency = InterestCompoundingFrequency.Annually // Simple interest
        };

        return Ok(result);
    }

    /// <summary>
    /// Calculate compound interest for an account
    /// </summary>
    [HttpPost("calculate-compound")]
    public async Task<ActionResult<InterestCalculationResult>> CalculateCompoundInterest([FromBody] InterestCalculationRequest request)
    {
        var userId = GetCurrentUserId();
        
        // Verify user has access to the account
        var hasAccess = await _accountService.CanUserAccessAccountAsync(request.AccountId, userId);
        if (!hasAccess)
        {
            return Forbid(ErrorMessages.YouDontHaveAccessToThisAccount);
        }

        var account = await _accountService.GetAccountByIdAsync(request.AccountId);
        if (account == null)
        {
            return NotFound(ErrorMessages.AccountNotFound);
        }

        var compoundingFrequency = request.CompoundingFrequency ?? (int)account.CompoundingFrequency;
        var interestAmount = await _interestCalculationService.CalculateCompoundInterestAsync(
            account, request.FromDate, request.ToDate, compoundingFrequency);

        var result = new InterestCalculationResult
        {
            Success = true,
            InterestAmount = interestAmount,
            PrincipalAmount = account.Balance,
            InterestRate = account.InterestRate,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            DaysCalculated = (int)(request.ToDate - request.FromDate).TotalDays,
            CompoundingFrequency = account.CompoundingFrequency
        };

        return Ok(result);
    }

    /// <summary>
    /// Calculate daily interest for an account
    /// </summary>
    [HttpPost("calculate-daily")]
    public async Task<ActionResult<InterestCalculationResult>> CalculateDailyInterest([FromBody] InterestCalculationRequest request)
    {
        var userId = GetCurrentUserId();
        
        // Verify user has access to the account
        var hasAccess = await _accountService.CanUserAccessAccountAsync(request.AccountId, userId);
        if (!hasAccess)
        {
            return Forbid(ErrorMessages.YouDontHaveAccessToThisAccount);
        }

        var account = await _accountService.GetAccountByIdAsync(request.AccountId);
        if (account == null)
        {
            return NotFound(ErrorMessages.AccountNotFound);
        }

        var interestAmount = await _interestCalculationService.CalculateDailyInterestAsync(
            account, request.FromDate);

        var result = new InterestCalculationResult
        {
            Success = true,
            InterestAmount = interestAmount,
            PrincipalAmount = account.Balance,
            InterestRate = account.InterestRate,
            FromDate = request.FromDate,
            ToDate = request.FromDate,
            DaysCalculated = 1,
            CompoundingFrequency = InterestCompoundingFrequency.Daily
        };

        return Ok(result);
    }

    /// <summary>
    /// Apply interest to an account
    /// </summary>
    [HttpPost("apply")]
    public async Task<ActionResult<InterestCalculationResult>> ApplyInterest([FromBody] ApplyInterestRequest request)
    {
        var userId = GetCurrentUserId();
        
        // Verify user has access to the account
        var hasAccess = await _accountService.CanUserAccessAccountAsync(request.AccountId, userId);
        if (!hasAccess)
        {
            return Forbid(ErrorMessages.YouDontHaveAccessToThisAccount);
        }

        var success = await _interestCalculationService.ApplyInterestAsync(request.AccountId, request.UserId);
        
        var result = new InterestCalculationResult
        {
            Success = success,
            Message = success ? "Interest applied successfully" : "Failed to apply interest"
        };

        return Ok(result);
    }

    /// <summary>
    /// Update interest rate for an account (Admin only)
    /// </summary>
    [HttpPut("update-rate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<InterestCalculationResult>> UpdateInterestRate([FromBody] UpdateInterestRateRequest request)
    {
        var success = await _interestCalculationService.UpdateInterestRateAsync(
            request.AccountId, request.NewInterestRate, request.UserId);
        
        var result = new InterestCalculationResult
        {
            Success = success,
            Message = success ? "Interest rate updated successfully" : "Failed to update interest rate",
            InterestRate = request.NewInterestRate
        };

        return Ok(result);
    }

    /// <summary>
    /// Get interest rate for account type and balance
    /// </summary>
    [HttpGet("rate/{accountType}/{balance}")]
    public async Task<ActionResult<decimal>> GetInterestRate(AccountType accountType, decimal balance)
    {
        var rate = await _interestCalculationService.GetInterestRateAsync(accountType, balance);
        return Ok(rate);
    }

    /// <summary>
    /// Get accounts eligible for interest processing
    /// </summary>
    [HttpGet("eligible-accounts")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<AccountDto>>> GetAccountsForInterestProcessing()
    {
        var accounts = await _interestCalculationService.GetAccountsForInterestProcessingAsync();
        var accountDtos = accounts.Select(a => new AccountDto
        {
            Id = a.Id,
            AccountNumber = a.AccountNumber,
            AccountHolderName = a.AccountHolderName,
            Balance = a.Balance,
            InterestRate = a.InterestRate,
            LastInterestCalculationDate = a.LastInterestCalculationDate,
            CompoundingFrequency = a.CompoundingFrequency
        }).ToList();

        return Ok(accountDtos);
    }

    /// <summary>
    /// Process monthly interest for all eligible accounts (Admin only)
    /// </summary>
    [HttpPost("process-monthly")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<MonthlyInterestProcessingSummary>> ProcessMonthlyInterest()
    {
        var success = await _interestCalculationService.ProcessMonthlyInterestAsync();
        
        var summary = new MonthlyInterestProcessingSummary
        {
            Success = success,
            Message = success ? "Monthly interest processing completed" : "Monthly interest processing failed",
            ProcessingDate = DateTime.UtcNow
        };

        return Ok(summary);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}