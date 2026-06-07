using Bank.Api.Helpers;
using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Bank.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers.Payment;

/// <summary>
/// Controller for bill payment management and integration operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BillPaymentManagementController : ControllerBase
{
    private readonly IBillPaymentService _billPaymentService;
    private readonly IBillerIntegrationService _billerIntegrationService;
    private readonly IPaymentRetryService _paymentRetryService;
    private readonly IPaymentReceiptService _paymentReceiptService;

    public BillPaymentManagementController(
        IBillPaymentService billPaymentService,
        IBillerIntegrationService billerIntegrationService,
        IPaymentRetryService paymentRetryService,
        IPaymentReceiptService paymentReceiptService)
    {
        _billPaymentService = billPaymentService;
        _billerIntegrationService = billerIntegrationService;
        _paymentRetryService = paymentRetryService;
        _paymentReceiptService = paymentReceiptService;
    }

    /// <summary>
    /// Process scheduled bill payments manually (Admin only)
    /// </summary>
    [HttpPost("process-payments")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<ProcessBillPaymentResponse>>> ProcessScheduledPayments([FromQuery] DateTime? processingDate = null)
    {
        var results = await _billPaymentService.ProcessBillPaymentAsync(processingDate);
        return Ok(results);
    }

    /// <summary>
    /// Get payment retry history
    /// </summary>
    [HttpGet("payments/{paymentId}/retries")]
    public async Task<ActionResult<List<PaymentRetryResult>>> GetPaymentRetryHistory(Guid paymentId)
    {
        var customerId = this.GetCurrentUserId();
        
        // Verify payment belongs to customer (unless admin)
        if (!User.IsInRole("Admin"))
        {
            var payment = await _billPaymentService.GetBillPaymentByIdAsync(customerId, paymentId);
            if (payment == null)
            {
                return NotFound("Payment not found");
            }
        }

        var retries = await _paymentRetryService.GetPaymentRetryHistoryAsync(paymentId);
        return Ok(retries);
    }

    /// <summary>
    /// Cancel payment retries
    /// </summary>
    [HttpPost("payments/{paymentId}/cancel-retries")]
    public async Task<ActionResult> CancelPaymentRetries(Guid paymentId)
    {
        var customerId = this.GetCurrentUserId();
        
        // Verify payment belongs to customer (unless admin)
        if (!User.IsInRole("Admin"))
        {
            var payment = await _billPaymentService.GetBillPaymentByIdAsync(customerId, paymentId);
            if (payment == null)
            {
                return NotFound("Payment not found");
            }
        }

        var success = await _paymentRetryService.CancelPaymentRetriesAsync(paymentId);
        if (!success)
        {
            return BadRequest("Failed to cancel payment retries");
        }

        return Ok(new { message = "Payment retries cancelled successfully" });
    }

    /// <summary>
    /// Process retry payments manually (Admin only)
    /// </summary>
    [HttpPost("process-retries")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<PaymentRetryResult>>> ProcessRetryPayments()
    {
        var results = await _paymentRetryService.ProcessRetryPaymentsAsync();
        return Ok(results);
    }

    /// <summary>
    /// Get retry statistics (Admin only)
    /// </summary>
    [HttpGet("retry-statistics")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Dictionary<string, int>>> GetRetryStatistics([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
    {
        var statistics = await _paymentRetryService.GetRetryStatisticsAsync(fromDate, toDate);
        return Ok(statistics);
    }

    /// <summary>
    /// Check biller health
    /// </summary>
    [HttpPost("billers/{billerId}/health-check")]
    public async Task<ActionResult<BillerHealthCheckResponse>> CheckBillerHealth(Guid billerId)
    {
        var healthCheck = await _billerIntegrationService.CheckBillerHealthAsync(billerId);
        return Ok(healthCheck);
    }

    /// <summary>
    /// Get supported payment methods for a biller
    /// </summary>
    [HttpGet("billers/{billerId}/payment-methods")]
    public async Task<ActionResult<List<string>>> GetSupportedPaymentMethods(Guid billerId)
    {
        var methods = await _billerIntegrationService.GetSupportedPaymentMethodsAsync(billerId);
        return Ok(methods);
    }

    /// <summary>
    /// Validate biller account information
    /// </summary>
    [HttpPost("billers/{billerId}/validate-account")]
    public async Task<ActionResult<BillerAccountValidationResponse>> ValidateBillerAccount(Guid billerId, [FromBody] ValidateBillerAccountRequest request)
    {
        var validation = await _billerIntegrationService.ValidateBillerAccountAsync(billerId, request.AccountNumber);
        return Ok(validation);
    }

    /// <summary>
    /// Get bill presentment for customer and biller
    /// </summary>
    [HttpGet("billers/{billerId}/presentments")]
    public async Task<ActionResult<List<BillPresentmentDto>>> GetBillPresentment(Guid billerId)
    {
        var customerId = this.GetCurrentUserId();
        var presentments = await _billerIntegrationService.GetBillPresentmentAsync(customerId, billerId);
        return Ok(presentments);
    }

    /// <summary>
    /// Get payment routing preferences for a biller
    /// </summary>
    [HttpGet("billers/{billerId}/routing-preferences")]
    public async Task<ActionResult<PaymentRoutingPreferences>> GetPaymentRoutingPreferences(Guid billerId)
    {
        var preferences = await _billerIntegrationService.GetPaymentRoutingPreferencesAsync(billerId);
        return Ok(preferences);
    }

    /// <summary>
    /// Synchronize payment status with external systems (Admin only)
    /// </summary>
    [HttpPost("synchronize-status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<PaymentStatusSyncResult>>> SynchronizePaymentStatus([FromBody] List<Guid> paymentIds)
    {
        var results = await _billerIntegrationService.SynchronizePaymentStatusAsync(paymentIds);
        return Ok(results);
    }

    /// <summary>
    /// Get payment receipt by payment ID
    /// </summary>
    [HttpGet("payments/{paymentId}/receipt")]
    public async Task<ActionResult<PaymentReceiptDto>> GetPaymentReceipt(Guid paymentId)
    {
        var customerId = this.GetCurrentUserId();
        
        // Verify payment belongs to customer (unless admin)
        if (!User.IsInRole("Admin"))
        {
            var payment = await _billPaymentService.GetBillPaymentByIdAsync(customerId, paymentId);
            if (payment == null)
            {
                return NotFound("Payment not found");
            }
        }

        var receipt = await _paymentReceiptService.GetReceiptByPaymentIdAsync(paymentId);
        if (receipt == null)
        {
            return NotFound("Receipt not found");
        }

        return Ok(receipt);
    }

    /// <summary>
    /// Get payment receipt by receipt number
    /// </summary>
    [HttpGet("receipts/{receiptNumber}")]
    public async Task<ActionResult<PaymentReceiptDto>> GetReceiptByNumber(string receiptNumber)
    {
        var receipt = await _paymentReceiptService.GetReceiptByNumberAsync(receiptNumber);
        if (receipt == null)
        {
            return NotFound("Receipt not found");
        }

        // Verify receipt belongs to customer (unless admin)
        if (!User.IsInRole("Admin"))
        {
            var customerId = this.GetCurrentUserId();
            if (receipt.CustomerId != customerId)
            {
                return NotFound("Receipt not found");
            }
        }

        return Ok(receipt);
    }

    /// <summary>
    /// Generate receipt PDF
    /// </summary>
    [HttpGet("receipts/{receiptNumber}/pdf")]
    public async Task<ActionResult> GetReceiptPdf(string receiptNumber)
    {
        var receipt = await _paymentReceiptService.GetReceiptByNumberAsync(receiptNumber);
        if (receipt == null)
        {
            return NotFound("Receipt not found");
        }

        // Verify receipt belongs to customer (unless admin)
        if (!User.IsInRole("Admin"))
        {
            var customerId = this.GetCurrentUserId();
            if (receipt.CustomerId != customerId)
            {
                return NotFound("Receipt not found");
            }
        }

        var pdfBytes = await _paymentReceiptService.GenerateReceiptPdfAsync(receiptNumber);
        return File(pdfBytes, "application/pdf", $"receipt-{receiptNumber}.pdf");
    }

    /// <summary>
    /// Resend receipt to customer
    /// </summary>
    [HttpPost("receipts/{receiptNumber}/resend")]
    public async Task<ActionResult> ResendReceipt(string receiptNumber, [FromBody] ResendReceiptRequest request)
    {
        var receipt = await _paymentReceiptService.GetReceiptByNumberAsync(receiptNumber);
        if (receipt == null)
        {
            return NotFound("Receipt not found");
        }

        // Verify receipt belongs to customer (unless admin)
        if (!User.IsInRole("Admin"))
        {
            var customerId = this.GetCurrentUserId();
            if (receipt.CustomerId != customerId)
            {
                return NotFound("Receipt not found");
            }
        }

        var success = await _paymentReceiptService.ResendReceiptAsync(receiptNumber, request.DeliveryMethod);
        if (!success)
        {
            return BadRequest("Failed to resend receipt");
        }

        return Ok(new { message = "Receipt resent successfully" });
    }

    /// <summary>
    /// Validate receipt authenticity
    /// </summary>
    [HttpPost("receipts/{receiptNumber}/validate")]
    public async Task<ActionResult<bool>> ValidateReceipt(string receiptNumber, [FromBody] ValidateReceiptRequest request)
    {
        var isValid = await _paymentReceiptService.ValidateReceiptAsync(receiptNumber, request.ConfirmationNumber);
        return Ok(new { isValid, message = isValid ? "Receipt is valid" : "Receipt is invalid" });
    }

    /// <summary>
    /// Get customer receipts with pagination
    /// </summary>
    [HttpPost("receipts")]
    public async Task<ActionResult<Bank.Domain.Common.PagedResult<PaymentReceiptDto>>> GetCustomerReceipts([FromBody] GetCustomerReceiptsRequest request)
    {
        var customerId = this.GetCurrentUserId();
        var receipts = await _paymentReceiptService.GetCustomerReceiptsAsync(
            customerId, 
            request.PageNumber, 
            request.PageSize,
            request.FromDate,
            request.ToDate);
        
        return Ok(receipts);
    }
}

// Request DTOs for the controller
public record ValidateBillerAccountRequest(string AccountNumber);
public record ResendReceiptRequest(string DeliveryMethod = "email");
public record ValidateReceiptRequest(string ConfirmationNumber);
public record GetCustomerReceiptsRequest(
    int PageNumber = 1,
    int PageSize = 20,
    DateTime? FromDate = null,
    DateTime? ToDate = null);