using Bank.Application.Interfaces;
using System.Security.Claims;
using System.Text.Json;

namespace Bank.Api.Middleware.Audit;

/// <summary>
/// Handles logging of audit entries and security events
/// </summary>
public class AuditLogger
{
    private readonly ILogger<AuditLogger> _logger;

    public AuditLogger(ILogger<AuditLogger> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Logs an audit entry using pre-captured data (safe to call after HttpContext disposal).
    /// </summary>
    public async Task LogAuditEntryDirectAsync(
        Guid? userId,
        string? ipAddress,
        string? userAgent,
        string? sessionId,
        string method,
        string path,
        string requestDetails,
        string responseDetails,
        long durationMs,
        int statusCode,
        string requestId)
    {
        var action = $"{method} {path}";

        _logger.LogInformation(
            "Audit: {Action} | User: {UserId} | IP: {IpAddress} | Status: {StatusCode} | Duration: {DurationMs}ms | RequestId: {RequestId}",
            action, userId?.ToString() ?? "anonymous", ipAddress, statusCode, durationMs, requestId);
    }

    /// <summary>
    /// Logs a security event using pre-captured data (safe to call after HttpContext disposal).
    /// </summary>
    public async Task LogSecurityEventDirectAsync(
        Guid? userId,
        string? ipAddress,
        string? userAgent,
        string? sessionId,
        string method,
        string path,
        string action,
        string additionalData,
        string requestId)
    {
        _logger.LogWarning(
            "SecurityEvent: {Action} | User: {UserId} | IP: {IpAddress} | Path: {Path} | RequestId: {RequestId} | Details: {Details}",
            action, userId?.ToString() ?? "anonymous", ipAddress, path, requestId, additionalData);
    }

    private static Guid? GetUserId(HttpContext context)
    {
        var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    private static string? GetClientIpAddress(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        return context.Connection.RemoteIpAddress?.ToString();
    }
}
