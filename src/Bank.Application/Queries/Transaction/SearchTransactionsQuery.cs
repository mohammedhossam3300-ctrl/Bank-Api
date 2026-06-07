using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using MediatR;
using TransactionEntity = Bank.Domain.Entities.Transaction;

namespace Bank.Application.Queries.Transaction;

/// <summary>Paged transaction search. Returns items + total count in one round-trip.</summary>
public record SearchTransactionsQuery(
    TransactionSearchCriteria Criteria,
    int PageNumber = 1,
    int PageSize = 50) : IRequest<SearchTransactionsResult>;

public record SearchTransactionsResult(
    IEnumerable<TransactionEntity> Transactions,
    int TotalCount,
    int PageNumber,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public sealed class SearchTransactionsQueryHandler
    : IRequestHandler<SearchTransactionsQuery, SearchTransactionsResult>
{
    private readonly ITransactionService _transactionService;

    public SearchTransactionsQueryHandler(ITransactionService transactionService)
        => _transactionService = transactionService;

    public async Task<SearchTransactionsResult> Handle(
        SearchTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        var (transactions, totalCount) = await _transactionService.SearchTransactionsAsync(
            request.Criteria, request.PageNumber, request.PageSize);

        return new SearchTransactionsResult(transactions, totalCount, request.PageNumber, request.PageSize);
    }
}
