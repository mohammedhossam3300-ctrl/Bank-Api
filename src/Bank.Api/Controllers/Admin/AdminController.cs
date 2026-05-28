using Bank.Application.Interfaces;
using Bank.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers.Admin;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IAccountService _accountService;
    private readonly IUserRepository _userRepository;

    public AdminController(IAuthService authService, IAccountService accountService, IUserRepository userRepository)
    {
        _authService = authService;
        _accountService = accountService;
        _userRepository = userRepository;
    }

    /// <summary>
    /// GET /api/admin/users — List all users (Admin only).
    /// </summary>
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _authService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// GET /api/admin/users/{id} — Get a user by ID (Admin only).
    /// </summary>
    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var users = await _authService.GetAllUsersAsync();
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    /// <summary>
    /// DELETE /api/admin/users/{id} — Soft-delete a user (Admin only).
    /// </summary>
    [HttpDelete("users/{id}")]
    public async Task<IActionResult> SuspendUser(Guid id)
    {
        var users = await _authService.GetAllUsersAsync();
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user == null) return NotFound();

        user.SoftDelete("Admin");
        await _userRepository.UpdateAsync(user);
        return Ok(new { Message = "User suspended successfully." });
    }

    /// <summary>
    /// GET /api/admin/accounts — List all accounts (Admin only).
    /// </summary>
    [HttpGet("accounts")]
    public async Task<IActionResult> GetAllAccounts()
    {
        var accounts = await _accountService.GetAllAccountsAsync();
        return Ok(accounts);
    }

    /// <summary>
    /// GET /api/admin/accounts/{id} — Get a specific account (Admin only).
    /// </summary>
    [HttpGet("accounts/{id}")]
    public async Task<IActionResult> GetAccountById(Guid id)
    {
        var account = await _accountService.GetAccountByIdAsync(id);
        if (account == null) return NotFound();
        return Ok(account);
    }
}
