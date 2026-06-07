using Bank.Application.Interfaces;
using MediatR;

namespace Bank.Application.Commands.Account;

public record DeleteAccountCommand(
    Guid AccountId,
    Guid RequestingUserId) : IRequest;

public sealed class DeleteAccountCommandHandler
    : IRequestHandler<DeleteAccountCommand>
{
    private readonly IAccountService _accountService;

    public DeleteAccountCommandHandler(IAccountService accountService)
        => _accountService = accountService;

    public async Task Handle(
        DeleteAccountCommand request,
        CancellationToken cancellationToken)
    {
        var account = await _accountService.GetAccountByIdAsync(request.AccountId)
            ?? throw new KeyNotFoundException($"Account {request.AccountId} not found.");

        if (account.UserId != request.RequestingUserId)
            throw new UnauthorizedAccessException("You do not have permission to delete this account.");

        account.SoftDelete(request.RequestingUserId.ToString());
        await _accountService.UpdateAccountAsync(account);
    }
}
