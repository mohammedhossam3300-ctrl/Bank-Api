using Bank.Application.Interfaces;
using Bank.Application.DTOs;
using Bank.Api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = await _authService.LoginAsync(request.Email, request.Password);
        return this.CreateSuccessResponse("Login successful", new AuthResponse(token));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = await _authService.RegisterAsync(request.Username, request.Email, request.Password);
        return this.CreateSuccessResponse("Registration successful", new { user.Id, user.UserName, user.Email });
    }
}
