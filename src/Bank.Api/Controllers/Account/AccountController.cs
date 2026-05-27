using Bank.Application.Interfaces;
using Bank.Application.DTOs;
using Bank.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bank.Api.Controllers.Account;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    /// <summary>
    /// GET /api/account — List all accounts for the logged-in user.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyAccounts()
    {
        var userId = this.GetCurrentUserIdRequired();
        var accounts = await _accountService.GetUserAccountsAsync(userId);
        return this.CreateSuccessResponse("Accounts retrieved successfully", accounts);
    }

    /// <summary>
    /// GET /api/account/{id} — Get a specific account by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAccountById(Guid id)
    {
        var account = await _accountService.GetAccountByIdAsync(id);
        if (account == null) return this.CreateNotFoundResponse("Account not found");
        return this.CreateSuccessResponse("Account retrieved successfully", account);
    }

    /// <summary>
    /// POST /api/account — Create a new account for the logged-in user.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        var userId = this.GetCurrentUserIdRequired();
        var account = await _accountService.CreateAccountAsync(userId, request.AccountNickname);
        return CreatedAtAction(nameof(GetAccountById), new { id = account.Id }, 
            new { Success = true, Message = "Account created successfully", Data = account });
    }

    /// <summary>
    /// PUT /api/account/{id} — Update an account's holder name.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] UpdateAccountRequest request)
    {
        var account = await _accountService.GetAccountByIdAsync(id);
        if (account == null) return this.CreateNotFoundResponse("Account not found");

        // Verify ownership
        var userId = this.GetCurrentUserIdRequired();
        if (account.UserId != userId) return this.CreateForbiddenResponse("Access denied");

        account.AccountHolderName = request.AccountHolderName;
        // In a real app, persist via service  
        return this.CreateSuccessResponse("Account updated successfully", account);
    }

    /// <summary>
    /// DELETE /api/account/{id} — Delete an account.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAccount(Guid id)
    {
        var account = await _accountService.GetAccountByIdAsync(id);
        if (account == null) return this.CreateNotFoundResponse("Account not found");

        var userId = this.GetCurrentUserIdRequired();
        if (account.UserId != userId) return this.CreateForbiddenResponse("Access denied");

        // In a real app, delete via service
        return NoContent();
    }
}
