using Bank.Application.Interfaces;
using Bank.Domain.Enums;
using MediatR;
using TransactionEntity = Bank.Domain.Entities.Transaction;

namespace Bank.Application.Queries.Transaction;

public record GetTransactionsByTypeQuery(
    Guid AccountId,
    TransactionType Type) : IRequest<IEnumerable<TransactionEntity>>;

public sealed class GetTransactionsByTypeQueryHandler
    : IRequestHandler<GetTransactionsByTypeQuery, IEnumerable<TransactionEntity>>
{
    private readonly ITransactionService _transactionService;

    public GetTransactionsByTypeQueryHandler(ITransactionService transactionService)
        => _transactionService = transactionService;

    public Task<IEnumerable<TransactionEntity>> Handle(
        GetTransactionsByTypeQuery request,
        CancellationToken cancellationToken)
        => _transactionService.GetTransactionsByTypeAsync(request.AccountId, request.Type);
}
