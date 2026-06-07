using Bank.Application.Interfaces;
using Bank.Domain.Enums;
using MediatR;
using TransactionEntity = Bank.Domain.Entities.Transaction;

namespace Bank.Application.Queries.Transaction;

public record GetTransactionsByStatusQuery(
    Guid AccountId,
    TransactionStatus Status) : IRequest<IEnumerable<TransactionEntity>>;

public sealed class GetTransactionsByStatusQueryHandler
    : IRequestHandler<GetTransactionsByStatusQuery, IEnumerable<TransactionEntity>>
{
    private readonly ITransactionService _transactionService;

    public GetTransactionsByStatusQueryHandler(ITransactionService transactionService)
        => _transactionService = transactionService;

    public Task<IEnumerable<TransactionEntity>> Handle(
        GetTransactionsByStatusQuery request,
        CancellationToken cancellationToken)
        => _transactionService.GetTransactionsByStatusAsync(request.AccountId, request.Status);
}
