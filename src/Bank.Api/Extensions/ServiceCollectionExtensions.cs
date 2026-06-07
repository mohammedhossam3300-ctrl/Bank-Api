using Bank.Api.Extensions.Configuration;
using Bank.Api.Extensions.DependencyInjection;
using Bank.Api.Extensions.Infrastructure;

namespace Bank.Api.Extensions;

/// <summary>
/// Main orchestrator for service registrations - delegates to specialized extension classes
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register all Bank API services in the correct order
    /// </summary>
    public static IServiceCollection AddBankApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Core infrastructure
        services.AddDatabaseServices(configuration);
        services.AddCachingServices(configuration);
        services.AddAuthenticationServices(configuration);
        
        // Data access
        services.AddRepositoryServices();
        
        // Business logic
        services.AddApplicationServices(configuration);
        services.AddInfrastructureServices();
        
        // CQRS and validation
        services.AddCqrsServices();
        
        // Background jobs
        services.AddBackgroundJobServices(configuration);
        
        // API features
        services.AddApiDocumentationServices();
        services.AddCorsServices(configuration);

        return services;
    }
}