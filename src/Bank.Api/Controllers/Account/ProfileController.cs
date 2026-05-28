using Bank.Application.Interfaces;
using Bank.Application.DTOs;
using Bank.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bank.Api.Controllers.Account;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;

    public ProfileController(IAuthService authService, IUserRepository userRepository)
    {
        _authService = authService;
        _userRepository = userRepository;
    }

    /// <summary>
    /// GET /api/profile — Get current user's profile.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        var user = await _authService.GetUserByEmailAsync(email);
        if (user == null) return NotFound();

        return Ok(new ProfileResponse(user.Id, user.UserName!, user.Email!, user.FirstName, user.LastName));
    }

    /// <summary>
    /// PUT /api/profile — Update current user's profile.
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        var user = await _authService.GetUserByEmailAsync(email);
        if (user == null) return NotFound();

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        return Ok(new ProfileResponse(user.Id, user.UserName!, user.Email!, user.FirstName, user.LastName));
    }

    /// <summary>
    /// DELETE /api/profile — Soft-delete (deactivate) current user's account.
    /// </summary>
    [HttpDelete]
    public async Task<IActionResult> DeactivateAccount()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        var user = await _authService.GetUserByEmailAsync(email);
        if (user == null) return NotFound();

        user.SoftDelete(user.UserName);
        await _userRepository.UpdateAsync(user);
        return Ok(new { Message = "Account deactivated successfully." });
    }
}
