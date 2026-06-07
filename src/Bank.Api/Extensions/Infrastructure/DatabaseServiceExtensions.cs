using Microsoft.EntityFrameworkCore;
using Bank.Infrastructure.Data;

namespace Bank.Api.Extensions.Infrastructure;

/// <summary>
/// Extension methods for database service registration
/// </summary>
public static class DatabaseServiceExtensions
{
    /// <summary>
    /// Register database and Entity Framework services
    /// </summary>
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");

        services.AddDbContext<BankDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlServerOptions =>
            {
                sqlServerOptions.MigrationsAssembly("Bank.Infrastructure");
                sqlServerOptions.CommandTimeout(300); // 5 minutes timeout for migrations
            });
        });

        return services;
    }
}
