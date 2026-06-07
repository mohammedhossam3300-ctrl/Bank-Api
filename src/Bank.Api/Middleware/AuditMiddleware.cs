using Bank.Api.Middleware.Audit;
using System.Diagnostics;
using System.Security.Claims;

namespace Bank.Api.Middleware;

/// <summary>
/// Middleware for automatic audit logging of HTTP requests and responses.
/// Captures user actions, IP addresses, and request/response data for compliance.
/// Delegates to specialized classes for each responsibility.
/// </summary>
public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditMiddleware> _logger;
    private readonly AuditPathFilter _pathFilter;
    private readonly ContentSanitizer _sanitizer;
    private readonly RequestCapture _requestCapture;
    private readonly ResponseCapture _responseCapture;
    private readonly AuditLogger _auditLogger;

    public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger, ILogger<AuditLogger> auditLoggerLogger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        // Initialize specialized components
        _pathFilter = new AuditPathFilter();
        _sanitizer = new ContentSanitizer();
        _requestCapture = new RequestCapture();
        _responseCapture = new ResponseCapture();
        _auditLogger = new AuditLogger(auditLoggerLogger);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip audit logging for excluded paths
        if (_pathFilter.ShouldSkipAuditLogging(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var requestId = Guid.NewGuid().ToString();
        context.Items["RequestId"] = requestId;

        var stopwatch = Stopwatch.StartNew();
        var originalBodyStream = context.Response.Body;

        try
        {
            // Capture request details
            var requestDetails = await _requestCapture.CaptureRequestDetailsAsync(context.Request);
            
            // Capture immutable request-time context before async operations
            var capturedMethod = context.Request.Method;
            var capturedPath = context.Request.Path.ToString();
            var capturedUserAgent = context.Request.Headers["User-Agent"].FirstOrDefault();
            var capturedIpAddress = GetClientIpAddressSafe(context);

            // Create a memory stream to capture response
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            // Execute the request
            await _next(context);

            stopwatch.Stop();

            // Capture response details and context data while context is still valid
            var responseDetails = await _responseCapture.CaptureResponseDetailsAsync(context.Response, responseBodyStream);
            var capturedUserId = GetUserIdSafe(context);
            var capturedSessionId = GetSessionIdSafe(context);
            var capturedStatusCode = context.Response.StatusCode;
            var capturedDuration = stopwatch.ElapsedMilliseconds;

            // Copy response back to original stream
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBodyStream);

            // Log the audit entry asynchronously using only captured (value-type/string) data
            _ = Task.Run(() =>
            {
                try
                {
                    var auditData = new AuditEntryData
                    {
                        UserId = capturedUserId,
                        IpAddress = capturedIpAddress,
                        UserAgent = capturedUserAgent,
                        SessionId = capturedSessionId,
                        Method = capturedMethod,
                        Path = capturedPath,
                        RequestDetails = requestDetails,
                        ResponseDetails = responseDetails,
                        DurationMs = capturedDuration,
                        StatusCode = capturedStatusCode,
                        RequestId = requestId
                    };
                    _auditLogger.LogAuditEntryDirect(auditData);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to log audit entry for request {RequestId}", requestId);
                }
            });
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error in audit middleware for request {RequestId}", requestId);

            // Capture all needed data from context before it's disposed
            var capturedMethod = context.Request.Method;
            var capturedPath = context.Request.Path.ToString();
            var capturedIpAddress = GetClientIpAddressSafe(context);
            var capturedUserAgent = context.Request.Headers["User-Agent"].FirstOrDefault();
            var capturedUserId = GetUserIdSafe(context);
            var capturedSessionId = GetSessionIdSafe(context);
            var errorMessage = ex.Message;

            // Log the error as a security event using only captured data
            _ = Task.Run(() =>
            {
                try
                {
                    var securityData = new SecurityEventData
                    {
                        UserId = capturedUserId,
                        IpAddress = capturedIpAddress,
                        UserAgent = capturedUserAgent,
                        SessionId = capturedSessionId,
                        Method = capturedMethod,
                        Path = capturedPath,
                        Action = "MIDDLEWARE_ERROR",
                        AdditionalData = errorMessage,
                        RequestId = requestId
                    };
                    _auditLogger.LogSecurityEventDirect(securityData);
                }
                catch (Exception logEx)
                {
                    _logger.LogError(logEx, "Failed to log security event for request {RequestId}", requestId);
                }
            });
            
            throw;
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private static Guid? GetUserIdSafe(HttpContext context)
    {
        try
        {
            var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
        catch
        {
            return null;
        }
    }

    private static string? GetClientIpAddressSafe(HttpContext context)
    {
        try
        {
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
                return forwardedFor.Split(',')[0].Trim();

            var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
                return realIp;

            return context.Connection.RemoteIpAddress?.ToString();
        }
        catch
        {
            return null;
        }
    }

    private static string? GetSessionIdSafe(HttpContext context)
    {
        try
        {
            return context.Session?.Id;
        }
        catch
        {
            return null;
        }
    }
}
