using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Bank.Application.DTOs;
using Bank.Application.Services;
using Bank.Domain.Entities;
using Bank.Domain.Enums;
using Bank.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Bank.Tests
{
    public class TransactionServiceTests
    {
        private class QueryableAsyncProvider<T> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;
            public QueryableAsyncProvider(IQueryProvider inner) { _inner = inner; }
            public IQueryable CreateQuery(Expression expression) => new EnumerableQuery<T>(expression as Expression ?? throw new InvalidOperationException());
            public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new EnumerableQuery<TElement>(expression);
            public object Execute(Expression expression) => _inner.Execute(expression)!;
            public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);
            public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression) => new AsyncEnumerable<TResult>(Execute<IEnumerable<TResult>>(expression));
            public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) => Task.FromResult(Execute<TResult>(expression));
        }

        private class AsyncEnumerable<T> : IAsyncEnumerable<T>, IQueryable<T>
        {
            private readonly IEnumerable<T> _enumerable;
            public AsyncEnumerable(IEnumerable<T> enumerable) { _enumerable = enumerable; }
            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new AsyncEnumerator<T>(_enumerable.GetEnumerator());
            public Type ElementType => typeof(T);
            public Expression Expression => (_enumerable as IQueryable)?.Expression ?? Expression.Constant(this);
            public IQueryProvider Provider => new QueryableAsyncProvider<T>((_enumerable as IQueryable)?.Provider ?? new EnumerableQuery<T>(_enumerable));
            public IEnumerator<T> GetEnumerator() => _enumerable.GetEnumerator();
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _enumerable.GetEnumerator();
        }

        private class AsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;
            public AsyncEnumerator(IEnumerator<T> inner) { _inner = inner; }
            public T Current => _inner.Current;
            public ValueTask DisposeAsync() { _inner.Dispose(); return ValueTask.CompletedTask; }
            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
        }

        private static Mock<IRepository<T>> MockRepository<T>(IQueryable<T> data) where T : Bank.Domain.Common.BaseEntity
        {
            var mock = new Mock<IRepository<T>>();
            mock.Setup(r => r.Query()).Returns(data);
            return mock;
        }

        private static Account CreateAccount(Guid id, decimal balance)
        {
            return new Account
            {
                Id = id,
                AccountNumber = $"ACC-{id.ToString().Substring(0, 8)}",
                AccountHolderName = "Test User",
                Balance = balance,
                UserId = Guid.NewGuid()
            };
        }

        [Fact]
        public async Task InitiateTransaction_Should_Throw_On_Insufficient_Funds()
        {
            // Arrange
            var fromId = Guid.NewGuid();
            var toId = Guid.NewGuid();
            var from = CreateAccount(fromId, 50m);
            var to = CreateAccount(toId, 0m);

            var fromRepo = new Mock<IRepository<Account>>();
            fromRepo.Setup(r => r.GetByIdAsync(fromId)).ReturnsAsync(from);
            fromRepo.Setup(r => r.Update(It.IsAny<Account>()));

            var toRepo = new Mock<IRepository<Account>>();
            toRepo.Setup(r => r.GetByIdAsync(toId)).ReturnsAsync(to);
            toRepo.Setup(r => r.Update(It.IsAny<Account>()));

            var trxRepo = new Mock<IRepository<Transaction>>();
            trxRepo.Setup(r => r.AddAsync(It.IsAny<Transaction>())).Returns(Task.CompletedTask);

            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Repository<Account>()).Returns(fromRepo.Object);
            // We need to return the same mock for both account fetches. Simplest: sequence by id with the same repo mock.
            // The TransactionService calls Repository<Account>() twice; returning fromRepo both times is fine.
            uow.Setup(u => u.Repository<Transaction>()).Returns(trxRepo.Object);
            uow.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uow.Setup(u => u.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var service = new TransactionService(uow.Object);

            // Act + Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => service.InitiateTransactionAsync(fromId, toId, 100m, TransactionType.RTGS, "desc"));
            Assert.Contains("Insufficient funds", ex.Message);
            uow.Verify(u => u.RollbackTransactionAsync(), Times.Once);
            trxRepo.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Never);
        }

        [Fact]
        public async Task InitiateTransaction_RTGS_Should_Transfer_And_Complete()
        {
            // Arrange
            var fromId = Guid.NewGuid();
            var toId = Guid.NewGuid();
            var from = CreateAccount(fromId, 200m);
            var to = CreateAccount(toId, 20m);

            var accountRepo = new Mock<IRepository<Account>>();
            accountRepo.Setup(r => r.GetByIdAsync(fromId)).ReturnsAsync(from);
            accountRepo.Setup(r => r.GetByIdAsync(toId)).ReturnsAsync(to);
            accountRepo.Setup(r => r.Update(It.IsAny<Account>()));

            var trxRepo = new Mock<IRepository<Transaction>>();
            Transaction? saved = null;
            trxRepo.Setup(r => r.AddAsync(It.IsAny<Transaction>())).Callback<Transaction>(t => saved = t).Returns(Task.CompletedTask);

            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Repository<Account>()).Returns(accountRepo.Object);
            uow.Setup(u => u.Repository<Transaction>()).Returns(trxRepo.Object);
            uow.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uow.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var service = new TransactionService(uow.Object);

            // Act
            var result = await service.InitiateTransactionAsync(fromId, toId, 100m, TransactionType.RTGS, "payment");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TransactionStatus.Completed, result.Status);
            Assert.Equal(100m, result.Amount);
            Assert.Equal(fromId, result.FromAccountId);
            Assert.Equal(toId, result.ToAccountId);
            Assert.True(result.ProcessedAt.HasValue);
            Assert.Equal(100m, 200m - from.Balance);
            Assert.Equal(100m, to.Balance - 20m);
            uow.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task InitiateTransaction_NonRTGS_Should_Pend_And_Debit_From_Only()
        {
            // Arrange
            var fromId = Guid.NewGuid();
            var toId = Guid.NewGuid();
            var from = CreateAccount(fromId, 300m);
            var to = CreateAccount(toId, 50m);

            var accountRepo = new Mock<IRepository<Account>>();
            accountRepo.Setup(r => r.GetByIdAsync(fromId)).ReturnsAsync(from);
            accountRepo.Setup(r => r.GetByIdAsync(toId)).ReturnsAsync(to);
            accountRepo.Setup(r => r.Update(It.IsAny<Account>()));

            var trxRepo = new Mock<IRepository<Transaction>>();
            Transaction? saved = null;
            trxRepo.Setup(r => r.AddAsync(It.IsAny<Transaction>())).Callback<Transaction>(t => saved = t).Returns(Task.CompletedTask);

            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Repository<Account>()).Returns(accountRepo.Object);
            uow.Setup(u => u.Repository<Transaction>()).Returns(trxRepo.Object);
            uow.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            uow.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var service = new TransactionService(uow.Object);

            // Act
            var result = await service.InitiateTransactionAsync(fromId, toId, 120m, TransactionType.ACH, "ach payment");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TransactionStatus.Pending, result.Status);
            Assert.Null(result.ProcessedAt);
            Assert.Equal(180m, from.Balance); // debited
            Assert.Equal(50m, to.Balance);   // unchanged
            uow.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task GetTransactionHistory_Should_Return_Ordered_By_CreatedAt_Desc()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var otherId = Guid.NewGuid();

            var tx1 = new Transaction { Id = Guid.NewGuid(), CreatedAt = new DateTime(2024, 1, 1), FromAccountId = accountId, ToAccountId = otherId, Amount = 10, Type = TransactionType.ACH, Status = TransactionStatus.Completed };
            var tx2 = new Transaction { Id = Guid.NewGuid(), CreatedAt = new DateTime(2024, 2, 1), FromAccountId = otherId, ToAccountId = accountId, Amount = 20, Type = TransactionType.RTGS, Status = TransactionStatus.Completed };
            var tx3 = new Transaction { Id = Guid.NewGuid(), CreatedAt = new DateTime(2023, 12, 1), FromAccountId = otherId, ToAccountId = otherId, Amount = 30, Type = TransactionType.WPS, Status = TransactionStatus.Pending };

            var data = new List<Transaction> { tx1, tx2, tx3 }.AsQueryable();
            var trxRepo = new Mock<IRepository<Transaction>>();
            // For EF-like async ToListAsync(), return an IQueryable backed by our list
            trxRepo.Setup(r => r.Query()).Returns(data);

            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Repository<Transaction>()).Returns(trxRepo.Object);

            var service = new TransactionService(uow.Object);

            // Act
            var list = await service.GetTransactionHistoryAsync(accountId);
            var arr = list.ToArray();

            // Assert
            Assert.Equal(2, arr.Length);
            Assert.Equal(tx2.Id, arr[0].Id); // Feb first
            Assert.Equal(tx1.Id, arr[1].Id); // Jan next
        }

        [Fact]
        public async Task ExportTransactionsToCsv_Should_Render_Correct_Headers_And_Rows()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var criteria = new TransactionSearchCriteria { AccountId = accountId };

            var fromAcc = CreateAccount(Guid.NewGuid(), 0);
            fromAcc.AccountNumber = "FROM-123";
            var toAcc = CreateAccount(Guid.NewGuid(), 0);
            toAcc.AccountNumber = "TO-456";

            var tx = new Transaction
            {
                Id = Guid.NewGuid(),
                CreatedAt = new DateTime(2024, 05, 10, 14, 30, 0, DateTimeKind.Utc),
                Reference = "REF001",
                Type = TransactionType.RTGS,
                Status = TransactionStatus.Completed,
                FromAccountId = fromAcc.Id,
                ToAccountId = toAcc.Id,
                FromAccount = fromAcc,
                ToAccount = toAcc,
                Amount = 123.45m,
                Description = "Test export"
            };

            var data = new List<Transaction> { tx }.AsQueryable();
            var trxRepo = new Mock<IRepository<Transaction>>();
            trxRepo.Setup(r => r.Query()).Returns(data);

            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.Repository<Transaction>()).Returns(trxRepo.Object);

            var service = new TransactionService(uow.Object);

            // Act
            var bytes = await service.ExportTransactionsToCsvAsync(criteria);
            var csv = System.Text.Encoding.UTF8.GetString(bytes);

            // Assert
            var lines = csv.Trim().Split('\n');
            Assert.StartsWith("Date,Reference,Type,Status,From Account,To Account,Amount,Description", lines[0]);
            Assert.Contains("2024-05-10 14:30:00,REF001,RTGS,Completed,FROM-123,TO-456,123.45,\"Test export\"", csv);
        }
    }
}
