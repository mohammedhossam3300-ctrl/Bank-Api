using Bank.Api.Extensions;
using Bank.Api.Extensions.Configuration;
using Bank.Api.Extensions.Data;
using Bank.Api.Extensions.DependencyInjection;
using Bank.Api.Extensions.Infrastructure;
using Bank.Api.Extensions.Middleware;
using Bank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add controllers with JSON configuration
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// === SERVICE REGISTRATIONS (Organized by Concern) ===

// 1. Database & Data Access Layer
builder.Services.AddDatabaseServices(builder.Configuration);

// 2. Caching & Session Management
builder.Services.AddCachingServices(builder.Configuration);

// 3. Authentication & Authorization
builder.Services.AddAuthenticationServices(builder.Configuration);

// 4. Repository Layer (Data Access)
builder.Services.AddRepositoryServices();

// 5. Application Services (Business Logic)
builder.Services.AddApplicationServices(builder.Configuration);

// 6. Infrastructure Services (External Integrations)
builder.Services.AddInfrastructureServices();

// 7. CQRS & Validation
builder.Services.AddCqrsServices();

// 8. AutoMapper (Object Mapping)
builder.Services.AddAutoMapperServices();

// 9. Background Jobs
builder.Services.AddBackgroundJobServices(builder.Configuration);

// 10. API Documentation
builder.Services.AddApiDocumentationServices();

// 11. CORS Policies
builder.Services.AddCorsServices(builder.Configuration);

var app = builder.Build();

// === DATABASE MIGRATION ===
var skipMigrations = builder.Configuration.GetValue<bool>("DatabaseSettings:SkipMigrations", false);
var skipSeeding = builder.Configuration.GetValue<bool>("DatabaseSettings:SkipSeeding", false);

if (!skipMigrations)
{
    await app.ApplyDatabaseMigrationsAsync();
}
else
{
    app.Logger.LogWarning("⚠️ Database migrations skipped (development mode)");
}

// === DATA SEEDING ===
if (!skipSeeding)
{
    await app.SeedInitialDataAsync();
}
else
{
    app.Logger.LogWarning("⚠️ Data seeding skipped (development mode)");
}

// === MIDDLEWARE PIPELINE (Order Matters) ===

// 1. Development-specific middleware
app.ConfigureDevelopmentMiddleware();

// 2. Security & audit middleware
app.ConfigureSecurityMiddleware();

// 3. Standard ASP.NET Core middleware
app.ConfigureStandardMiddleware();

app.Run();

// Make Program class accessible for testing
public partial class Program { }
