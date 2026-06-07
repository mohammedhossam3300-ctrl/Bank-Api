---
name: CQRS MediatR structure
description: Patterns and pitfalls for the FinBank Application layer MediatR/CQRS setup.
---

## Namespace collision — always use type aliases

Query/Command namespaces collide with entity names. `Bank.Application.Queries.Transaction` and `Bank.Domain.Entities.Transaction` clash, causing CS0118. Same for `Account`.

**Fix:** Add a using alias at the top of every affected file:
```csharp
using TransactionEntity = Bank.Domain.Entities.Transaction;
using AccountEntity = Bank.Domain.Entities.Account;
```

**Why:** The project uses folder-matching namespaces (`Queries/Transaction/` → `Bank.Application.Queries.Transaction`), which collides with entity class names.

## ValidationBehavior pipeline

Registered in `CqrsServiceExtensions` as:
```csharp
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
```
All `AbstractValidator<TCommand>` implementations are automatically picked up. No additional wiring needed per-command. Throws `FluentValidation.ValidationException` (→ HTTP 400 with field-level errors via `GlobalExceptionMiddleware`).

## N+1 batch loop pattern

When a public method does `GetByIdAsync(id)` and is called in a batch loop, extract a private `*CoreAsync(Entity entity, ...)` overload that skips the fetch. The public method fetches then delegates; the batch loop calls the core method directly.

Applied to:
- `InterestCalculationService.ApplyInterestAsync` → `ApplyInterestCoreAsync(Account, Guid)`
- `AccountLifecycleService.ApplyAccountFeesAsync` → `ApplyAccountFeesCoreAsync(Account, Guid)`

**Why:** Batch jobs fetch a list of entities then re-fetched each by ID inside the loop = N+1. The core-method pattern eliminates it without breaking the public API.

## Folder layout (Application layer)

```
Commands/
  Behaviors/ValidationBehavior.cs
  Account/CreateAccountCommand.cs, UpdateAccountCommand.cs, DeleteAccountCommand.cs
  Transaction/InitiateTransactionCommand.cs, ExportTransactionsCommand.cs
Queries/
  Account/GetUserAccountsQuery.cs, GetAccountByIdQuery.cs
  Transaction/GetTransactionHistoryQuery.cs, GetTransactionByIdQuery.cs,
              SearchTransactionsQuery.cs, GetTransactionsByDateRangeQuery.cs,
              GetTransactionsByTypeQuery.cs, GetTransactionsByAmountRangeQuery.cs,
              GetTransactionsByStatusQuery.cs, GetTransactionStatisticsQuery.cs
EventHandlers/ (audit + security domain events — not commands/queries)
```

## Controller pattern

Controllers inject only `IMediator` — no service interfaces directly. `KeyNotFoundException` from handlers → 404, `UnauthorizedAccessException` → 401, `ValidationException` → 400.

## Duplicate search criteria mapping

`TransactionSearchRequest` → `TransactionSearchCriteria` mapping lives in a single private static `MapCriteria(TransactionSearchRequest)` helper in `TransactionController`. Do not copy-paste it per endpoint.
