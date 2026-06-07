using Bank.Api.Constants;
using Bank.Api.Helpers;
using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Bank.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bank.Api.Controllers.Account;

/// <summary>
/// Controller for managing joint account operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JointAccountController : ControllerBase
{
    private readonly IJointAccountService _jointAccountService;
    private readonly IAccountService _accountService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<JointAccountController> _logger;

    public JointAccountController(
        IJointAccountService jointAccountService,
        IAccountService accountService,
        IUserRepository userRepository,
        ILogger<JointAccountController> logger)
    {
        _jointAccountService = jointAccountService;
        _accountService = accountService;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Add a joint holder to an account
    /// </summary>
    [HttpPost("add-holder")]
    public async Task<ActionResult<bool>> AddJointHolder([FromBody] AddJointHolderRequest request)
    {
        try
        {
            var currentUserId = this.GetCurrentUserIdRequired();
            
            // Verify current user has access to the account
            var hasAccess = await _accountService.CanUserAccessAccountAsync(request.AccountId, currentUserId);
            if (!hasAccess)
            {
                return this.CreateForbiddenResponse(ErrorMessages.YouDontHaveAccessToThisAccount);
            }

            var success = await _jointAccountService.AddJointHolderAsync(
                request.AccountId, request.UserId, request.Role, currentUserId);

            if (success)
            {
                return this.CreateSuccessResponse("Joint holder added successfully");
            }

            return this.CreateErrorResponse("Failed to add joint holder", 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding joint holder to account {AccountId}", request.AccountId);
            return this.CreateErrorResponse("An error occurred while adding joint holder");
        }
    }

    /// <summary>
    /// Remove a joint holder from an account
    /// </summary>
    [HttpPost("remove-holder")]
    public async Task<ActionResult<bool>> RemoveJointHolder([FromBody] RemoveJointHolderRequest request)
    {
        try
        {
            var currentUserId = this.GetCurrentUserIdRequired();
            
            // Verify current user has access to the account
            var hasAccess = await _accountService.CanUserAccessAccountAsync(request.AccountId, currentUserId);
            if (!hasAccess)
            {
                return this.CreateForbiddenResponse(ErrorMessages.YouDontHaveAccessToThisAccount);
            }

            var success = await _jointAccountService.RemoveJointHolderAsync(
                request.AccountId, request.UserId, currentUserId);

            if (success)
            {
                return this.CreateSuccessResponse("Joint holder removed successfully");
            }

            return this.CreateErrorResponse("Failed to remove joint holder", 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing joint holder from account {AccountId}", request.AccountId);
            return this.CreateErrorResponse("An error occurred while removing joint holder");
        }
    }

    /// <summary>
    /// Update joint holder role and permissions
    /// </summary>
    [HttpPut("update-holder")]
    public async Task<ActionResult<bool>> UpdateJointHolder([FromBody] UpdateJointHolderRequest request)
    {
        try
        {
            var currentUserId = this.GetCurrentUserId();
            
            // Verify current user has access to the account
            var hasAccess = await _accountService.CanUserAccessAccountAsync(request.AccountId, currentUserId);
            if (!hasAccess)
            {
                return Forbid(ErrorMessages.YouDontHaveAccessToThisAccount);
            }

            var success = await _jointAccountService.UpdateJointHolderRoleAsync(
                request.AccountId, request.UserId, request.NewRole, currentUserId);

            if (success)
            {
                return Ok(new { Success = true, Message = "Joint holder updated successfully" });
            }

            return BadRequest(new { Success = false, Message = "Failed to update joint holder" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating joint holder for account {AccountId}", request.AccountId);
            return StatusCode(500, new { Success = false, Message = "An error occurred while updating joint holder" });
        }
    }

    /// <summary>
    /// Get all joint holders for an account
    /// </summary>
    [HttpGet("{accountId}/holders")]
    public async Task<ActionResult<List<JointAccountHolderDto>>> GetJointHolders(Guid accountId)
    {
        try
        {
            var currentUserId = this.GetCurrentUserId();
            
            // Verify current user has access to the account
            var hasAccess = await _accountService.CanUserAccessAccountAsync(accountId, currentUserId);
            if (!hasAccess)
            {
                return Forbid(ErrorMessages.YouDontHaveAccessToThisAccount);
            }

            var jointHolders = await _jointAccountService.GetJointHoldersAsync(accountId);
            var holderDtos = new List<JointAccountHolderDto>();

            foreach (var holder in jointHolders)
            {
                var user = await _userRepository.GetByIdAsync(holder.UserId);
                var addedByUser = await _userRepository.GetByIdAsync(holder.AddedByUserId);
                var removedByUser = holder.RemovedByUserId.HasValue 
                    ? await _userRepository.GetByIdAsync(holder.RemovedByUserId.Value) 
                    : null;

                holderDtos.Add(new JointAccountHolderDto
                {
                    Id = holder.Id,
                    AccountId = holder.AccountId,
                    UserId = holder.UserId,
                    UserName = user?.FullName ?? "Unknown",
                    UserEmail = user?.Email ?? "Unknown",
                    Role = holder.Role,
                    AddedDate = holder.AddedDate,
                    RemovedDate = holder.RemovedDate,
                    IsActive = holder.IsActive,
                    RequiresSignature = holder.RequiresSignature,
                    TransactionLimit = holder.TransactionLimit,
                    DailyLimit = holder.DailyLimit,
                    Notes = holder.Notes,
                    AddedByUserName = addedByUser?.FullName ?? "Unknown",
                    RemovedByUserName = removedByUser?.FullName
                });
            }

            return Ok(holderDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving joint holders for account {AccountId}", accountId);
            return StatusCode(500, "An error occurred while retrieving joint holders");
        }
    }

    /// <summary>
    /// Check if user can perform a transaction on the account
    /// </summary>
    [HttpPost("check-transaction-permission")]
    public async Task<ActionResult<TransactionPermissionResult>> CheckTransactionPermission([FromBody] TransactionPermissionRequest request)
    {
        try
        {
            var currentUserId = this.GetCurrentUserId();
            
            // Only allow checking permissions for current user or if user is admin
            if (request.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid("You can only check your own transaction permissions");
            }

            var canPerform = await _jointAccountService.CanUserPerformTransactionAsync(
                request.AccountId, request.UserId, request.Amount);

            var requiresMultiple = await _jointAccountService.RequiresMultipleSignaturesAsync(
                request.AccountId, request.Amount);

            var account = await _accountService.GetAccountByIdAsync(request.AccountId);
            
            var result = new TransactionPermissionResult
            {
                CanPerformTransaction = canPerform,
                RequiresMultipleSignatures = requiresMultiple,
                SignaturesRequired = account?.MinimumSignaturesRequired ?? 1
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking transaction permission for account {AccountId}", request.AccountId);
            return StatusCode(500, "An error occurred while checking transaction permission");
        }
    }

    /// <summary>
    /// Get all accounts for current user (including joint accounts)
    /// </summary>
    [HttpGet("my-accounts")]
    public async Task<ActionResult<List<AccountDto>>> GetMyAccounts()
    {
        try
        {
            var currentUserId = this.GetCurrentUserId();
            var accounts = await _jointAccountService.GetAccountsForUserAsync(currentUserId);
            
            var accountDtos = accounts.Select(a => new AccountDto
            {
                Id = a.Id,
                AccountNumber = a.AccountNumber,
                AccountHolderName = a.AccountHolderName,
                Balance = a.Balance,
                Status = a.Status,
                Type = a.Type,
                IsJointAccount = a.IsJointAccount,
                RequiresMultipleSignatures = a.RequiresMultipleSignatures,
                MultipleSignatureThreshold = a.MultipleSignatureThreshold
            }).ToList();

            return Ok(accountDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving accounts for user {UserId}", this.GetCurrentUserId());
            return StatusCode(500, "An error occurred while retrieving accounts");
        }
    }

    /// <summary>
    /// Convert an account to joint account
    /// </summary>
    [HttpPost("convert-to-joint")]
    public async Task<ActionResult<bool>> ConvertToJointAccount([FromBody] ConvertToJointAccountRequest request)
    {
        try
        {
            var currentUserId = this.GetCurrentUserId();
            
            // Verify current user has access to the account
            var hasAccess = await _accountService.CanUserAccessAccountAsync(request.AccountId, currentUserId);
            if (!hasAccess)
            {
                return Forbid(ErrorMessages.YouDontHaveAccessToThisAccount);
            }

            var success = await _jointAccountService.ConvertToJointAccountAsync(request.AccountId, currentUserId);

            if (success && (request.RequiresMultipleSignatures || request.MultipleSignatureThreshold.HasValue))
            {
                // Update additional joint account settings
                var account = await _accountService.GetAccountByIdAsync(request.AccountId);
                if (account != null)
                {
                    account.RequiresMultipleSignatures = request.RequiresMultipleSignatures;
                    account.MultipleSignatureThreshold = request.MultipleSignatureThreshold;
                    account.MinimumSignaturesRequired = request.MinimumSignaturesRequired;
                    
                    await _accountService.UpdateAccountAsync(account);
                }
            }

            if (success)
            {
                return Ok(new { Success = true, Message = "Account converted to joint account successfully" });
            }

            return BadRequest(new { Success = false, Message = "Failed to convert account to joint account" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting account {AccountId} to joint account", request.AccountId);
            return StatusCode(500, new { Success = false, Message = "An error occurred while converting account" });
        }
    }

    /// <summary>
    /// Convert joint account to single account
    /// </summary>
    [HttpPost("convert-to-single")]
    public async Task<ActionResult<bool>> ConvertToSingleAccount([FromBody] ConvertToSingleAccountRequest request)
    {
        try
        {
            var currentUserId = this.GetCurrentUserId();
            
            // Verify current user has access to the account
            var hasAccess = await _accountService.CanUserAccessAccountAsync(request.AccountId, currentUserId);
            if (!hasAccess)
            {
                return Forbid(ErrorMessages.YouDontHaveAccessToThisAccount);
            }

            var success = await _jointAccountService.ConvertToSingleAccountAsync(
                request.AccountId, request.RemainingHolderId, currentUserId);

            if (success)
            {
                return Ok(new { Success = true, Message = "Account converted to single account successfully" });
            }

            return BadRequest(new { Success = false, Message = "Failed to convert account to single account" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting account {AccountId} to single account", request.AccountId);
            return StatusCode(500, new { Success = false, Message = "An error occurred while converting account" });
        }
    }

    /// <summary>
    /// Get joint account summary with all holders and settings
    /// </summary>
    [HttpGet("{accountId}/summary")]
    public async Task<ActionResult<JointAccountSummary>> GetJointAccountSummary(Guid accountId)
    {
        try
        {
            var currentUserId = this.GetCurrentUserId();
            
            // Verify current user has access to the account
            var hasAccess = await _accountService.CanUserAccessAccountAsync(accountId, currentUserId);
            if (!hasAccess)
            {
                return Forbid(ErrorMessages.YouDontHaveAccessToThisAccount);
            }

            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null)
            {
                return NotFound(ErrorMessages.AccountNotFound);
            }

            var jointHolders = await _jointAccountService.GetJointHoldersAsync(accountId);
            var holderDtos = new List<JointAccountHolderDto>();

            foreach (var holder in jointHolders)
            {
                var user = await _userRepository.GetByIdAsync(holder.UserId);
                var addedByUser = await _userRepository.GetByIdAsync(holder.AddedByUserId);

                holderDtos.Add(new JointAccountHolderDto
                {
                    Id = holder.Id,
                    AccountId = holder.AccountId,
                    UserId = holder.UserId,
                    UserName = user?.FullName ?? "Unknown",
                    UserEmail = user?.Email ?? "Unknown",
                    Role = holder.Role,
                    AddedDate = holder.AddedDate,
                    IsActive = holder.IsActive,
                    RequiresSignature = holder.RequiresSignature,
                    TransactionLimit = holder.TransactionLimit,
                    DailyLimit = holder.DailyLimit,
                    AddedByUserName = addedByUser?.FullName ?? "Unknown"
                });
            }

            var summary = new JointAccountSummary
            {
                AccountId = account.Id,
                AccountNumber = account.AccountNumber,
                IsJointAccount = account.IsJointAccount,
                RequiresMultipleSignatures = account.RequiresMultipleSignatures,
                MultipleSignatureThreshold = account.MultipleSignatureThreshold,
                MinimumSignaturesRequired = account.MinimumSignaturesRequired,
                ActiveJointHoldersCount = account.GetActiveJointHoldersCount(),
                JointHolders = holderDtos
            };

            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving joint account summary for account {AccountId}", accountId);
            return this.CreateErrorResponse("An error occurred while retrieving joint account summary");
        }
    }
}