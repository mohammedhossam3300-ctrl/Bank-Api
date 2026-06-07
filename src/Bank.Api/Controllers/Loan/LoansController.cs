using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Bank.Application.Validators;
using Bank.Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bank.Api.Controllers.Loan;

/// <summary>
/// Controller for loan management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;
    private readonly IValidator<LoanApplicationRequest> _loanApplicationValidator;
    private readonly IValidator<LoanPaymentRequest> _loanPaymentValidator;
    private readonly IValidator<ApprovalDecision> _approvalDecisionValidator;

    public LoansController(
        ILoanService loanService,
        IValidator<LoanApplicationRequest> loanApplicationValidator,
        IValidator<LoanPaymentRequest> loanPaymentValidator,
        IValidator<ApprovalDecision> approvalDecisionValidator)
    {
        _loanService = loanService;
        _loanApplicationValidator = loanApplicationValidator;
        _loanPaymentValidator = loanPaymentValidator;
        _approvalDecisionValidator = approvalDecisionValidator;
    }

    /// <summary>
    /// Submit a new loan application
    /// </summary>
    [HttpPost("apply")]
    public async Task<ActionResult<LoanApplicationResult>> SubmitApplication([FromBody] LoanApplicationRequest request)
    {
        // Validate the request
        var validationResult = await _loanApplicationValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new LoanApplicationResult
            {
                IsSuccess = false,
                Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
            });
        }

        var customerId = GetCurrentUserId();
        var result = await _loanService.SubmitApplicationAsync(customerId, request);
        
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Get all loans for the current customer
    /// </summary>
    [HttpGet("my-loans")]
    public async Task<ActionResult<List<LoanDto>>> GetMyLoans()
    {
        var customerId = GetCurrentUserId();
        var loans = await _loanService.GetCustomerLoansAsync(customerId);
        return Ok(loans);
    }

    /// <summary>
    /// Get loan details by ID
    /// </summary>
    [HttpGet("{loanId}")]
    public async Task<ActionResult<LoanDto>> GetLoan(Guid loanId)
    {
        var loan = await _loanService.GetLoanByIdAsync(loanId);
        
        if (loan == null)
        {
            return NotFound(new { Message = "Loan not found" });
        }

        // Ensure customer can only access their own loans (admins bypass this check)
        var customerId = GetCurrentUserId();
        if (!User.IsInRole("Admin") && loan.CustomerId != customerId)
        {
            return Forbid();
        }

        return Ok(loan);
    }

    /// <summary>
    /// Generate repayment schedule for a loan
    /// </summary>
    [HttpGet("{loanId}/repayment-schedule")]
    public async Task<ActionResult<RepaymentSchedule>> GetRepaymentSchedule(Guid loanId)
    {
        var schedule = await _loanService.GenerateRepaymentScheduleAsync(loanId);
        
        if (schedule.LoanId == Guid.Empty)
        {
            return NotFound(new { Message = "Loan not found" });
        }

        return Ok(schedule);
    }

    /// <summary>
    /// Get loan payment history
    /// </summary>
    [HttpGet("{loanId}/payments")]
    public async Task<ActionResult<List<Domain.Entities.LoanPayment>>> GetPaymentHistory(Guid loanId)
    {
        var payments = await _loanService.GetLoanPaymentHistoryAsync(loanId);
        return Ok(payments);
    }

    /// <summary>
    /// Make a loan payment
    /// </summary>
    [HttpPost("payments")]
    public async Task<ActionResult<PaymentResult>> MakePayment([FromBody] LoanPaymentRequest request)
    {
        // Validate the request
        var validationResult = await _loanPaymentValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new PaymentResult
            {
                IsSuccess = false,
                Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
            });
        }

        var customerId = GetCurrentUserId();
        var result = await _loanService.ProcessPaymentAsync(request, customerId);
        
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Get next payment details for a loan
    /// </summary>
    [HttpGet("{loanId}/next-payment")]
    public async Task<ActionResult<RepaymentScheduleEntry>> GetNextPayment(Guid loanId)
    {
        var nextPayment = await _loanService.GetNextPaymentDetailsAsync(loanId);
        
        if (nextPayment == null)
        {
            return NotFound(new { Message = "No upcoming payments found" });
        }

        return Ok(nextPayment);
    }

    /// <summary>
    /// Search loans (admin only)
    /// </summary>
    [HttpPost("search")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<ActionResult<object>> SearchLoans([FromBody] LoanSearchRequest request)
    {
        var (loans, totalCount) = await _loanService.SearchLoansAsync(request);
        
        return Ok(new
        {
            Loans = loans,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        });
    }

    /// <summary>
    /// Perform credit scoring for a loan (admin only)
    /// </summary>
    [HttpPost("{loanId}/credit-score")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<ActionResult<CreditScoreResult>> PerformCreditScoring(Guid loanId)
    {
        var loan = await _loanService.GetLoanByIdAsync(loanId);
        if (loan == null)
        {
            return NotFound(new { Message = "Loan not found" });
        }

        // Get customer ID from loan (this would need to be retrieved from the loan service)
        // For now, we'll use a placeholder approach
        var result = await _loanService.PerformCreditScoringAsync(Guid.Empty, loanId);
        
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Process loan approval/rejection (admin only)
    /// </summary>
    [HttpPost("{loanId}/approve")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<ActionResult<LoanApprovalResult>> ProcessApproval(Guid loanId, [FromBody] ApprovalDecision decision)
    {
        // Validate the request
        var validationResult = await _approvalDecisionValidator.ValidateAsync(decision);
        if (!validationResult.IsValid)
        {
            return BadRequest(new LoanApprovalResult
            {
                IsSuccess = false,
                Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
            });
        }

        var approvedBy = GetCurrentUserId();
        var result = await _loanService.ProcessApprovalAsync(loanId, decision, approvedBy);
        
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Disburse an approved loan (admin only)
    /// </summary>
    [HttpPost("{loanId}/disburse")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<ActionResult<DisbursementResult>> DisburseLoan(Guid loanId)
    {
        var disbursedBy = GetCurrentUserId();
        var result = await _loanService.DisburseLoanAsync(loanId, disbursedBy);
        
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Process delinquent loans (admin only, typically called by background job)
    /// </summary>
    [HttpPost("process-delinquent")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> ProcessDelinquentLoans()
    {
        var processedCount = await _loanService.ProcessDelinquentLoansAsync();
        
        return Ok(new
        {
            Message = $"Processed {processedCount} delinquent loans",
            ProcessedCount = processedCount,
            ProcessedAt = DateTime.UtcNow
        });
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }
        return userId;
    }
}