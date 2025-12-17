namespace DublinBikesApi.Services;

/// <summary>
/// Interface for caching operations
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a value from cache
    /// </summary>
    T? Get<T>(string key);

    /// <summary>
    /// Sets a value in cache with expiration
    /// </summary>
    void Set<T>(string key, T value, TimeSpan expiration);

    /// <summary>
    /// Removes a value from cache
    /// </summary>
    void Remove(string key);

    /// <summary>
    /// Clears all cache entries
    /// </summary>
    void Clear();
}
