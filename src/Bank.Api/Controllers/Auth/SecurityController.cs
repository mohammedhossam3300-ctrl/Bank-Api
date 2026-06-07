using Bank.Api.Helpers;
using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Bank.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bank.Api.Controllers.Auth;

/// <summary>
/// Controller for security management including IP whitelist and password policies
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SecurityController : ControllerBase
{
    private readonly IIpWhitelistService _ipWhitelistService;
    private readonly IPasswordPolicyService _passwordPolicyService;
    private readonly IAccountLockoutService _accountLockoutService;
    private readonly ILogger<SecurityController> _logger;

    public SecurityController(
        IIpWhitelistService ipWhitelistService,
        IPasswordPolicyService passwordPolicyService,
        IAccountLockoutService accountLockoutService,
        ILogger<SecurityController> logger)
    {
        _ipWhitelistService = ipWhitelistService;
        _passwordPolicyService = passwordPolicyService;
        _accountLockoutService = accountLockoutService;
        _logger = logger;
    }

    #region IP Whitelist Management

    /// <summary>
    /// Gets all IP whitelist entries (admin only)
    /// </summary>
    [HttpGet("ip-whitelist")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<IpWhitelistInfo>>> GetIpWhitelistEntries([FromQuery] IpWhitelistType? type = null, [FromQuery] bool activeOnly = true)
    {
        try
        {
            var entries = await _ipWhitelistService.GetWhitelistEntriesAsync(type, activeOnly);
            
            var entryInfos = entries.Select(e => new IpWhitelistInfo
            {
                Id = e.Id,
                IpAddress = e.IpAddress,
                IpRange = e.IpRange,
                Type = e.Type,
                Description = e.Description,
                IsActive = e.IsActive,
                ExpiresAt = e.ExpiresAt,
                CreatedByUserName = e.CreatedByUser?.UserName ?? "Unknown",
                ApprovedByUserName = e.ApprovedByUser?.UserName,
                ApprovedAt = e.ApprovedAt,
                CreatedAt = e.CreatedAt
            }).ToList();

            return Ok(entryInfos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving IP whitelist entries");
            return StatusCode(500, "Failed to retrieve IP whitelist entries");
        }
    }

    /// <summary>
    /// Adds an IP address to the whitelist (admin only)
    /// </summary>
    [HttpPost("ip-whitelist")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IpWhitelistResult>> AddIpToWhitelist([FromBody] AddIpWhitelistRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _ipWhitelistService.AddIpToWhitelistAsync(
                request.IpAddress,
                request.Type,
                request.Description,
                userId,
                request.IpRange,
                request.ExpiresAt);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding IP to whitelist");
            return StatusCode(500, "Failed to add IP to whitelist");
        }
    }

    /// <summary>
    /// Approves a pending IP whitelist entry (admin only)
    /// </summary>
    [HttpPost("ip-whitelist/{whitelistId}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ApproveIpWhitelist(Guid whitelistId, [FromBody] ApproveIpWhitelistRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var success = await _ipWhitelistService.ApproveIpWhitelistAsync(whitelistId, userId, request.Notes);

            if (!success)
            {
                return NotFound("IP whitelist entry not found");
            }

            return Ok(new { message = "IP whitelist entry approved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving IP whitelist entry {WhitelistId}", whitelistId);
            return StatusCode(500, "Failed to approve IP whitelist entry");
        }
    }

    /// <summary>
    /// Revokes an IP whitelist entry (admin only)
    /// </summary>
    [HttpDelete("ip-whitelist/{whitelistId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> RevokeIpWhitelist(Guid whitelistId)
    {
        try
        {
            var success = await _ipWhitelistService.RevokeIpWhitelistAsync(whitelistId);

            if (!success)
            {
                return NotFound("IP whitelist entry not found");
            }

            return Ok(new { message = "IP whitelist entry revoked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking IP whitelist entry {WhitelistId}", whitelistId);
            return StatusCode(500, "Failed to revoke IP whitelist entry");
        }
    }

    /// <summary>
    /// Gets pending IP whitelist approvals (admin only)
    /// </summary>
    [HttpGet("ip-whitelist/pending")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<IpWhitelistInfo>>> GetPendingApprovals()
    {
        try
        {
            var entries = await _ipWhitelistService.GetPendingApprovalsAsync();
            
            var entryInfos = entries.Select(e => new IpWhitelistInfo
            {
                Id = e.Id,
                IpAddress = e.IpAddress,
                IpRange = e.IpRange,
                Type = e.Type,
                Description = e.Description,
                IsActive = e.IsActive,
                ExpiresAt = e.ExpiresAt,
                CreatedByUserName = e.CreatedByUser?.UserName ?? "Unknown",
                CreatedAt = e.CreatedAt
            }).ToList();

            return Ok(entryInfos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending IP whitelist approvals");
            return StatusCode(500, "Failed to retrieve pending approvals");
        }
    }

    #endregion

    #region Password Policy Management

    /// <summary>
    /// Gets all active password policies (admin only)
    /// </summary>
    [HttpGet("password-policies")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<PasswordPolicyInfo>>> GetPasswordPolicies()
    {
        try
        {
            var policies = await _passwordPolicyService.GetActivePasswordPoliciesAsync();
            
            var policyInfos = policies.Select(p => new PasswordPolicyInfo
            {
                Id = p.Id,
                Name = p.Name,
                ComplexityLevel = p.ComplexityLevel,
                MinimumLength = p.MinimumLength,
                MaximumLength = p.MaximumLength,
                RequireUppercase = p.RequireUppercase,
                RequireLowercase = p.RequireLowercase,
                RequireDigits = p.RequireDigits,
                RequireSpecialCharacters = p.RequireSpecialCharacters,
                MinimumUniqueCharacters = p.MinimumUniqueCharacters,
                PasswordHistoryCount = p.PasswordHistoryCount,
                MaxPasswordAge = p.MaxPasswordAge,
                MaxFailedAttempts = p.MaxFailedAttempts,
                LockoutDuration = p.LockoutDuration,
                IsDefault = p.IsDefault,
                IsActive = p.IsActive,
                Description = p.Description
            }).ToList();

            return Ok(policyInfos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving password policies");
            return StatusCode(500, "Failed to retrieve password policies");
        }
    }

    /// <summary>
    /// Gets the default password policy
    /// </summary>
    [HttpGet("password-policies/default")]
    public async Task<ActionResult<PasswordPolicyInfo>> GetDefaultPasswordPolicy()
    {
        try
        {
            var policy = await _passwordPolicyService.GetDefaultPasswordPolicyAsync();
            
            if (policy == null)
            {
                return NotFound("No default password policy found");
            }

            var policyInfo = new PasswordPolicyInfo
            {
                Id = policy.Id,
                Name = policy.Name,
                ComplexityLevel = policy.ComplexityLevel,
                MinimumLength = policy.MinimumLength,
                MaximumLength = policy.MaximumLength,
                RequireUppercase = policy.RequireUppercase,
                RequireLowercase = policy.RequireLowercase,
                RequireDigits = policy.RequireDigits,
                RequireSpecialCharacters = policy.RequireSpecialCharacters,
                MinimumUniqueCharacters = policy.MinimumUniqueCharacters,
                PasswordHistoryCount = policy.PasswordHistoryCount,
                MaxPasswordAge = policy.MaxPasswordAge,
                MaxFailedAttempts = policy.MaxFailedAttempts,
                LockoutDuration = policy.LockoutDuration,
                IsDefault = policy.IsDefault,
                IsActive = policy.IsActive,
                Description = policy.Description
            };

            return Ok(policyInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving default password policy");
            return StatusCode(500, "Failed to retrieve default password policy");
        }
    }

    /// <summary>
    /// Validates a password against the current policy
    /// </summary>
    [HttpPost("password-policies/validate")]
    public async Task<ActionResult<PasswordValidationResult>> ValidatePassword([FromBody] ValidatePasswordRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _passwordPolicyService.ValidatePasswordAsync(request.Password, userId, request.ComplexityLevel);
            
            // Don't return the actual password in the response for security
            return Ok(new
            {
                result.IsValid,
                result.Errors,
                result.RequiredComplexityLevel,
                result.IsPasswordRecentlyUsed,
                result.IsCommonPassword,
                result.ContainsUserInfo,
                result.PasswordStrengthScore
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating password");
            return StatusCode(500, "Failed to validate password");
        }
    }

    /// <summary>
    /// Generates a secure password (admin only)
    /// </summary>
    [HttpPost("password-policies/generate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<string>> GenerateSecurePassword([FromBody] GeneratePasswordRequest request)
    {
        try
        {
            var password = await _passwordPolicyService.GenerateSecurePasswordAsync(request.ComplexityLevel);
            return Ok(new { password });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating secure password");
            return StatusCode(500, "Failed to generate secure password");
        }
    }

    #endregion

    #region Account Lockout Management

    /// <summary>
    /// Gets lockout status for the current user
    /// </summary>
    [HttpGet("lockout/status")]
    public async Task<ActionResult<AccountLockoutInfo>> GetLockoutStatus()
    {
        try
        {
            var userId = GetCurrentUserId();
            var lockout = await _accountLockoutService.GetLockoutStatusAsync(userId);

            if (lockout == null)
            {
                return Ok(new AccountLockoutInfo
                {
                    UserId = userId,
                    FailedAttempts = 0,
                    IsCurrentlyLocked = false
                });
            }

            var lockoutInfo = new AccountLockoutInfo
            {
                UserId = lockout.UserId,
                FailedAttempts = lockout.FailedAttempts,
                LockedUntil = lockout.LockedUntil,
                LockoutReason = lockout.LockoutReason,
                IsCurrentlyLocked = lockout.IsCurrentlyLocked,
                LastFailedAttempt = lockout.LastFailedAttempt,
                LastSuccessfulLogin = lockout.LastSuccessfulLogin,
                CreatedAt = lockout.CreatedAt
            };

            return Ok(lockoutInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lockout status");
            return StatusCode(500, "Failed to retrieve lockout status");
        }
    }

    /// <summary>
    /// Gets all locked accounts (admin only)
    /// </summary>
    [HttpGet("lockout/locked-accounts")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<AccountLockoutInfo>>> GetLockedAccounts()
    {
        try
        {
            var lockouts = await _accountLockoutService.GetLockedAccountsAsync();
            
            var lockoutInfos = lockouts.Select(l => new AccountLockoutInfo
            {
                UserId = l.UserId,
                UserName = l.User?.UserName ?? "Unknown",
                Email = l.User?.Email ?? "Unknown",
                FailedAttempts = l.FailedAttempts,
                LockedUntil = l.LockedUntil,
                LockoutReason = l.LockoutReason,
                IsCurrentlyLocked = l.IsCurrentlyLocked,
                LockoutNotes = l.LockoutNotes,
                LastFailedAttempt = l.LastFailedAttempt,
                LastSuccessfulLogin = l.LastSuccessfulLogin,
                LockedByUserName = l.LockedByUser?.UserName,
                CreatedAt = l.CreatedAt
            }).ToList();

            return Ok(lockoutInfos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving locked accounts");
            return StatusCode(500, "Failed to retrieve locked accounts");
        }
    }

    /// <summary>
    /// Manually locks an account (admin only)
    /// </summary>
    [HttpPost("lockout/{userId}/lock")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> LockAccount(Guid userId, [FromBody] LockAccountRequest request)
    {
        try
        {
            var lockedByUserId = GetCurrentUserId();
            var success = await _accountLockoutService.LockAccountAsync(
                userId,
                request.Reason,
                request.LockoutDuration,
                request.Notes,
                lockedByUserId);

            if (!success)
            {
                return BadRequest("Failed to lock account");
            }

            return Ok(new { message = "Account locked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error locking account {UserId}", userId);
            return StatusCode(500, "Failed to lock account");
        }
    }

    /// <summary>
    /// Manually unlocks an account (admin only)
    /// </summary>
    [HttpPost("lockout/{userId}/unlock")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UnlockAccount(Guid userId)
    {
        try
        {
            var unlockedByUserId = GetCurrentUserId();
            var success = await _accountLockoutService.UnlockAccountAsync(userId, unlockedByUserId);

            if (!success)
            {
                return NotFound("Account not found or not locked");
            }

            return Ok(new { message = "Account unlocked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlocking account {UserId}", userId);
            return StatusCode(500, "Failed to unlock account");
        }
    }

    /// <summary>
    /// Gets lockout statistics (admin only)
    /// </summary>
    [HttpGet("lockout/statistics")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<LockoutStatistics>> GetLockoutStatistics()
    {
        try
        {
            var statistics = await _accountLockoutService.GetLockoutStatisticsAsync();
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lockout statistics");
            return StatusCode(500, "Failed to retrieve lockout statistics");
        }
    }

    #endregion

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value 
            ?? User.FindFirst("id")?.Value;

        return userIdClaim != null && Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}

