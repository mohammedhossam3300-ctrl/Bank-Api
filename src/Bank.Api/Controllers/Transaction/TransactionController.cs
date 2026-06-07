using Bank.Application.DTOs;
using Bank.Application.Commands.Transaction;
using Bank.Application.Queries.Transaction;
using Bank.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers.Transaction;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> InitiateTransaction([FromBody] InitiateTransactionCommand request)
    {
        var transaction = await _mediator.Send(request);
        return Ok(transaction);
    }

    [HttpGet("history/{accountId}")]
    public async Task<IActionResult> GetHistory(Guid accountId)
    {
        var history = await _mediator.Send(new GetTransactionHistoryQuery(accountId));
        return Ok(history);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransaction(Guid id)
    {
        var transaction = await _mediator.Send(new GetTransactionByIdQuery(id));
        return transaction is null ? NotFound() : Ok(transaction);
    }

    [HttpPost("search")]
    public async Task<IActionResult> SearchTransactions([FromBody] TransactionSearchRequest request)
    {
        var result = await _mediator.Send(
            new SearchTransactionsQuery(MapCriteria(request), request.PageNumber, request.PageSize));

        return Ok(result);
    }

    [HttpGet("by-date-range/{accountId}")]
    public async Task<IActionResult> GetTransactionsByDateRange(
        Guid accountId,
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        var transactions = await _mediator.Send(
            new GetTransactionsByDateRangeQuery(accountId, fromDate, toDate));
        return Ok(transactions);
    }

    [HttpGet("by-type/{accountId}")]
    public async Task<IActionResult> GetTransactionsByType(
        Guid accountId,
        [FromQuery] TransactionType type)
    {
        var transactions = await _mediator.Send(new GetTransactionsByTypeQuery(accountId, type));
        return Ok(transactions);
    }

    [HttpGet("by-amount-range/{accountId}")]
    public async Task<IActionResult> GetTransactionsByAmountRange(
        Guid accountId,
        [FromQuery] decimal minAmount,
        [FromQuery] decimal maxAmount)
    {
        var transactions = await _mediator.Send(
            new GetTransactionsByAmountRangeQuery(accountId, minAmount, maxAmount));
        return Ok(transactions);
    }

    [HttpGet("by-status/{accountId}")]
    public async Task<IActionResult> GetTransactionsByStatus(
        Guid accountId,
        [FromQuery] TransactionStatus status)
    {
        var transactions = await _mediator.Send(new GetTransactionsByStatusQuery(accountId, status));
        return Ok(transactions);
    }

    [HttpGet("statistics/{accountId}")]
    public async Task<IActionResult> GetStatistics(
        Guid accountId,
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        var statistics = await _mediator.Send(
            new GetTransactionStatisticsQuery(accountId, fromDate, toDate));
        return Ok(statistics);
    }

    // Export endpoints delegate to the service through the mediator pipeline but return
    // file content directly — kept with mediator-dispatched commands for consistency.
    [HttpPost("export/csv")]
    public async Task<IActionResult> ExportToCsv([FromBody] TransactionSearchRequest request)
    {
        var csvData = await _mediator.Send(new ExportTransactionsCsvCommand(MapCriteria(request)));
        return File(csvData, "text/csv", $"transactions_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");
    }

    [HttpPost("export/excel")]
    public async Task<IActionResult> ExportToExcel([FromBody] TransactionSearchRequest request)
    {
        var excelData = await _mediator.Send(new ExportTransactionsExcelCommand(MapCriteria(request)));
        return File(excelData,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"transactions_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx");
    }

    // Single shared mapping — eliminates the previous 3× copy-paste of this logic.
    private static TransactionSearchCriteria MapCriteria(TransactionSearchRequest r) => new()
    {
        AccountId    = r.AccountId,
        FromDate     = r.FromDate,
        ToDate       = r.ToDate,
        Type         = r.Type,
        Status       = r.Status,
        MinAmount    = r.MinAmount,
        MaxAmount    = r.MaxAmount,
        Description  = r.Description,
        Reference    = r.Reference,
        FromAccountId = r.FromAccountId,
        ToAccountId  = r.ToAccountId
    };
}
