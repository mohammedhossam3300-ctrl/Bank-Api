using Bank.Api.Middleware;
using Hangfire;

namespace Bank.Api.Extensions.Middleware;

/// <summary>
/// Extension methods for configuring middleware pipeline
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Configure development-specific middleware
    /// </summary>
    public static WebApplication ConfigureDevelopmentMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            // Swagger documentation
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Bank Simulator API v1");
                options.RoutePrefix = string.Empty; // Swagger UI at root
            });

            // Hangfire dashboard for background jobs
            app.UseHangfireDashboard("/hangfire", new Hangfire.DashboardOptions
            {
                Authorization = new[] { new Hangfire.Dashboard.LocalRequestsOnlyAuthorizationFilter() }
            });
        }

        return app;
    }

    /// <summary>
    /// Configure security and audit middleware
    /// </summary>
    public static WebApplication ConfigureSecurityMiddleware(this WebApplication app)
    {
        // Global exception handling (must be first)
        app.UseMiddleware<GlobalExceptionMiddleware>();
        
        // Audit logging for compliance
        app.UseMiddleware<AuditMiddleware>();
        
        // Two-factor authentication enforcement
        app.UseMiddleware<TwoFactorAuthMiddleware>();

        return app;
    }

    /// <summary>
    /// Configure standard ASP.NET Core middleware
    /// </summary>
    public static WebApplication ConfigureStandardMiddleware(this WebApplication app)
    {
        // CORS policy
        app.UseCors("AllowAngular");
        
        // Session management
        app.UseSession();
        
        // Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();
        
        // Controller routing
        app.MapControllers();

        return app;
    }
}