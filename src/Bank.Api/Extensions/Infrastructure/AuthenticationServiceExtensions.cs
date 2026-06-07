using Microsoft.AspNetCore.Identity;
using Bank.Domain.Entities;
using Bank.Infrastructure.Data;

namespace Bank.Api.Extensions.Infrastructure;

/// <summary>
/// Extension methods for authentication and authorization service registration
/// </summary>
public static class AuthenticationServiceExtensions
{
    /// <summary>
    /// Register authentication and authorization services
    /// </summary>
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // ASP.NET Core Identity — strong password and lockout policy
        services.AddIdentity<User, Role>(options =>
        {
            // Password complexity: uppercase + lowercase + digit + special char, min 12 chars
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 12;
            options.Password.RequiredUniqueChars = 6;

            // Account lockout: lock after 5 consecutive failures for 15 minutes
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<BankDbContext>()
        .AddDefaultTokenProviders();

        // JWT Authentication — explicit, strict validation parameters
        services.AddAuthentication(opts => {
            opts.DefaultAuthenticateScheme = "Bearer";
            opts.DefaultChallengeScheme = "Bearer";
        }).AddJwtBearer(options => {
            var jwtSettings = configuration.GetSection("Jwt");

            var rawKey = jwtSettings["Key"];
            if (string.IsNullOrWhiteSpace(rawKey))
                throw new InvalidOperationException("JWT signing key is not configured. Set the Jwt:Key configuration value.");

            var key = System.Text.Encoding.ASCII.GetBytes(rawKey);
            if (key.Length < 32)
                throw new InvalidOperationException("JWT signing key must be at least 32 bytes (256 bits).");

            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),
                RequireExpirationTime = true,
            };
        });

        return services;
    }
}
