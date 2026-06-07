namespace Bank.Api.Extensions.Configuration;

using Bank.Api.Configuration;

/// <summary>
/// Extension methods for CORS policy registration
/// Implements secure CORS configuration using origin whitelisting
/// </summary>
public static class CorsServiceExtensions
{
    /// <summary>
    /// Register CORS policies with environment-specific origins using whitelist approach.
    /// 
    /// Security: This implementation uses a whitelist-based approach and does NOT use AllowAnyOrigin().
    /// Only explicitly configured trusted origins are allowed. The policy is environment-specific
    /// and read from configuration (appsettings.json, environment variables).
    /// 
    /// Allowed origins must be defined in appsettings.json:
    /// {
    ///   "Cors": {
    ///     "AllowedOrigins": ["https://trustedhost.com", "http://localhost:3000"],
    ///     "AllowedMethods": ["GET", "POST", "PUT", "DELETE"],
    ///     "AllowedHeaders": ["Content-Type", "Authorization"],
    ///     "AllowCredentials": true,
    ///     "MaxAge": 3600
    ///   }
    /// }
    /// </summary>
    public static IServiceCollection AddCorsServices(this IServiceCollection services, IConfiguration configuration)
    {
        var corsSettings = new CorsSettings();
        configuration.GetSection("Cors").Bind(corsSettings);

        // Validate configuration
        if (corsSettings.AllowedOrigins.Length == 0)
        {
            throw new InvalidOperationException(
                "CORS AllowedOrigins is not configured. Please add 'Cors:AllowedOrigins' to appsettings.json with explicit trusted origins. Example: [\"https://yourdomain.com\", \"http://localhost:3000\"]");
        }

        // Security check: Ensure no wildcard or open configurations
        ValidateOrigins(corsSettings.AllowedOrigins);

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAngular", policy =>
            {
                // Whitelist approach: Only allow explicitly configured origins
                policy
                    .WithOrigins(corsSettings.AllowedOrigins)
                    .WithMethods(corsSettings.AllowedMethods)
                    .WithHeaders(corsSettings.AllowedHeaders)
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(corsSettings.MaxAge));

                // Only allow credentials if explicitly configured
                if (corsSettings.AllowCredentials)
                {
                    policy.AllowCredentials();
                }
            });
        });

        return services;
    }

    /// <summary>
    /// Validates that origins don't use wildcard or unsafe patterns
    /// </summary>
    private static void ValidateOrigins(string[] allowedOrigins)
    {
        foreach (var origin in allowedOrigins)
        {
            if (string.IsNullOrWhiteSpace(origin))
            {
                throw new InvalidOperationException("CORS origin cannot be empty or whitespace");
            }

            // Reject wildcard origin
            if (origin == "*")
            {
                throw new InvalidOperationException(
                    "CORS: Wildcard origin (*) is not allowed. This would allow requests from any domain. " +
                    "Use explicit domain whitelist instead. Example: [\"https://domain.com\"]");
            }

            // Warn about localhost in production (should use env-specific config)
            if (origin.Contains("localhost") || origin.Contains("127.0.0.1"))
            {
                // This is OK for development, but recommend proper environment separation
            }
        }
    }
}

