namespace Bank.Api.Extensions.Infrastructure;

/// <summary>
/// Extension methods for caching and session service registration
/// </summary>
public static class CachingServiceExtensions
{
    /// <summary>
    /// Register caching and session services
    /// </summary>
    public static IServiceCollection AddCachingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDistributedMemoryCache();

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        return services;
    }
}
