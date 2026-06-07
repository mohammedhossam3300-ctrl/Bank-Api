using Bank.Application.Interfaces;
using MediatR;
using AccountEntity = Bank.Domain.Entities.Account;

namespace Bank.Application.Commands.Account;

public record UpdateAccountCommand(
    Guid AccountId,
    string AccountHolderName,
    Guid RequestingUserId) : IRequest<AccountEntity>;

public sealed class UpdateAccountCommandHandler
    : IRequestHandler<UpdateAccountCommand, AccountEntity>
{
    private readonly IAccountService _accountService;

    public UpdateAccountCommandHandler(IAccountService accountService)
        => _accountService = accountService;

    public async Task<AccountEntity> Handle(
        UpdateAccountCommand request,
        CancellationToken cancellationToken)
    {
        var account = await _accountService.GetAccountByIdAsync(request.AccountId)
            ?? throw new KeyNotFoundException($"Account {request.AccountId} not found.");

        if (account.UserId != request.RequestingUserId)
            throw new UnauthorizedAccessException("You do not have permission to modify this account.");

        account.AccountHolderName = request.AccountHolderName;
        account.UpdatedAt = DateTime.UtcNow;
        account.UpdatedBy = request.RequestingUserId.ToString();

        await _accountService.UpdateAccountAsync(account);
        return account;
    }
}
