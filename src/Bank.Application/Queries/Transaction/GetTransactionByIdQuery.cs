using Bank.Application.Interfaces;
using MediatR;
using TransactionEntity = Bank.Domain.Entities.Transaction;

namespace Bank.Application.Queries.Transaction;

public record GetTransactionByIdQuery(Guid TransactionId) : IRequest<TransactionEntity?>;

public sealed class GetTransactionByIdQueryHandler
    : IRequestHandler<GetTransactionByIdQuery, TransactionEntity?>
{
    private readonly ITransactionService _transactionService;

    public GetTransactionByIdQueryHandler(ITransactionService transactionService)
        => _transactionService = transactionService;

    public Task<TransactionEntity?> Handle(
        GetTransactionByIdQuery request,
        CancellationToken cancellationToken)
        => _transactionService.GetTransactionByIdAsync(request.TransactionId);
}
