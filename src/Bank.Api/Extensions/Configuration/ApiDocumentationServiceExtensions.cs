using Microsoft.OpenApi.Models;

namespace Bank.Api.Extensions.Configuration;

/// <summary>
/// Extension methods for API documentation service registration
/// </summary>
public static class ApiDocumentationServiceExtensions
{
    /// <summary>
    /// Register API documentation services
    /// </summary>
    public static IServiceCollection AddApiDocumentationServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => type.FullName!.Replace("+", "."));

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Bank Payment Simulator API",
                Version = "v1",
                Description = "Fintech Payment System Simulator - ACH, WPS, RTGS & Batch Processing"
            });

            // JWT Bearer auth in Swagger
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
