using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration for ASP.NET Identity UserLogin table to handle key length limitations
/// </summary>
public class IdentityUserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> builder)
    {
        // Reduce key lengths for index compatibility
        builder.Property(l => l.LoginProvider)
            .HasMaxLength(128);
            
        builder.Property(l => l.ProviderKey)
            .HasMaxLength(128);
    }
}

