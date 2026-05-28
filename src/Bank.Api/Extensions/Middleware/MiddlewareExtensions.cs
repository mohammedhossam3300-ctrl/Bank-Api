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
                <link href='https://fonts.googleapis.com/css2?family=Poppins:wght@400;600;700&display=swap' rel='stylesheet'>
                <link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css'>
                <style>
                    #swagger-nav-bar {
                        display: flex;
                        align-items: center;
                        gap: 8px;
                        background: rgba(14,14,14,0.97);
                        padding: 0 28px;
                        height: 60px;
                        border-bottom: 1px solid rgba(30,144,255,0.25);
                        font-family: 'Poppins', sans-serif;
                        position: sticky;
                        top: 0;
                        z-index: 9999;
                        box-shadow: 0 2px 20px rgba(0,0,0,0.5);
                        box-sizing: border-box;
                    }
                    #swagger-nav-bar .snb-logo {
                        width: 34px; height: 34px;
                        object-fit: contain;
                        filter: drop-shadow(0 0 8px rgba(30,144,255,0.5));
                        flex-shrink: 0;
                        margin-right: 4px;
                    }
                    #swagger-nav-bar .snb-title {
                        color: #fff;
                        font-weight: 700;
                        font-size: 0.95rem;
                        margin-right: auto;
                        letter-spacing: 0.2px;
                        white-space: nowrap;
                    }
                    #swagger-nav-bar a {
                        color: #9a9a9a;
                        text-decoration: none;
                        font-size: 0.875rem;
                        font-weight: 600;
                        padding: 7px 15px;
                        border-radius: 8px;
                        transition: background 0.2s, color 0.2s, border-color 0.2s;
                        display: inline-flex;
                        align-items: center;
                        gap: 7px;
                        border: 1px solid transparent;
                        white-space: nowrap;
                    }
                    #swagger-nav-bar a:hover {
                        color: #fff;
                        background: rgba(30,144,255,0.15);
                        border-color: rgba(30,144,255,0.4);
                    }
                    #swagger-nav-bar a.snb-active {
                        color: #fff;
                        background: linear-gradient(135deg,#1e90ff,#0047ab);
                        border-color: #1e90ff;
                        box-shadow: 0 3px 10px rgba(30,144,255,0.35);
                    }
                    @media (max-width: 600px) {
                        #swagger-nav-bar { padding: 0 14px; gap: 4px; }
                        #swagger-nav-bar .snb-title { display: none; }
                        #swagger-nav-bar a { padding: 6px 10px; font-size: 0.82rem; gap: 5px; }
                    }
                </style>
                <div id='swagger-nav-bar'>
                    <img class='snb-logo' src='/images/logo.png' alt='logo'/>
                    <span class='snb-title'>Bank Management API</span>
                    <a href='/' onclick='event.stopPropagation();event.preventDefault();window.location.href=this.href;'><i class='fa-solid fa-house'></i> Home</a>
                    <a href='/Docs.html' onclick='event.stopPropagation();event.preventDefault();window.location.href=this.href;'><i class='fa-solid fa-book'></i> Docs</a>
                    <a class='snb-active' href='/swagger'><i class='fa-solid fa-code'></i> Swagger</a>
                </div>
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