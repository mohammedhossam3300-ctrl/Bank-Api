using System.Net;
using System.Text.Json;

namespace Bank.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        string message;
        int statusCode;

        switch (exception)
        {
            case UnauthorizedAccessException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = "Unauthorized access.";
                break;
            case InvalidOperationException:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = "Invalid operation.";
                break;
            case KeyNotFoundException:
                statusCode = (int)HttpStatusCode.NotFound;
                message = "Resource not found.";
                break;
            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = "An unexpected error occurred. Please try again later.";
                break;
        }

        context.Response.StatusCode = statusCode;

        // Never expose internal exception details, stack traces, or message text to the client.
        // All details are logged server-side by the catch block above.
        var result = JsonSerializer.Serialize(new { Message = message });
        return context.Response.WriteAsync(result);
    }
}
