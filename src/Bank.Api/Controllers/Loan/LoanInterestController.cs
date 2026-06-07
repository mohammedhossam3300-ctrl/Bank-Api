using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Bank.Domain.Enums;
using Bank.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers.Loan;

/// <summary>
/// Controller for loan interest calculations and amortization
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LoanInterestController : ControllerBase
{
    private readonly ILoanInterestCalculationService _loanInterestService;
    private readonly ILoanService _loanService;
    private readonly ILoanRepository _loanRepository;

    public LoanInterestController(
        ILoanInterestCalculationService loanInterestService,
        ILoanService loanService,
        ILoanRepository loanRepository)
    {
        _loanInterestService = loanInterestService;
        _loanService = loanService;
        _loanRepository = loanRepository;
    }

    /// <summary>
    /// Calculate monthly payment for loan parameters
    /// </summary>
    [HttpPost("calculate-monthly-payment")]
    public async Task<ActionResult<decimal>> CalculateMonthlyPayment([FromBody] MonthlyPaymentRequest request)
    {
        var monthlyPayment = await _loanInterestService.CalculateMonthlyPaymentAsync(
            request.Principal, request.AnnualRate, request.TermInMonths, request.CalculationMethod);

        return Ok(monthlyPayment);
    }

    /// <summary>
    /// Generate amortization schedule for a loan
    /// </summary>
    [HttpGet("{loanId}/amortization-schedule")]
    public async Task<ActionResult<AmortizationSchedule>> GetAmortizationSchedule(Guid loanId)
    {
        var loan = await _loanService.GetLoanByIdAsync(loanId);
        if (loan == null)
        {
            return NotFound($"Loan {loanId} not found");
        }

        // Get the actual loan entity for calculations
        var loanEntity = await GetLoanEntityAsync(loanId);
        if (loanEntity == null)
        {
            return NotFound($"Loan entity {loanId} not found");
        }

        var schedule = await _loanInterestService.GenerateAmortizationScheduleAsync(loanEntity);
        return Ok(schedule);
    }

    /// <summary>
    /// Calculate early payoff amount for a loan
    /// </summary>
    [HttpPost("{loanId}/early-payoff")]
    public async Task<ActionResult<EarlyPayoffCalculation>> CalculateEarlyPayoff(Guid loanId, [FromBody] EarlyPayoffRequest request)
    {
        var calculation = await _loanInterestService.CalculateEarlyPayoffAmountAsync(loanId, request.PayoffDate);
        return Ok(calculation);
    }

    /// <summary>
    /// Get interest rate for loan type and credit score
    /// </summary>
    [HttpPost("get-interest-rate")]
    public async Task<ActionResult<decimal>> GetInterestRate([FromBody] InterestRateRequest request)
    {
        var rate = await _loanInterestService.GetInterestRateForLoanTypeAsync(
            request.LoanType, request.CreditScore, request.LoanAmount);

        return Ok(rate);
    }

    /// <summary>
    /// Get loan type configuration
    /// </summary>
    [HttpGet("loan-type-config/{loanType}")]
    public async Task<ActionResult<LoanTypeConfiguration>> GetLoanTypeConfiguration(LoanType loanType)
    {
        var config = await _loanInterestService.GetLoanTypeConfigurationAsync(loanType);
        return Ok(config);
    }

    /// <summary>
    /// Calculate remaining interest for a loan
    /// </summary>
    [HttpGet("{loanId}/remaining-interest")]
    public async Task<ActionResult<decimal>> GetRemainingInterest(Guid loanId)
    {
        var loanEntity = await GetLoanEntityAsync(loanId);
        if (loanEntity == null)
        {
            return NotFound($"Loan {loanId} not found");
        }

        var remainingInterest = await _loanInterestService.CalculateRemainingInterestAsync(loanEntity);
        return Ok(remainingInterest);
    }

    /// <summary>
    /// Update loan interest rate (Admin only)
    /// </summary>
    [HttpPut("{loanId}/update-rate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> UpdateInterestRate(Guid loanId, [FromBody] UpdateLoanRateRequest request)
    {
        var userId = GetCurrentUserId();
        var success = await _loanInterestService.UpdateLoanInterestRateAsync(loanId, request.NewRate, userId);
        
        if (success)
        {
            return Ok(new { Success = true, Message = "Interest rate updated successfully" });
        }
        
        return BadRequest("Failed to update interest rate");
    }

    #region Private Helper Methods

    private async Task<Domain.Entities.Loan?> GetLoanEntityAsync(Guid loanId)
    {
        return await _loanRepository.GetByIdAsync(loanId);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("id");
        return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : Guid.Empty;
    }

    #endregion
}

