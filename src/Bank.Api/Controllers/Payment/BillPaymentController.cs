using Bank.Api.Helpers;
using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Bank.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers.Payment;

/// <summary>
/// Controller for bill payment operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BillPaymentController : ControllerBase
{
    private readonly IBillPaymentService _billPaymentService;

    public BillPaymentController(
        IBillPaymentService billPaymentService)
    {
        _billPaymentService = billPaymentService;
    }

    /// <summary>
    /// Get all available billers
    /// </summary>
    [HttpGet("billers")]
    public async Task<ActionResult<List<BillerDto>>> GetAvailableBillers()
    {
        var billers = await _billPaymentService.GetAvailableBillersAsync();
        return Ok(billers);
    }

    /// <summary>
    /// Get billers by category
    /// </summary>
    [HttpGet("billers/category/{category}")]
    public async Task<ActionResult<List<BillerDto>>> GetBillersByCategory(BillerCategory category)
    {
        var billers = await _billPaymentService.GetBillersByCategoryAsync(category);
        return Ok(billers);
    }

    /// <summary>
    /// Search billers
    /// </summary>
    [HttpPost("billers/search")]
    public async Task<ActionResult<List<BillerDto>>> SearchBillers([FromBody] BillerSearchRequest request)
    {
        var billers = await _billPaymentService.SearchBillersAsync(request);
        return Ok(billers);
    }

    /// <summary>
    /// Get biller details by ID
    /// </summary>
    [HttpGet("billers/{billerId}")]
    public async Task<ActionResult<BillerDto>> GetBillerById(Guid billerId)
    {
        var biller = await _billPaymentService.GetBillerByIdAsync(billerId);
        if (biller == null)
        {
            return NotFound("Biller not found");
        }
        return Ok(biller);
    }

    /// <summary>
    /// Schedule a bill payment
    /// </summary>
    [HttpPost("schedule")]
    public async Task<ActionResult<ScheduleBillPaymentResponse>> ScheduleBillPayment([FromBody] ScheduleBillPaymentRequest request)
    {
        var customerId = this.GetCurrentUserId();
        var response = await _billPaymentService.ScheduleBillPaymentAsync(customerId, request);
        
        if (response.Status == BillPaymentStatus.Failed)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Get bill payment history for the current customer
    /// </summary>
    [HttpPost("history")]
    public async Task<ActionResult<Bank.Domain.Common.PagedResult<BillPaymentHistoryDto>>> GetBillPaymentHistory([FromBody] BillPaymentHistoryRequest request)
    {
        var customerId = this.GetCurrentUserId();
        var history = await _billPaymentService.GetBillPaymentHistoryAsync(customerId, request);
        return Ok(history);
    }

    /// <summary>
    /// Get pending bill payments for the current customer
    /// </summary>
    [HttpGet("pending")]
    public async Task<ActionResult<List<BillPaymentDto>>> GetPendingBillPayments()
    {
        var customerId = this.GetCurrentUserId();
        var payments = await _billPaymentService.GetPendingBillPaymentsAsync(customerId);
        return Ok(payments);
    }

    /// <summary>
    /// Get bill payment details by ID
    /// </summary>
    [HttpGet("{paymentId}")]
    public async Task<ActionResult<BillPaymentDto>> GetBillPaymentById(Guid paymentId)
    {
        var customerId = this.GetCurrentUserId();
        var payment = await _billPaymentService.GetBillPaymentByIdAsync(customerId, paymentId);
        
        if (payment == null)
        {
            return NotFound("Payment not found");
        }

        return Ok(payment);
    }

    /// <summary>
    /// Cancel a scheduled bill payment
    /// </summary>
    [HttpPost("{paymentId}/cancel")]
    public async Task<ActionResult> CancelScheduledPayment(Guid paymentId)
    {
        var customerId = this.GetCurrentUserId();
        var success = await _billPaymentService.CancelScheduledPaymentAsync(customerId, paymentId);
        
        if (!success)
        {
            return BadRequest("Payment cannot be cancelled");
        }

        return Ok(new { message = "Payment cancelled successfully" });
    }

    /// <summary>
    /// Update a scheduled bill payment
    /// </summary>
    [HttpPut("{paymentId}")]
    public async Task<ActionResult> UpdateScheduledPayment(Guid paymentId, [FromBody] UpdateBillPaymentRequest request)
    {
        var customerId = this.GetCurrentUserId();
        var success = await _billPaymentService.UpdateScheduledPaymentAsync(customerId, paymentId, request);
        
        if (!success)
        {
            return BadRequest("Payment cannot be updated");
        }

        return Ok(new { message = "Payment updated successfully" });
    }
}