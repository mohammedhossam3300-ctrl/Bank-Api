namespace Bank.Api.Configuration;

/// <summary>
/// CORS configuration settings with security-first approach
/// Uses whitelist-based origin validation (NOT wildcard/AllowAnyOrigin)
/// </summary>
public class CorsSettings
{
    /// <summary>
    /// Allowed origins for CORS requests (WHITELIST APPROACH)
    /// 
    /// Security: Only explicitly listed domains will be allowed.
    /// NEVER use "*" (wildcard). Always specify exact trusted origins.
    /// 
    /// Examples:
    ///   - "https://yourdomain.com"
    ///   - "https://app.yourdomain.com"
    ///   - "http://localhost:3000" (for development only)
    /// 
    /// Environment-specific configuration recommended:
    ///   - Production: ["https://yourdomain.com"]
    ///   - Staging: ["https://staging.yourdomain.com"]
    ///   - Development: ["http://localhost:3000", "http://localhost:4200"]
    /// </summary>
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Allowed HTTP methods for CORS requests
    /// Default: GET, POST, PUT, DELETE, PATCH, OPTIONS
    /// </summary>
    public string[] AllowedMethods { get; set; } = new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS" };

    /// <summary>
    /// Allowed headers in CORS requests
    /// Default: Content-Type, Authorization, X-Requested-With
    /// </summary>
    public string[] AllowedHeaders { get; set; } = new[] { "Content-Type", "Authorization", "X-Requested-With" };

    /// <summary>
    /// Whether credentials (cookies, authorization headers) are allowed in CORS requests
    /// Default: true (required for authentication)
    /// WARNING: When AllowCredentials is true, AllowedOrigins MUST NOT be "*"
    /// </summary>
    public bool AllowCredentials { get; set; } = true;

    /// <summary>
    /// Max age for preflight (OPTIONS) requests in seconds
    /// Default: 3600 (1 hour)
    /// Higher values reduce preflight requests but lower values are safer for policy updates
    /// </summary>
    public int MaxAge { get; set; } = 3600;
}
