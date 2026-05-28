using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Bank.Domain.Entities;
using Bank.Infrastructure.Data;

namespace Bank.Api.Extensions.Data;

/// <summary>
/// Extension methods for data seeding and database management
/// </summary>
public static class DataSeedingExtensions
{
    /// <summary>
    /// Apply pending database migrations (or ensure schema is created for SQLite)
    /// </summary>
    public static async Task ApplyDatabaseMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<BankDbContext>();

            logger.LogInformation("🔍 Initializing database schema...");

            // For SQLite, EnsureCreated is the most reliable approach without pre-generated migrations
            var created = await dbContext.Database.EnsureCreatedAsync();
            if (created)
            {
                logger.LogInformation("✅ Database schema created successfully!");
            }
            else
            {
                logger.LogInformation("✅ Database schema already exists - no changes needed.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Unexpected error initializing database: {Message}", ex.Message);
            logger.LogWarning("⚠️ Application will continue - some features may be unavailable.");
        }
    }

    /// <summary>
    /// Seed initial data (roles, admin user, policies) with error handling
    /// </summary>
    public static async Task SeedInitialDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            logger.LogInformation("🌱 Starting data seeding...");
            
            await SeedRolesAsync(scope.ServiceProvider);
            await SeedAdminUserAsync(scope.ServiceProvider);
            await SeedPasswordPoliciesAsync(scope.ServiceProvider);
            
            logger.LogInformation("✅ Data seeding completed successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Error during data seeding: {Message}", ex.Message);
            logger.LogWarning("⚠️ Application will continue without seeding data. Database may not be accessible.");
        }
    }

    /// <summary>
    /// Seed system roles
    /// </summary>
    private static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
        
        string[] roles = { "Admin", "User", "Manager", "Auditor" };
        
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new Role 
                { 
                    Name = roleName, 
                    Description = $"{roleName} role with appropriate permissions" 
                });
            }
        }
    }

    /// <summary>
    /// Seed default admin user
    /// </summary>
    private static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var dbContext = serviceProvider.GetRequiredService<BankDbContext>();

        var adminEmail = "admin@finbank.com";
        
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new User
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "System",
                LastName = "Admin",
                PhoneNumber = "+1234567890",
                PhoneNumberConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                
                // Seed a default account for the admin
                if (!dbContext.Accounts.Any(a => a.UserId == adminUser.Id))
                {
                    dbContext.Accounts.Add(new Account
                    {
                        UserId = adminUser.Id,
                        AccountNumber = "DE-1000-AD",
                        AccountHolderName = "System Admin",
                        Balance = 50000.00m,
                        Type = Bank.Domain.Enums.AccountType.Checking,
                        Status = Bank.Domain.Enums.AccountStatus.Active
                    });
                    
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }

    /// <summary>
    /// Seed default password policies
    /// </summary>
    private static async Task SeedPasswordPoliciesAsync(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<BankDbContext>();
        
        if (!dbContext.PasswordPolicies.Any())
        {
            var policies = new[]
            {
                PasswordPolicy.CreateBasicPolicy(),
                PasswordPolicy.CreateStandardPolicy(),
                PasswordPolicy.CreateStrongPolicy(),
                PasswordPolicy.CreateEnterprisePolicy()
            };

            dbContext.PasswordPolicies.AddRange(policies);
            await dbContext.SaveChangesAsync();
        }
    }
}