using Bank.Application.Interfaces;
using MediatR;
using TransactionEntity = Bank.Domain.Entities.Transaction;

namespace Bank.Application.Queries.Transaction;

public record GetTransactionsByDateRangeQuery(
    Guid AccountId,
    DateTime FromDate,
    DateTime ToDate) : IRequest<IEnumerable<TransactionEntity>>;

public sealed class GetTransactionsByDateRangeQueryHandler
    : IRequestHandler<GetTransactionsByDateRangeQuery, IEnumerable<TransactionEntity>>
{
    private readonly ITransactionService _transactionService;

    public GetTransactionsByDateRangeQueryHandler(ITransactionService transactionService)
        => _transactionService = transactionService;

    public Task<IEnumerable<TransactionEntity>> Handle(
        GetTransactionsByDateRangeQuery request,
        CancellationToken cancellationToken)
        => _transactionService.GetTransactionsByDateRangeAsync(
            request.AccountId, request.FromDate, request.ToDate);
}
