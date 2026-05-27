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

        optionsBuilder.UseSqlite("Data Source=banking.db", sqliteOptions =>
        {
            sqliteOptions.MigrationsAssembly("Bank.Infrastructure");
        });

        return new BankDbContext(optionsBuilder.Options);
    }
}
