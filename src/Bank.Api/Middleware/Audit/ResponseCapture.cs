using System.Text;
using System.Text.Json;

namespace Bank.Api.Middleware.Audit;

/// <summary>
/// Handles capturing and sanitizing HTTP response details for audit logging
/// </summary>
public class ResponseCapture
{
    public ResponseCapture()
    {
    }

    public async Task<string> CaptureResponseDetailsAsync(HttpResponse response, MemoryStream responseBodyStream)
    {
        var responseBody = string.Empty;
        
        if (responseBodyStream.Length > 0 && responseBodyStream.Length <= 10240) // Skip large responses > 10KB
        {
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[responseBodyStream.Length];
            await responseBodyStream.ReadAsync(buffer, 0, buffer.Length);
            
            if (ContentSanitizer.IsLoggableContentType(response.ContentType))
            {
                responseBody = Encoding.UTF8.GetString(buffer);
                responseBody = ContentSanitizer.SanitizeSensitiveData(responseBody);
            }
            else
            {
                responseBody = "[Binary Content]";
            }
        }

        var responseDetails = new
        {
            StatusCode = response.StatusCode,
            Headers = response.Headers.Where(h => !ContentSanitizer.IsSecuritySensitiveHeader(h.Key))
                .ToDictionary(h => h.Key, h => h.Value.ToString()),
            ContentType = response.ContentType,
            ContentLength = responseBodyStream.Length,
            Body = responseBody
        };

        return JsonSerializer.Serialize(responseDetails, new JsonSerializerOptions 
        { 
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}
