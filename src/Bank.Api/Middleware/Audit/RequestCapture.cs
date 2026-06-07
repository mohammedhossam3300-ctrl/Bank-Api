using System.Text;
using System.Text.Json;

namespace Bank.Api.Middleware.Audit;

/// <summary>
/// Handles capturing and sanitizing HTTP request details for audit logging
/// </summary>
public class RequestCapture
{
    public RequestCapture()
    {
    }

    public async Task<string> CaptureRequestDetailsAsync(HttpRequest request)
    {
        var requestDetails = new
        {
            Method = request.Method,
            Path = request.Path.Value,
            QueryString = request.QueryString.Value,
            Headers = request.Headers.Where(h => !ContentSanitizer.IsSecuritySensitiveHeader(h.Key))
                .ToDictionary(h => h.Key, h => h.Value.ToString()),
            ContentType = request.ContentType,
            ContentLength = request.ContentLength,
            Body = await GetRequestBodyAsync(request)
        };

        return JsonSerializer.Serialize(requestDetails, new JsonSerializerOptions 
        { 
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    private async Task<string> GetRequestBodyAsync(HttpRequest request)
    {
        if (request.ContentLength == 0 || request.ContentLength > 10240) // Skip large bodies > 10KB
            return string.Empty;

        if (!ContentSanitizer.IsLoggableContentType(request.ContentType))
            return "[Binary Content]";

        request.EnableBuffering();
        var buffer = new byte[Convert.ToInt32(request.ContentLength ?? 0)];
        await request.Body.ReadAsync(buffer, 0, buffer.Length);
        request.Body.Position = 0;

        var body = Encoding.UTF8.GetString(buffer);
        
        // Sanitize sensitive data
        return ContentSanitizer.SanitizeSensitiveData(body);
    }
}
