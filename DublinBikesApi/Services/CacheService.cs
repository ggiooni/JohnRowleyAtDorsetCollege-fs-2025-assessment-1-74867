using Microsoft.Extensions.Caching.Memory;

namespace DublinBikesApi.Services;

/// <summary>
/// In-memory cache implementation
/// </summary>
public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public T? Get<T>(string key)
    {
        if (_cache.TryGetValue(key, out T? value))
        {
            _logger.LogDebug($"Cache hit for key: {key}");
            return value;
        }

        _logger.LogDebug($"Cache miss for key: {key}");
        return default;
    }

    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        _cache.Set(key, value, options);
        _logger.LogDebug($"Cache set for key: {key}, expires in {expiration.TotalMinutes} minutes");
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
        _logger.LogDebug($"Cache removed for key: {key}");
    }

    public void Clear()
    {
        if (_cache is MemoryCache memoryCache)
        {
            memoryCache.Compact(1.0);
            _logger.LogInformation("Cache cleared");
        }
    }
}
