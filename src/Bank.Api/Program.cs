using Bank.Api.Extensions;
using Bank.Api.Extensions.Configuration;
using Bank.Api.Extensions.Data;
using Bank.Api.Extensions.DependencyInjection;
using Bank.Api.Extensions.Infrastructure;
using Bank.Api.Extensions.Middleware;
using Bank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

// Load environment variables from .env file
var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envPath))
{
    foreach (var line in File.ReadAllLines(envPath))
    {
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
            continue;

        var parts = line.Split('=', 2);
        if (parts.Length == 2)
        {
            var key = parts[0].Trim();
            var value = parts[1].Trim();
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}

var builder = WebApplication.CreateBuilder(args);

// Process appsettings.json to replace placeholders with environment variables
ProcessConfigurationPlaceholders(builder.Configuration);

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

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

// Make Program class accessible for testing
public partial class Program
{
    /// <summary>
    /// Process configuration to replace {PLACEHOLDER} values with environment variables
    /// </summary>
    private static void ProcessConfigurationPlaceholders(IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(connectionString))
        {
            var processedConnection = ReplacePlaceholders(connectionString);
            // Force set the processed connection string
            ((IConfigurationBuilder)config).AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = processedConnection
            });
        }

        var jwtKey = config["Jwt:Key"];
        if (!string.IsNullOrEmpty(jwtKey) && jwtKey.StartsWith('{') && jwtKey.EndsWith('}'))
        {
            var processedKey = ReplacePlaceholders(jwtKey);
            ((IConfigurationBuilder)config).AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = processedKey
            });
        }
    }

    private static string ReplacePlaceholders(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var result = value;
        var pattern = @"\{([^}]+)\}";
        
        foreach (System.Text.RegularExpressions.Match match in System.Text.RegularExpressions.Regex.Matches(value, pattern))
        {
            var placeholder = match.Groups[1].Value;
            var envValue = Environment.GetEnvironmentVariable(placeholder);
            if (!string.IsNullOrEmpty(envValue))
            {
                result = result.Replace(match.Value, envValue);
            }
        }

        return result;
    }
}
