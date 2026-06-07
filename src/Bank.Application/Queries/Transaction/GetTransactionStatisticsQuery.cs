using Bank.Application.DTOs.Transaction.Core;
using Bank.Application.Interfaces;
using MediatR;

namespace Bank.Application.Queries.Transaction;

public record GetTransactionStatisticsQuery(
    Guid AccountId,
    DateTime FromDate,
    DateTime ToDate) : IRequest<TransactionStatistics>;

public sealed class GetTransactionStatisticsQueryHandler
    : IRequestHandler<GetTransactionStatisticsQuery, TransactionStatistics>
{
    private readonly ITransactionService _transactionService;

    public GetTransactionStatisticsQueryHandler(ITransactionService transactionService)
        => _transactionService = transactionService;

    public Task<TransactionStatistics> Handle(
        GetTransactionStatisticsQuery request,
        CancellationToken cancellationToken)
        => _transactionService.GetTransactionStatisticsAsync(
            request.AccountId, request.FromDate, request.ToDate);
}
