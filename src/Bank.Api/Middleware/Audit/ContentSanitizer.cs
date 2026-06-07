using System.Text.RegularExpressions;

namespace Bank.Api.Middleware.Audit;

/// <summary>
/// Handles sanitization of sensitive data in request/response content
/// </summary>
public class ContentSanitizer
{
    private static readonly string[] SensitiveHeaders = new[]
    {
        "Authorization",
        "Cookie",
        "Set-Cookie",
        "X-API-Key",
        "X-Auth-Token"
    };

    private static readonly string[] LoggableContentTypes = new[]
    {
        "application/json",
        "application/xml",
        "text/plain",
        "text/xml",
        "application/x-www-form-urlencoded"
    };

    private static readonly string[] SensitivePatterns = new[]
    {
        @"""password""\s*:\s*""[^""]*""",
        @"""Password""\s*:\s*""[^""]*""",
        @"""pin""\s*:\s*""[^""]*""",
        @"""Pin""\s*:\s*""[^""]*""",
        @"""ssn""\s*:\s*""[^""]*""",
        @"""socialSecurityNumber""\s*:\s*""[^""]*""",
        @"""creditCardNumber""\s*:\s*""[^""]*""",
        @"""accountNumber""\s*:\s*""[^""]*"""
    };

    public bool IsSecuritySensitiveHeader(string headerName)
    {
        return SensitiveHeaders.Contains(headerName, StringComparer.OrdinalIgnoreCase);
    }

    public bool IsLoggableContentType(string? contentType)
    {
        if (string.IsNullOrEmpty(contentType))
            return false;

        return LoggableContentTypes.Any(type => 
            contentType.StartsWith(type, StringComparison.OrdinalIgnoreCase));
    }

    public string SanitizeSensitiveData(string content)
    {
        if (string.IsNullOrEmpty(content))
            return content;

        // Set a timeout of 1 second to prevent ReDoS attacks on malicious regex patterns
        var regexTimeout = TimeSpan.FromSeconds(1);

        foreach (var pattern in SensitivePatterns)
        {
            content = Regex.Replace(
                content, 
                pattern, 
                match => match.Value.Substring(0, match.Value.IndexOf(':') + 1) + " \"[REDACTED]\"",
                RegexOptions.IgnoreCase,
                regexTimeout);
        }

        return content;
    }
}
