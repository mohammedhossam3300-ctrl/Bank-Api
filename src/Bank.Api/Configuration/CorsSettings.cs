namespace Bank.Api.Configuration;

/// <summary>
/// CORS configuration settings
/// </summary>
public class CorsSettings
{
    /// <summary>
    /// Allowed origins for CORS requests
    /// </summary>
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Allowed HTTP methods
    /// </summary>
    public string[] AllowedMethods { get; set; } = new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS" };

    /// <summary>
    /// Allowed headers in requests
    /// </summary>
    public string[] AllowedHeaders { get; set; } = new[] { "Content-Type", "Authorization", "X-Requested-With" };

    /// <summary>
    /// Whether credentials (cookies, headers) are allowed
    /// </summary>
    public bool AllowCredentials { get; set; } = true;

    /// <summary>
    /// Max age for preflight requests in seconds
    /// </summary>
    public int MaxAge { get; set; } = 3600;
}
