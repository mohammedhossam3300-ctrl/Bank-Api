using System.Text.Json;
using Bank.Application.Interfaces;
using Bank.Application.DTOs.Shared.RateLimit;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Bank.Infrastructure.Services;

/// <summary>
/// Redis-based rate limiting service implementation
/// </summary>
public class RateLimitingService : IRateLimitingService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RateLimitingService> _logger;
    
    private const string KeyPrefix = "rate_limit:";

    public RateLimitingService(IDistributedCache cache, ILogger<RateLimitingService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<RateLimitResult> IsRequestAllowedAsync(string key, RateLimitPolicy policy)
    {
        try
        {
            var cacheKey = $"{KeyPrefix}{key}";
            var rateLimitData = await GetRateLimitDataAsync(cacheKey);

            var now = DateTime.UtcNow;
            
            // Check if window has expired
            if (rateLimitData == null || now >= rateLimitData.WindowEnd)
            {
                // Start new window
                rateLimitData = new RateLimitData
                {
                    RequestCount = 1,
                    WindowStart = now,
                    WindowEnd = now.Add(policy.WindowDuration)
                };
                
                await SaveRateLimitDataAsync(cacheKey, rateLimitData, policy.WindowDuration);
                
                return new RateLimitResult
                {
                    IsAllowed = true,
                    RequestsRemaining = policy.RequestLimit - 1,
                    ResetTime = policy.WindowDuration
                };
            }

            // Check if limit exceeded
            if (rateLimitData.RequestCount >= policy.RequestLimit)
            {
                var resetTime = rateLimitData.WindowEnd - now;
                
                _logger.LogWarning("Rate limit exceeded for key {Key}. Requests: {Count}/{Limit}", 
                    key, rateLimitData.RequestCount, policy.RequestLimit);
                
                return new RateLimitResult
                {
                    IsAllowed = false,
                    RequestsRemaining = 0,
                    ResetTime = resetTime,
                    Message = $"Rate limit exceeded. Try again in {resetTime.TotalSeconds:F0} seconds."
                };
            }

            // Increment counter
            rateLimitData.RequestCount++;
            await SaveRateLimitDataAsync(cacheKey, rateLimitData, rateLimitData.WindowEnd - now);

            return new RateLimitResult
            {
                IsAllowed = true,
                RequestsRemaining = policy.RequestLimit - rateLimitData.RequestCount,
                ResetTime = rateLimitData.WindowEnd - now
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking rate limit for key {Key}", key);
            // On error, allow the request to avoid blocking legitimate users
            return new RateLimitResult { IsAllowed = true, RequestsRemaining = 0, ResetTime = TimeSpan.Zero };
        }
    }

    public async Task<RateLimitResult> IsUserRequestAllowedAsync(Guid userId, string action, RateLimitPolicy? customPolicy = null)
    {
        var key = $"user:{userId}:{action}";
        var policy = customPolicy ?? GetDefaultPolicyForAction(action);
        return await IsRequestAllowedAsync(key, policy);
    }

    public async Task<RateLimitResult> IsIpRequestAllowedAsync(string ipAddress, string action, RateLimitPolicy? customPolicy = null)
    {
        var key = $"ip:{ipAddress}:{action}";
        var policy = customPolicy ?? GetDefaultPolicyForAction(action);
        return await IsRequestAllowedAsync(key, policy);
    }

    public async Task ResetRateLimitAsync(string key)
    {
        try
        {
            var cacheKey = $"{KeyPrefix}{key}";
            await _cache.RemoveAsync(cacheKey);
            _logger.LogInformation("Rate limit reset for key {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting rate limit for key {Key}", key);
        }
    }

    public async Task<RateLimitStatus> GetRateLimitStatusAsync(string key)
    {
        try
        {
            var cacheKey = $"{KeyPrefix}{key}";
            var rateLimitData = await GetRateLimitDataAsync(cacheKey);

            if (rateLimitData == null)
            {
                return new RateLimitStatus
                {
                    RequestCount = 0,
                    RequestLimit = 0,
                    WindowDuration = TimeSpan.Zero,
                    WindowStart = DateTime.UtcNow,
                    WindowEnd = DateTime.UtcNow
                };
            }

            return new RateLimitStatus
            {
                RequestCount = rateLimitData.RequestCount,
                RequestLimit = 100, // Default - in production, store this with the data
                WindowDuration = rateLimitData.WindowEnd - rateLimitData.WindowStart,
                WindowStart = rateLimitData.WindowStart,
                WindowEnd = rateLimitData.WindowEnd
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting rate limit status for key {Key}", key);
            return new RateLimitStatus();
        }
    }

    private async Task<RateLimitData?> GetRateLimitDataAsync(string cacheKey)
    {
        var data = await _cache.GetStringAsync(cacheKey);
        if (string.IsNullOrEmpty(data))
            return null;

        return JsonSerializer.Deserialize<RateLimitData>(data);
    }

    private async Task SaveRateLimitDataAsync(string cacheKey, RateLimitData data, TimeSpan expiry)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry
        };

        var serializedData = JsonSerializer.Serialize(data);
        await _cache.SetStringAsync(cacheKey, serializedData, options);
    }

    private static RateLimitPolicy GetDefaultPolicyForAction(string action)
    {
        return action.ToLowerInvariant() switch
        {
            "login" => RateLimitPolicy.Login,
            "transaction" => RateLimitPolicy.Transaction,
            "2fa" => RateLimitPolicy.TwoFactor,
            _ => RateLimitPolicy.Default
        };
    }

    private class RateLimitData
    {
        public int RequestCount { get; set; }
        public DateTime WindowStart { get; set; }
        public DateTime WindowEnd { get; set; }
    }
}