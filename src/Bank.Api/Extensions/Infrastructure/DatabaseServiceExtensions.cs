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
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly("Bank.Infrastructure");
                npgsqlOptions.CommandTimeout(300);
            });
        });

        return services;
    }
}
