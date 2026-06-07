using Bank.Application.Interfaces;
using MediatR;
using TransactionEntity = Bank.Domain.Entities.Transaction;

namespace Bank.Application.Queries.Transaction;

public record GetTransactionsByAmountRangeQuery(
    Guid AccountId,
    decimal MinAmount,
    decimal MaxAmount) : IRequest<IEnumerable<TransactionEntity>>;

public sealed class GetTransactionsByAmountRangeQueryHandler
    : IRequestHandler<GetTransactionsByAmountRangeQuery, IEnumerable<TransactionEntity>>
{
    private readonly ITransactionService _transactionService;

    public GetTransactionsByAmountRangeQueryHandler(ITransactionService transactionService)
        => _transactionService = transactionService;

    public Task<IEnumerable<TransactionEntity>> Handle(
        GetTransactionsByAmountRangeQuery request,
        CancellationToken cancellationToken)
        => _transactionService.GetTransactionsByAmountRangeAsync(
            request.AccountId, request.MinAmount, request.MaxAmount);
}
