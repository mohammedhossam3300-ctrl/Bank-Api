using Bank.Application.Interfaces;
using MediatR;
using AccountEntity = Bank.Domain.Entities.Account;

namespace Bank.Application.Queries.Account;

public record GetUserAccountsQuery(Guid UserId) : IRequest<IEnumerable<AccountEntity>>;

public sealed class GetUserAccountsQueryHandler
    : IRequestHandler<GetUserAccountsQuery, IEnumerable<AccountEntity>>
{
    private readonly IAccountService _accountService;

    public GetUserAccountsQueryHandler(IAccountService accountService)
        => _accountService = accountService;

    public Task<IEnumerable<AccountEntity>> Handle(
        GetUserAccountsQuery request,
        CancellationToken cancellationToken)
        => _accountService.GetUserAccountsAsync(request.UserId);
}
