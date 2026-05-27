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
        var dbPath = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=banking.db";

        services.AddDbContext<BankDbContext>(options =>
        {
            options.UseSqlite(dbPath, sqliteOptions =>
            {
                sqliteOptions.MigrationsAssembly("Bank.Infrastructure");
            });
        });

        return services;
    }
}
