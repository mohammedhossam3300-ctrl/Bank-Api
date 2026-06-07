using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bank.Application.DTOs.Payment.Biller;
using Bank.Application.Services;
using Bank.Domain.Entities;
using Bank.Domain.Enums;
using Bank.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Bank.Tests
{
    public class BillPaymentProcessingServiceTests
    {
        private static BillPayment NewBillPayment(
            Guid id,
            Guid customerId,
            Guid billerId,
            decimal amount,
            BillPaymentStatus status = BillPaymentStatus.Pending)
        {
            return new BillPayment
            {
                Id = id,
                CustomerId = customerId,
                BillerId = billerId,
                Amount = amount,
                Currency = "USD",
                ScheduledDate = DateTime.UtcNow.AddDays(1),
                Status = status,
                Reference = "REF-123",
                Description = "Test payment",
                CreatedAt = DateTime.UtcNow
            };
        }

        private static Biller NewBiller(Guid id, string name = "Test Biller", bool isActive = true)
        {
            return new Biller
            {
                Id = id,
                Name = name,
                Category = BillerCategory.Utilities,
                AccountNumber = "ACC-123456",
                RoutingNumber = "ROUTE-789",
                Address = "123 Main St",
                IsActive = isActive,
                SupportedPaymentMethods = "[\"ACH\", \"Wire\"]",
                MinAmount = 10m,
                MaxAmount = 10000m,
                ProcessingDays = 1,
                CreatedAt = DateTime.UtcNow
            };
        }

        private static Account NewAccount(Guid id, Guid customerId, decimal balance = 5000m)
        {
            return new Account
            {
                Id = id,
                CustomerId = customerId,
                Type = AccountType.Checking,
                Status = AccountStatus.Active,
                Balance = balance,
                CreatedAt = DateTime.UtcNow
            };
        }

        private BillPaymentProcessingService CreateService(
            IBillPaymentRepository billPaymentRepository = null,
            IBillerRepository billerRepository = null,
            IAccountService accountService = null,
            ITransactionService transactionService = null,
            IBillerIntegrationService billerIntegrationService = null,
            IPaymentRetryService paymentRetryService = null,
            IPaymentReceiptService paymentReceiptService = null,
            IUnitOfWork unitOfWork = null,
            ILogger<BillPaymentProcessingService> logger = null)
        {
            return new BillPaymentProcessingService(
                billPaymentRepository ?? new Mock<IBillPaymentRepository>().Object,
                billerRepository ?? new Mock<IBillerRepository>().Object,
                accountService ?? new Mock<IAccountService>().Object,
                transactionService ?? new Mock<ITransactionService>().Object,
                billerIntegrationService ?? new Mock<IBillerIntegrationService>().Object,
                paymentRetryService ?? new Mock<IPaymentRetryService>().Object,
                paymentReceiptService ?? new Mock<IPaymentReceiptService>().Object,
                unitOfWork ?? new Mock<IUnitOfWork>().Object,
                logger ?? new Mock<ILogger<BillPaymentProcessingService>>().Object);
        }

        [Fact]
        public async Task ScheduleBillPaymentAsync_Should_Schedule_Payment_Successfully()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var billerId = Guid.NewGuid();
            var paymentId = Guid.NewGuid();
            var biller = NewBiller(billerId);
            var account = NewAccount(Guid.NewGuid(), customerId, 5000m);

            var billPaymentRepository = new Mock<IBillPaymentRepository>();
            var billerRepository = new Mock<IBillerRepository>();
            var accountService = new Mock<IAccountService>();
            var unitOfWork = new Mock<IUnitOfWork>();

            billerRepository.Setup(r => r.GetByIdAsync(billerId)).ReturnsAsync(biller);
            accountService.Setup(s => s.GetUserAccountsAsync(customerId)).ReturnsAsync(new List<Account> { account });

            var request = new ScheduleBillPaymentRequest(
                billerId,
                100m,
                "USD",
                DateTime.UtcNow.AddDays(1),
                "REF-123",
                "Test payment");

            var service = CreateService(
                billPaymentRepository: billPaymentRepository.Object,
                billerRepository: billerRepository.Object,
                accountService: accountService.Object,
                unitOfWork: unitOfWork.Object);

            // Act
            var response = await service.ScheduleBillPaymentAsync(customerId, request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(BillPaymentStatus.Pending, response.Status);
            Assert.Equal("Bill payment scheduled successfully", response.Message);
            billPaymentRepository.Verify(r => r.AddAsync(It.IsAny<BillPayment>()), Times.Once);
            unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ScheduleBillPaymentAsync_Should_Fail_When_Biller_Not_Found()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var billerId = Guid.NewGuid();
            var account = NewAccount(Guid.NewGuid(), customerId, 5000m);

            var billerRepository = new Mock<IBillerRepository>();
            var accountService = new Mock<IAccountService>();

            billerRepository.Setup(r => r.GetByIdAsync(billerId)).ReturnsAsync((Biller)null);
            accountService.Setup(s => s.GetUserAccountsAsync(customerId)).ReturnsAsync(new List<Account> { account });

            var request = new ScheduleBillPaymentRequest(
                billerId,
                100m,
                "USD",
                DateTime.UtcNow.AddDays(1),
                "REF-123",
                "Test payment");

            var service = CreateService(
                billerRepository: billerRepository.Object,
                accountService: accountService.Object);

            // Act
            var response = await service.ScheduleBillPaymentAsync(customerId, request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(BillPaymentStatus.Failed, response.Status);
            Assert.Contains("Biller not found", response.Message);
        }

        [Fact]
        public async Task ScheduleBillPaymentAsync_Should_Fail_When_Insufficient_Funds()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var billerId = Guid.NewGuid();
            var biller = NewBiller(billerId);
            var account = NewAccount(Guid.NewGuid(), customerId, 50m); // Only $50

            var billerRepository = new Mock<IBillerRepository>();
            var accountService = new Mock<IAccountService>();

            billerRepository.Setup(r => r.GetByIdAsync(billerId)).ReturnsAsync(biller);
            accountService.Setup(s => s.GetUserAccountsAsync(customerId)).ReturnsAsync(new List<Account> { account });

            var request = new ScheduleBillPaymentRequest(
                billerId,
                100m, // Requesting $100
                "USD",
                DateTime.UtcNow.AddDays(1),
                "REF-123",
                "Test payment");

            var service = CreateService(
                billerRepository: billerRepository.Object,
                accountService: accountService.Object);

            // Act
            var response = await service.ScheduleBillPaymentAsync(customerId, request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(BillPaymentStatus.Failed, response.Status);
            Assert.Contains("Insufficient funds", response.Message);
        }

        [Fact]
        public async Task ProcessBillPaymentAsync_Should_Process_Due_Payments()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var billerId = Guid.NewGuid();
            var paymentId = Guid.NewGuid();
            var biller = NewBiller(billerId);
            var payment = NewBillPayment(paymentId, customerId, billerId, 100m);
            payment.Biller = biller;
            var account = NewAccount(Guid.NewGuid(), customerId, 5000m);

            var billPaymentRepository = new Mock<IBillPaymentRepository>();
            var accountService = new Mock<IAccountService>();
            var billerIntegrationService = new Mock<IBillerIntegrationService>();
            var paymentReceiptService = new Mock<IPaymentReceiptService>();
            var unitOfWork = new Mock<IUnitOfWork>();

            billPaymentRepository.Setup(r => r.GetScheduledPaymentsDueAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(new List<BillPayment> { payment });
            accountService.Setup(s => s.GetUserAccountsAsync(customerId)).ReturnsAsync(new List<Account> { account });
            billerIntegrationService.Setup(s => s.SendPaymentToBillerAsync(It.IsAny<BillerPaymentRequest>()))
                .ReturnsAsync(new BillerPaymentResponse { Success = true, Message = "Success" });

            var service = CreateService(
                billPaymentRepository: billPaymentRepository.Object,
                accountService: accountService.Object,
                billerIntegrationService: billerIntegrationService.Object,
                paymentReceiptService: paymentReceiptService.Object,
                unitOfWork: unitOfWork.Object);

            // Act
            var responses = await service.ProcessBillPaymentAsync();

            // Assert
            Assert.NotNull(responses);
            Assert.Single(responses);
            Assert.Equal(BillPaymentStatus.Processed, responses[0].Status);
            Assert.True(responses[0].Success);
            paymentReceiptService.Verify(s => s.GeneratePaymentReceiptAsync(paymentId), Times.Once);
            unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ProcessBillPaymentAsync_Should_Handle_Insufficient_Funds()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var billerId = Guid.NewGuid();
            var paymentId = Guid.NewGuid();
            var biller = NewBiller(billerId);
            var payment = NewBillPayment(paymentId, customerId, billerId, 100m);
            payment.Biller = biller;
            var account = NewAccount(Guid.NewGuid(), customerId, 50m); // Insufficient funds

            var billPaymentRepository = new Mock<IBillPaymentRepository>();
            var accountService = new Mock<IAccountService>();
            var paymentRetryService = new Mock<IPaymentRetryService>();
            var unitOfWork = new Mock<IUnitOfWork>();

            billPaymentRepository.Setup(r => r.GetScheduledPaymentsDueAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(new List<BillPayment> { payment });
            accountService.Setup(s => s.GetUserAccountsAsync(customerId)).ReturnsAsync(new List<Account> { account });

            var service = CreateService(
                billPaymentRepository: billPaymentRepository.Object,
                accountService: accountService.Object,
                paymentRetryService: paymentRetryService.Object,
                unitOfWork: unitOfWork.Object);

            // Act
            var responses = await service.ProcessBillPaymentAsync();

            // Assert
            Assert.NotNull(responses);
            Assert.Single(responses);
            Assert.Equal(BillPaymentStatus.Failed, responses[0].Status);
            Assert.False(responses[0].Success);
            Assert.Contains("Insufficient funds", responses[0].Message);
            paymentRetryService.Verify(s => s.SchedulePaymentRetryAsync(It.IsAny<PaymentRetryRequest>()), Times.Once);
        }

        [Fact]
        public async Task CancelScheduledPaymentAsync_Should_Cancel_Pending_Payment()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var paymentId = Guid.NewGuid();
            var payment = NewBillPayment(paymentId, customerId, Guid.NewGuid(), 100m, BillPaymentStatus.Pending);

            var billPaymentRepository = new Mock<IBillPaymentRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();

            billPaymentRepository.Setup(r => r.GetByIdAsync(paymentId)).ReturnsAsync(payment);

            var service = CreateService(
                billPaymentRepository: billPaymentRepository.Object,
                unitOfWork: unitOfWork.Object);

            // Act
            var result = await service.CancelScheduledPaymentAsync(customerId, paymentId);

            // Assert
            Assert.True(result);
            billPaymentRepository.Verify(r => r.Update(payment), Times.Once);
            unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CancelScheduledPaymentAsync_Should_Fail_When_Payment_Not_Found()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var paymentId = Guid.NewGuid();

            var billPaymentRepository = new Mock<IBillPaymentRepository>();
            billPaymentRepository.Setup(r => r.GetByIdAsync(paymentId)).ReturnsAsync((BillPayment)null);

            var service = CreateService(billPaymentRepository: billPaymentRepository.Object);

            // Act
            var result = await service.CancelScheduledPaymentAsync(customerId, paymentId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CancelScheduledPaymentAsync_Should_Fail_When_Customer_Mismatch()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var differentCustomerId = Guid.NewGuid();
            var paymentId = Guid.NewGuid();
            var payment = NewBillPayment(paymentId, differentCustomerId, Guid.NewGuid(), 100m);

            var billPaymentRepository = new Mock<IBillPaymentRepository>();
            billPaymentRepository.Setup(r => r.GetByIdAsync(paymentId)).ReturnsAsync(payment);

            var service = CreateService(billPaymentRepository: billPaymentRepository.Object);

            // Act
            var result = await service.CancelScheduledPaymentAsync(customerId, paymentId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateScheduledPaymentAsync_Should_Update_Pending_Payment()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var billerId = Guid.NewGuid();
            var paymentId = Guid.NewGuid();
            var payment = NewBillPayment(paymentId, customerId, billerId, 100m, BillPaymentStatus.Pending);
            var biller = NewBiller(billerId);

            var billPaymentRepository = new Mock<IBillPaymentRepository>();
            var billerRepository = new Mock<IBillerRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();

            billPaymentRepository.Setup(r => r.GetByIdAsync(paymentId)).ReturnsAsync(payment);
            billerRepository.Setup(r => r.GetByIdAsync(billerId)).ReturnsAsync(biller);

            var request = new UpdateBillPaymentRequest(
                150m,
                DateTime.UtcNow.AddDays(2),
                "NEW-REF",
                "Updated payment");

            var service = CreateService(
                billPaymentRepository: billPaymentRepository.Object,
                billerRepository: billerRepository.Object,
                unitOfWork: unitOfWork.Object);

            // Act
            var result = await service.UpdateScheduledPaymentAsync(customerId, paymentId, request);

            // Assert
            Assert.True(result);
            Assert.Equal(150m, payment.Amount);
            billPaymentRepository.Verify(r => r.Update(payment), Times.Once);
            unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateScheduledPaymentAsync_Should_Fail_When_Payment_Not_Found()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var paymentId = Guid.NewGuid();

            var billPaymentRepository = new Mock<IBillPaymentRepository>();
            billPaymentRepository.Setup(r => r.GetByIdAsync(paymentId)).ReturnsAsync((BillPayment)null);

            var request = new UpdateBillPaymentRequest(150m, DateTime.UtcNow.AddDays(2), "NEW-REF", "Updated");

            var service = CreateService(billPaymentRepository: billPaymentRepository.Object);

            // Act
            var result = await service.UpdateScheduledPaymentAsync(customerId, paymentId, request);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateScheduledPaymentAsync_Should_Fail_When_Amount_Invalid()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var billerId = Guid.NewGuid();
            var paymentId = Guid.NewGuid();
            var payment = NewBillPayment(paymentId, customerId, billerId, 100m, BillPaymentStatus.Pending);
            var biller = NewBiller(billerId);

            var billPaymentRepository = new Mock<IBillPaymentRepository>();
            var billerRepository = new Mock<IBillerRepository>();

            billPaymentRepository.Setup(r => r.GetByIdAsync(paymentId)).ReturnsAsync(payment);
            billerRepository.Setup(r => r.GetByIdAsync(billerId)).ReturnsAsync(biller);

            var request = new UpdateBillPaymentRequest(
                50000m, // Amount exceeds max
                DateTime.UtcNow.AddDays(2),
                "NEW-REF",
                "Updated payment");

            var service = CreateService(
                billPaymentRepository: billPaymentRepository.Object,
                billerRepository: billerRepository.Object);

            // Act
            var result = await service.UpdateScheduledPaymentAsync(customerId, paymentId, request);

            // Assert
            Assert.False(result);
        }
    }
}
