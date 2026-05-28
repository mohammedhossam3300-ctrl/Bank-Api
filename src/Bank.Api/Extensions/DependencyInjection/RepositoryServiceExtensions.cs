using Bank.Domain.Interfaces;
using Bank.Application.Interfaces;
using Bank.Infrastructure.Repositories;

namespace Bank.Api.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for repository service registration
/// </summary>
public static class RepositoryServiceExtensions
{
    /// <summary>
    /// Register all repository services
    /// </summary>
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IAccountLockoutRepository, AccountLockoutRepository>();
        services.AddScoped<IIpWhitelistRepository, IpWhitelistRepository>();
        services.AddScoped<IPasswordPolicyRepository, PasswordPolicyRepository>();
        services.AddScoped<IPasswordHistoryRepository, PasswordHistoryRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRecurringPaymentRepository, RecurringPaymentRepository>();
        services.AddScoped<IPaymentTemplateRepository, PaymentTemplateRepository>();
        services.AddScoped<IBeneficiaryRepository, BeneficiaryRepository>();
        services.AddScoped<IStatementRepository, StatementRepository>();
        services.AddScoped<ILoanRepository, LoanRepository>();
        services.AddScoped<ICardRepository, CardRepository>();
        services.AddScoped<ICardTransactionRepository, CardTransactionRepository>();
        services.AddScoped<IBillerRepository, BillerRepository>();
        services.AddScoped<IBillPaymentRepository, BillPaymentRepository>();
        services.AddScoped<IPaymentRetryRepository, PaymentRetryRepository>();
        services.AddScoped<IPaymentReceiptRepository, PaymentReceiptRepository>();
        services.AddScoped<IBillerHealthCheckRepository, BillerHealthCheckRepository>();
        services.AddScoped<IBillPresentmentRepository, BillPresentmentRepository>();
        services.AddScoped<IDepositProductRepository, DepositProductRepository>();
        services.AddScoped<IFixedDepositRepository, FixedDepositRepository>();
        services.AddScoped<IInterestTierRepository, InterestTierRepository>();
        services.AddScoped<IDepositTransactionRepository, DepositTransactionRepository>();
        services.AddScoped<IDepositCertificateRepository, DepositCertificateRepository>();
        services.AddScoped<IMaturityNoticeRepository, MaturityNoticeRepository>();

        return services;
    }
}
