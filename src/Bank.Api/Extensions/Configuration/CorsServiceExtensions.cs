namespace Bank.Api.Extensions.Configuration;

using Bank.Api.Configuration;

/// <summary>
/// Extension methods for CORS policy registration
/// </summary>
public static class CorsServiceExtensions
{
    /// <summary>
    /// Register CORS policies with environment-specific origins
    /// </summary>
    public static IServiceCollection AddCorsServices(this IServiceCollection services, IConfiguration configuration)
    {
        var corsSettings = new CorsSettings();
        configuration.GetSection("Cors").Bind(corsSettings);

        // Validate configuration
        if (corsSettings.AllowedOrigins.Length == 0)
        {
            throw new InvalidOperationException(
                "CORS AllowedOrigins is not configured. Please add 'Cors:AllowedOrigins' to appsettings.json");
        }

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAngular", policy =>
            {
                policy
                    .WithOrigins(corsSettings.AllowedOrigins)
                    .WithMethods(corsSettings.AllowedMethods)
                    .WithHeaders(corsSettings.AllowedHeaders)
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(corsSettings.MaxAge));

                if (corsSettings.AllowCredentials)
                {
                    policy.AllowCredentials();
                }
            });
        });

        return services;
    }
}
