using Hangfire;
using Hangfire.InMemory;

namespace Bank.Api.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for background job service registration
/// </summary>
public static class BackgroundJobServiceExtensions
{
    /// <summary>
    /// Register background job services
    /// </summary>
    public static IServiceCollection AddBackgroundJobServices(this IServiceCollection services, IConfiguration configuration)
    {
        var allowOfflineMode = configuration.GetValue<bool>("DatabaseSettings:AllowOfflineMode", false);

        if (!allowOfflineMode)
        {
            services.AddHangfire(config => config
                .UseInMemoryStorage());
            services.AddHangfireServer();
        }

        return services;
    }
}
