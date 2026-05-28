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
        // Swagger documentation (always enabled so the UI is reachable)
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Bank Simulator API v1");
            options.RoutePrefix = "swagger"; // Swagger UI at /swagger
            options.DocumentTitle = "Bank Management API";
            options.HeadContent = @"
                <style>
                    #swagger-nav-bar {
                        display: flex;
                        align-items: center;
                        gap: 16px;
                        background: linear-gradient(135deg, #1a1a1a 0%, #2a2a2a 100%);
                        padding: 10px 24px;
                        border-bottom: 2px solid #1e90ff;
                        font-family: Poppins, sans-serif;
                        position: sticky;
                        top: 0;
                        z-index: 9999;
                        box-shadow: 0 4px 15px rgba(30,144,255,0.3);
                    }
                    #swagger-nav-bar .snb-logo {
                        width: 36px;
                        height: 36px;
                        object-fit: contain;
                        filter: drop-shadow(0 0 8px rgba(30,144,255,0.6));
                        flex-shrink: 0;
                    }
                    #swagger-nav-bar .snb-title {
                        color: white;
                        font-weight: 700;
                        font-size: 1rem;
                        margin-right: auto;
                        letter-spacing: 0.3px;
                    }
                    #swagger-nav-bar a {
                        color: #b0b0b0;
                        text-decoration: none;
                        font-size: 0.9rem;
                        font-weight: 600;
                        padding: 7px 16px;
                        border-radius: 8px;
                        transition: all 0.2s ease;
                        display: flex;
                        align-items: center;
                        gap: 7px;
                        border: 1px solid transparent;
                    }
                    #swagger-nav-bar a:hover {
                        color: white;
                        background: rgba(30,144,255,0.15);
                        border-color: #1e90ff;
                    }
                    #swagger-nav-bar a.snb-active {
                        color: white;
                        background: linear-gradient(135deg,#1e90ff,#0047ab);
                        border-color: #1e90ff;
                        box-shadow: 0 3px 10px rgba(30,144,255,0.4);
                    }
                </style>
                <div id='swagger-nav-bar'>
                    <img class='snb-logo' src='/images/logo.png' alt='logo'/>
                    <span class='snb-title'>Bank Management API</span>
                    <a href='/'>&#8962; Home</a>
                    <a href='/Docs.html'>&#128214; Docs</a>
                    <a class='snb-active' href='/swagger'>&#128196; Swagger</a>
                </div>
                <link href='https://fonts.googleapis.com/css2?family=Poppins:wght@400;600;700&display=swap' rel='stylesheet'>
            ";
        });

        if (app.Environment.IsDevelopment())
        {
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
        // Static files (serves wwwroot — Home page, Docs, CSS, images)
        app.UseDefaultFiles(); // serves index.html at /
        app.UseStaticFiles();

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