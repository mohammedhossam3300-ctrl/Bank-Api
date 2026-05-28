using Bank.Application.Interfaces;

namespace Bank.Api.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for infrastructure service registration (external integrations)
/// </summary>
public static class InfrastructureServiceExtensions
{
    /// <summary>
    /// Register all infrastructure services (external integrations)
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Communication Services
        services.AddScoped<IEmailService, Bank.Infrastructure.Services.EmailService>();
        services.AddScoped<ISmsService, Bank.Infrastructure.Services.SmsService>();

        // Rate Limiting Service — Singleton so hit counts persist across requests
        services.AddSingleton<IRateLimitingService, Bank.Infrastructure.Services.RateLimitingService>();

        return services;
    }
}
