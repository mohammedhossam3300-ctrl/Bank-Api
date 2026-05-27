namespace Bank.Api.Extensions.Configuration;

/// <summary>
/// Extension methods for CORS policy registration
/// </summary>
public static class CorsServiceExtensions
{
    /// <summary>
    /// Register CORS policies
    /// </summary>
    public static IServiceCollection AddCorsServices(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAngular", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }
}
