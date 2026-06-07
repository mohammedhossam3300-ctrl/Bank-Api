using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers.Payment;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RecurringPaymentController : ControllerBase
{
    private readonly IRecurringPaymentService _recurringPaymentService;

    public RecurringPaymentController(
        IRecurringPaymentService recurringPaymentService)
    {
        _recurringPaymentService = recurringPaymentService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRecurringPayment([FromBody] CreateRecurringPaymentRequest request)
    {
        var recurringPayment = await _recurringPaymentService.CreateRecurringPaymentAsync(request);
        return CreatedAtAction(nameof(GetRecurringPayment), new { id = recurringPayment.Id }, recurringPayment);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRecurringPayment(Guid id)
    {
        var recurringPayment = await _recurringPaymentService.GetRecurringPaymentAsync(id);
        if (recurringPayment == null)
            return NotFound();

        return Ok(recurringPayment);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserRecurringPayments(Guid userId)
    {
        var recurringPayments = await _recurringPaymentService.GetUserRecurringPaymentsAsync(userId);
        return Ok(recurringPayments);
    }

    [HttpGet("account/{accountId}")]
    public async Task<IActionResult> GetAccountRecurringPayments(Guid accountId)
    {
        var recurringPayments = await _recurringPaymentService.GetAccountRecurringPaymentsAsync(accountId);
        return Ok(recurringPayments);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRecurringPayment(Guid id, [FromBody] UpdateRecurringPaymentRequest request)
    {
        var success = await _recurringPaymentService.UpdateRecurringPaymentAsync(id, request);
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id}/pause")]
    public async Task<IActionResult> PauseRecurringPayment(Guid id, [FromBody] string reason)
    {
        var success = await _recurringPaymentService.PauseRecurringPaymentAsync(id, reason);
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id}/resume")]
    public async Task<IActionResult> ResumeRecurringPayment(Guid id)
    {
        var success = await _recurringPaymentService.ResumeRecurringPaymentAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelRecurringPayment(Guid id, [FromBody] string reason)
    {
        var success = await _recurringPaymentService.CancelRecurringPaymentAsync(id, reason);
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpGet("{id}/executions")]
    public async Task<IActionResult> GetExecutionHistory(Guid id)
    {
        var executions = await _recurringPaymentService.GetExecutionHistoryAsync(id);
        return Ok(executions);
    }

    [HttpPost("bulk-transfer")]
    public async Task<IActionResult> ProcessBulkTransfers([FromBody] BulkTransferRequest request)
    {
        var result = await _recurringPaymentService.ProcessBulkTransfersAsync(request);
        return Ok(result);
    }

    [HttpGet("bulk-transfer/{batchId}/status")]
    public async Task<IActionResult> GetBulkTransferStatus(Guid batchId)
    {
        var result = await _recurringPaymentService.GetBulkTransferStatusAsync(batchId);
        return Ok(result);
    }

    [HttpGet("due")]
    [Authorize(Roles = "Admin,System")]
    public async Task<IActionResult> GetDueRecurringPayments()
    {
        var duePayments = await _recurringPaymentService.GetDueRecurringPaymentsAsync();
        return Ok(duePayments);
    }

    [HttpPost("process-due")]
    [Authorize(Roles = "Admin,System")]
    public async Task<IActionResult> ProcessDueRecurringPayments()
    {
        var processedCount = await _recurringPaymentService.ProcessDueRecurringPaymentsAsync();
        return Ok(new { ProcessedCount = processedCount });
    }
}