using Bank.Application.Commands.Behaviors;
using FluentValidation;
using MediatR;

namespace Bank.Api.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for CQRS and validation service registration.
/// </summary>
public static class CqrsServiceExtensions
{
    public static IServiceCollection AddCqrsServices(this IServiceCollection services)
    {
        // MediatR — scans the Application assembly for all IRequestHandler<,> registrations
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(
                typeof(Bank.Application.Commands.Transaction.InitiateTransactionCommand).Assembly));

        // ValidationBehavior — runs FluentValidation validators before every handler
        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        // FluentValidation — scans Application assembly for all AbstractValidator<T>
        services.AddValidatorsFromAssembly(
            typeof(Bank.Application.Validators.Transaction.InitiateTransactionCommandValidator).Assembly);

        return services;
    }
}
