using Bank.Api.Helpers;
using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bank.Api.Controllers.Auth;

/// <summary>
/// Controller for managing user sessions
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SessionController : ControllerBase
{
    private readonly ISessionService _sessionService;
    private readonly ILogger<SessionController> _logger;

    public SessionController(ISessionService sessionService, ILogger<SessionController> logger)
    {
        _sessionService = sessionService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all active sessions for the current user
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<List<SessionInfo>>> GetActiveSessions()
    {
        try
        {
            var userId = GetCurrentUserId();
            var sessions = await _sessionService.GetUserActiveSessionsAsync(userId);

            var sessionInfos = sessions.Select(s => new SessionInfo
            {
                Id = s.Id,
                SessionToken = s.SessionToken[..8] + "...", // Mask token for security
                ExpiresAt = s.ExpiresAt,
                Status = s.Status,
                IpAddress = s.IpAddress,
                UserAgent = s.UserAgent,
                LastActivityAt = s.LastActivityAt,
                IsAdminSession = s.IsAdminSession,
                CreatedAt = s.CreatedAt
            }).ToList();

            return Ok(sessionInfos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active sessions for user");
            return StatusCode(500, "Failed to retrieve active sessions");
        }
    }

    /// <summary>
    /// Terminates a specific session
    /// </summary>
    [HttpDelete("{sessionId}")]
    public async Task<ActionResult> TerminateSession(Guid sessionId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var sessions = await _sessionService.GetUserActiveSessionsAsync(userId);
            var session = sessions.FirstOrDefault(s => s.Id == sessionId);

            if (session == null)
            {
                return NotFound("Session not found");
            }

            await _sessionService.TerminateSessionAsync(session.SessionToken, "User requested termination");
            return Ok(new { message = "Session terminated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error terminating session {SessionId}", sessionId);
            return StatusCode(500, "Failed to terminate session");
        }
    }

    /// <summary>
    /// Terminates all sessions except the current one
    /// </summary>
    [HttpDelete("terminate-all")]
    public async Task<ActionResult> TerminateAllOtherSessions()
    {
        try
        {
            var userId = GetCurrentUserId();
            var currentSessionToken = GetCurrentSessionToken();

            await _sessionService.TerminateAllUserSessionsAsync(userId, "User requested termination of all other sessions", currentSessionToken);
            return Ok(new { message = "All other sessions terminated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error terminating all sessions for user");
            return StatusCode(500, "Failed to terminate sessions");
        }
    }

    /// <summary>
    /// Gets session statistics (admin only)
    /// </summary>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SessionStatistics>> GetSessionStatistics()
    {
        try
        {
            var statistics = await _sessionService.GetSessionStatisticsAsync();
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving session statistics");
            return StatusCode(500, "Failed to retrieve session statistics");
        }
    }

    /// <summary>
    /// Refreshes the current session
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<SessionResult>> RefreshSession([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest("Refresh token is required");
            }

            var result = await _sessionService.RefreshSessionAsync(request.RefreshToken);
            
            if (!result.Success)
            {
                return Unauthorized(result.ErrorMessage);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing session");
            return StatusCode(500, "Failed to refresh session");
        }
    }

    /// <summary>
    /// Updates session activity (called automatically by middleware)
    /// </summary>
    [HttpPost("activity")]
    public async Task<ActionResult> UpdateActivity()
    {
        try
        {
            var sessionToken = GetCurrentSessionToken();
            if (!string.IsNullOrEmpty(sessionToken))
            {
                await _sessionService.UpdateSessionActivityAsync(sessionToken);
            }
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating session activity");
            return StatusCode(500, "Failed to update session activity");
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value 
            ?? User.FindFirst("id")?.Value;

        return userIdClaim != null && Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    private string? GetCurrentSessionToken()
    {
        var sessionTokenClaim = User.FindFirst("session_token")?.Value;
        if (!string.IsNullOrEmpty(sessionTokenClaim))
        {
            return sessionTokenClaim;
        }

        // Extract Bearer token from Authorization header if present
        var authHeader = Request.Headers.Authorization.ToString();
        if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authHeader.Substring("Bearer ".Length);
        }

        return null;
    }
}

