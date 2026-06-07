using Bank.Application.Interfaces;
using MediatR;
using AccountEntity = Bank.Domain.Entities.Account;

namespace Bank.Application.Commands.Account;

public record CreateAccountCommand(
    Guid UserId,
    string AccountNickname) : IRequest<AccountEntity>;

public sealed class CreateAccountCommandHandler
    : IRequestHandler<CreateAccountCommand, AccountEntity>
{
    private readonly IAccountService _accountService;

    public CreateAccountCommandHandler(IAccountService accountService)
        => _accountService = accountService;

    public Task<AccountEntity> Handle(
        CreateAccountCommand request,
        CancellationToken cancellationToken)
        => _accountService.CreateAccountAsync(request.UserId, request.AccountNickname);
}
