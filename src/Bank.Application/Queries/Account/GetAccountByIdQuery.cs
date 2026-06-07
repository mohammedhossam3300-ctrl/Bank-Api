using Bank.Application.Interfaces;
using MediatR;
using AccountEntity = Bank.Domain.Entities.Account;

namespace Bank.Application.Queries.Account;

public record GetAccountByIdQuery(Guid AccountId) : IRequest<AccountEntity?>;

public sealed class GetAccountByIdQueryHandler
    : IRequestHandler<GetAccountByIdQuery, AccountEntity?>
{
    private readonly IAccountService _accountService;

    public GetAccountByIdQueryHandler(IAccountService accountService)
        => _accountService = accountService;

    public Task<AccountEntity?> Handle(
        GetAccountByIdQuery request,
        CancellationToken cancellationToken)
        => _accountService.GetAccountByIdAsync(request.AccountId);
}
