using FluentValidation;

namespace Bank.Api.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for CQRS and validation service registration
/// </summary>
public static class CqrsServiceExtensions
{
    /// <summary>
    /// Register CQRS and validation services
    /// </summary>
    public static IServiceCollection AddCqrsServices(this IServiceCollection services)
    {
        // MediatR for CQRS pattern
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Bank.Application.Commands.Transaction.InitiateTransactionCommand).Assembly));
        
        // FluentValidation
        services.AddValidatorsFromAssembly(typeof(Bank.Application.Validators.Transaction.InitiateTransactionCommandValidator).Assembly);

        return services;
    }
}
