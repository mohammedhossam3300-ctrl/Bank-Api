using Microsoft.AspNetCore.Identity;
using Bank.Domain.Enums;
using Bank.Domain.ValueObjects;
using Bank.Domain.Common;
using AccountEntity = Bank.Domain.Entities.Account;

namespace Bank.Domain.Entities;

/// <summary>
/// Application user entity extending ASP.NET Core Identity.
/// Uses Guid as the primary key type.
/// </summary>
public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Two-Factor Authentication properties
    public override bool TwoFactorEnabled { get; set; } = false;
    public TwoFactorStatus TwoFactorStatus { get; set; } = TwoFactorStatus.NotSetup;
    public string? TwoFactorSecretKey { get; set; } // For authenticator apps
    public string? TwoFactorBackupCodes { get; set; } // JSON array of backup codes
    public DateTime? TwoFactorSetupDate { get; set; }
    public DateTime? LastTwoFactorUsed { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public ICollection<AccountEntity> Accounts { get; set; } = new List<AccountEntity>();
    public ICollection<TwoFactorToken> TwoFactorTokens { get; set; } = new List<TwoFactorToken>();

    public void SoftDelete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }

    public void EnableTwoFactor(string? secretKey = null, string? backupCodes = null)
    {
        TwoFactorEnabled = true;
        TwoFactorStatus = TwoFactorStatus.Active;
        TwoFactorSecretKey = secretKey;
        TwoFactorBackupCodes = backupCodes;
        TwoFactorSetupDate = DateTime.UtcNow;
    }

    public void DisableTwoFactor()
    {
        TwoFactorEnabled = false;
        TwoFactorStatus = TwoFactorStatus.Disabled;
        TwoFactorSecretKey = null;
        TwoFactorBackupCodes = null;
    }

    public void MarkTwoFactorUsed()
    {
        LastTwoFactorUsed = DateTime.UtcNow;
    }
}

/// <summary>
/// Application role entity extending ASP.NET Core Identity.
/// Uses Guid as the primary key type.
/// </summary>
public class Role : IdentityRole<Guid>
{
    public string? Description { get; set; }
}
