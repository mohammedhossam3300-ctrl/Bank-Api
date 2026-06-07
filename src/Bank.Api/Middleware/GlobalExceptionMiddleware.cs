using FluentValidation;
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

        int statusCode;
        object body;

        switch (exception)
        {
            case ValidationException validationEx:
                statusCode = (int)HttpStatusCode.BadRequest;
                // Validation errors are user-facing by design — safe to return.
                var validationErrors = validationEx.Errors
                    .Select(e => new { e.PropertyName, e.ErrorMessage })
                    .ToList();
                body = new { Message = "One or more validation errors occurred.", Errors = validationErrors };
                break;

            case UnauthorizedAccessException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                body = new { Message = "Unauthorized access." };
                break;

            case InvalidOperationException:
                statusCode = (int)HttpStatusCode.BadRequest;
                body = new { Message = "Invalid operation." };
                break;

            case KeyNotFoundException:
                statusCode = (int)HttpStatusCode.NotFound;
                body = new { Message = "Resource not found." };
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                // Never expose internal exception details, stack traces, or message text to the client.
                // All details are logged server-side by the catch block above.
                body = new { Message = "An unexpected error occurred. Please try again later." };
                break;
        }

        context.Response.StatusCode = statusCode;
        var result = JsonSerializer.Serialize(body);
        return context.Response.WriteAsync(result);
    }
}
