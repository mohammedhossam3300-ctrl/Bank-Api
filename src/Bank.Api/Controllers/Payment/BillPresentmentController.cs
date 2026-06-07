using Bank.Api.Helpers;
using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers.Payment;

/// <summary>
/// Controller for bill presentment operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BillPresentmentController : ControllerBase
{
    private readonly IBillPresentmentService _billPresentmentService;
    private readonly ILogger<BillPresentmentController> _logger;

    public BillPresentmentController(
        IBillPresentmentService billPresentmentService,
        ILogger<BillPresentmentController> logger)
    {
        _billPresentmentService = billPresentmentService;
        _logger = logger;
    }

    /// <summary>
    /// Get bill presentments for the current customer
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<BillPresentmentDto>>> GetCustomerBillPresentments([FromQuery] Bank.Domain.Enums.BillPresentmentStatus? status = null)
    {
        try
        {
            var customerId = this.GetCurrentUserId();
            var presentments = await _billPresentmentService.GetCustomerBillPresentmentsAsync(customerId, status);
            return Ok(presentments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer bill presentments");
            return StatusCode(500, "An error occurred while retrieving bill presentments");
        }
    }

    /// <summary>
    /// Get bill presentments due within specified days
    /// </summary>
    [HttpGet("due-within/{days}")]
    public async Task<ActionResult<List<BillPresentmentDto>>> GetBillPresentmentsDueWithinDays(int days)
    {
        try
        {
            var customerId = this.GetCurrentUserId();
            var allPresentments = await _billPresentmentService.GetBillPresentmentsDueWithinDaysAsync(days);
            
            // Filter to only show presentments for the current customer (unless admin)
            var customerPresentments = User.IsInRole("Admin") 
                ? allPresentments 
                : allPresentments.Where(p => p.CustomerId == customerId).ToList();
            
            return Ok(customerPresentments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bill presentments due within {Days} days", days);
            return StatusCode(500, "An error occurred while retrieving bill presentments");
        }
    }

    /// <summary>
    /// Get overdue bill presentments (Admin only)
    /// </summary>
    [HttpGet("overdue")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<BillPresentmentDto>>> GetOverdueBillPresentments()
    {
        try
        {
            var presentments = await _billPresentmentService.GetOverdueBillPresentmentsAsync();
            return Ok(presentments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving overdue bill presentments");
            return StatusCode(500, "An error occurred while retrieving overdue bill presentments");
        }
    }

    /// <summary>
    /// Get bill presentment details by ID
    /// </summary>
    [HttpGet("{presentmentId}")]
    public async Task<ActionResult<BillPresentmentDto>> GetBillPresentmentById(Guid presentmentId)
    {
        try
        {
            var presentment = await _billPresentmentService.GetBillPresentmentByIdAsync(presentmentId);
            if (presentment == null)
            {
                return NotFound("Bill presentment not found");
            }

            // Verify presentment belongs to customer (unless admin)
            if (!User.IsInRole("Admin"))
            {
                var customerId = this.GetCurrentUserId();
                if (presentment.CustomerId != customerId)
                {
                    return NotFound("Bill presentment not found");
                }
            }

            return Ok(presentment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bill presentment {PresentmentId}", presentmentId);
            return StatusCode(500, "An error occurred while retrieving bill presentment");
        }
    }

    /// <summary>
    /// Create a new bill presentment (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BillPresentmentDto>> CreateBillPresentment([FromBody] CreateBillPresentmentRequest request)
    {
        try
        {
            var presentment = await _billPresentmentService.CreateBillPresentmentAsync(request);
            return CreatedAtAction(nameof(GetBillPresentmentById), new { presentmentId = presentment.Id }, presentment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating bill presentment");
            return StatusCode(500, "An error occurred while creating bill presentment");
        }
    }

    /// <summary>
    /// Update bill presentment status (Admin only)
    /// </summary>
    [HttpPut("{presentmentId}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateBillPresentmentStatus(Guid presentmentId, [FromBody] UpdateBillPresentmentStatusRequest request)
    {
        try
        {
            var success = await _billPresentmentService.UpdateBillPresentmentStatusAsync(presentmentId, request.Status);
            if (!success)
            {
                return NotFound("Bill presentment not found");
            }

            return Ok(new { message = "Bill presentment status updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating bill presentment status for {PresentmentId}", presentmentId);
            return StatusCode(500, "An error occurred while updating bill presentment status");
        }
    }

    /// <summary>
    /// Mark bill presentment as paid
    /// </summary>
    [HttpPost("{presentmentId}/mark-paid")]
    public async Task<ActionResult> MarkBillPresentmentAsPaid(Guid presentmentId, [FromBody] MarkBillPresentmentAsPaidRequest request)
    {
        try
        {
            var presentment = await _billPresentmentService.GetBillPresentmentByIdAsync(presentmentId);
            if (presentment == null)
            {
                return NotFound("Bill presentment not found");
            }

            // Verify presentment belongs to customer (unless admin)
            if (!User.IsInRole("Admin"))
            {
                var customerId = this.GetCurrentUserId();
                if (presentment.CustomerId != customerId)
                {
                    return NotFound("Bill presentment not found");
                }
            }

            var success = await _billPresentmentService.MarkBillPresentmentAsPaidAsync(presentmentId, request.PaymentId);
            if (!success)
            {
                return BadRequest("Bill presentment cannot be marked as paid");
            }

            return Ok(new { message = "Bill presentment marked as paid successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill presentment {PresentmentId} as paid", presentmentId);
            return StatusCode(500, "An error occurred while marking bill presentment as paid");
        }
    }

    /// <summary>
    /// Cancel bill presentment
    /// </summary>
    [HttpPost("{presentmentId}/cancel")]
    public async Task<ActionResult> CancelBillPresentment(Guid presentmentId)
    {
        try
        {
            var presentment = await _billPresentmentService.GetBillPresentmentByIdAsync(presentmentId);
            if (presentment == null)
            {
                return NotFound("Bill presentment not found");
            }

            // Verify presentment belongs to customer (unless admin)
            if (!User.IsInRole("Admin"))
            {
                var customerId = this.GetCurrentUserId();
                if (presentment.CustomerId != customerId)
                {
                    return NotFound("Bill presentment not found");
                }
            }

            var success = await _billPresentmentService.CancelBillPresentmentAsync(presentmentId);
            if (!success)
            {
                return BadRequest("Bill presentment cannot be cancelled");
            }

            return Ok(new { message = "Bill presentment cancelled successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling bill presentment {PresentmentId}", presentmentId);
            return StatusCode(500, "An error occurred while cancelling bill presentment");
        }
    }

    /// <summary>
    /// Process overdue bill presentments (Admin only)
    /// </summary>
    [HttpPost("process-overdue")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ProcessOverdueBillPresentments()
    {
        try
        {
            var processedCount = await _billPresentmentService.ProcessOverdueBillPresentmentsAsync();
            return Ok(new { message = $"Processed {processedCount} overdue bill presentments", processedCount });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing overdue bill presentments");
            return StatusCode(500, "An error occurred while processing overdue bill presentments");
        }
    }

    /// <summary>
    /// Synchronize bill presentments with external biller systems
    /// </summary>
    [HttpPost("synchronize")]
    public async Task<ActionResult<List<BillPresentmentSyncResult>>> SynchronizeBillPresentments([FromBody] SynchronizeBillPresentmentsRequest request)
    {
        try
        {
            var customerId = this.GetCurrentUserId();
            
            // Admin can synchronize for any customer, regular users only for themselves
            var targetCustomerId = User.IsInRole("Admin") && request.CustomerId.HasValue 
                ? request.CustomerId.Value 
                : customerId;

            var results = await _billPresentmentService.SynchronizeBillPresentmentsAsync(targetCustomerId, request.BillerId);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error synchronizing bill presentments");
            return StatusCode(500, "An error occurred while synchronizing bill presentments");
        }
    }

    /// <summary>
    /// Get bill presentments by biller (Admin only)
    /// </summary>
    [HttpGet("biller/{billerId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<BillPresentmentDto>>> GetBillPresentmentsByBiller(Guid billerId, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var presentments = await _billPresentmentService.GetBillPresentmentsByBillerAsync(billerId, fromDate, toDate);
            return Ok(presentments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bill presentments for biller {BillerId}", billerId);
            return StatusCode(500, "An error occurred while retrieving bill presentments");
        }
    }
}

// Request DTOs for the controller
public record UpdateBillPresentmentStatusRequest([property: System.Text.Json.Serialization.JsonRequired] Bank.Domain.Enums.BillPresentmentStatus Status);
public record MarkBillPresentmentAsPaidRequest([property: System.Text.Json.Serialization.JsonRequired] Guid PaymentId);
public record SynchronizeBillPresentmentsRequest([property: System.Text.Json.Serialization.JsonRequired] Guid BillerId, Guid? CustomerId = null);