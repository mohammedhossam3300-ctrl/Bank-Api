using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using MediatR;

namespace Bank.Application.Commands.Transaction;

public record ExportTransactionsCsvCommand(TransactionSearchCriteria Criteria) : IRequest<byte[]>;

public sealed class ExportTransactionsCsvCommandHandler
    : IRequestHandler<ExportTransactionsCsvCommand, byte[]>
{
    private readonly ITransactionService _transactionService;

    public ExportTransactionsCsvCommandHandler(ITransactionService transactionService)
        => _transactionService = transactionService;

    public Task<byte[]> Handle(ExportTransactionsCsvCommand request, CancellationToken cancellationToken)
        => _transactionService.ExportTransactionsToCsvAsync(request.Criteria);
}

public record ExportTransactionsExcelCommand(TransactionSearchCriteria Criteria) : IRequest<byte[]>;

public sealed class ExportTransactionsExcelCommandHandler
    : IRequestHandler<ExportTransactionsExcelCommand, byte[]>
{
    private readonly ITransactionService _transactionService;

    public ExportTransactionsExcelCommandHandler(ITransactionService transactionService)
        => _transactionService = transactionService;

    public Task<byte[]> Handle(ExportTransactionsExcelCommand request, CancellationToken cancellationToken)
        => _transactionService.ExportTransactionsToExcelAsync(request.Criteria);
}
