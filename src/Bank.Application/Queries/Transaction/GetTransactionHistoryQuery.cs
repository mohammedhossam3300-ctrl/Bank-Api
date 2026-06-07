using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using MediatR;
using TransactionEntity = Bank.Domain.Entities.Transaction;

namespace Bank.Application.Queries.Transaction;

public record GetTransactionHistoryQuery(Guid AccountId) : IRequest<IEnumerable<TransactionEntity>>;

public sealed class GetTransactionHistoryQueryHandler
    : IRequestHandler<GetTransactionHistoryQuery, IEnumerable<TransactionEntity>>
{
    private readonly ITransactionService _transactionService;

    public GetTransactionHistoryQueryHandler(ITransactionService transactionService)
        => _transactionService = transactionService;

    public Task<IEnumerable<TransactionEntity>> Handle(
        GetTransactionHistoryQuery request,
        CancellationToken cancellationToken)
        => _transactionService.GetTransactionHistoryAsync(request.AccountId);
}
