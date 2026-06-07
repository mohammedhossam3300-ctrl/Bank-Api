using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Bank.Infrastructure.Data;

/// <summary>
/// Design-time factory for BankDbContext to support EF Core tools
/// </summary>
public class BankDbContextFactory : IDesignTimeDbContextFactory<BankDbContext>
{
    public BankDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BankDbContext>();

        // Use the remote SQL Server connection string for migrations
        var connectionString = "Server=db48070.public.databaseasp.net; Database=db48070; User Id=db48070; Password=8s@A=4FaZ-k6; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;";

        optionsBuilder.UseSqlServer(connectionString, sqlServerOptions =>
        {
            sqlServerOptions.MigrationsAssembly("Bank.Infrastructure");
        });

        return new BankDbContext(optionsBuilder.Options);
    }
}
