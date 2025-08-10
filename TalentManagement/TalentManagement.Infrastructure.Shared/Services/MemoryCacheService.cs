using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.RegularExpressions;
using TalentManagement.Application.Interfaces;

namespace TalentManagement.Infrastructure.Shared.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<MemoryCacheService> _logger;
        private readonly ConcurrentDictionary<string, object> _keyTracker = new();
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(30);

        public MemoryCacheService(IMemoryCache memoryCache, ILogger<MemoryCacheService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                if (_memoryCache.TryGetValue(key, out var cachedValue))
                {
                    if (cachedValue is string json)
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        };
                        
                        var result = JsonSerializer.Deserialize<T>(json, options);
                        _logger.LogDebug("Cache hit for key: {Key}", key);
                        return Task.FromResult(result);
                    }
                    
                    if (cachedValue is T directValue)
                    {
                        _logger.LogDebug("Cache hit for key: {Key}", key);
                        return Task.FromResult<T?>(directValue);
                    }
                }

                _logger.LogDebug("Cache miss for key: {Key}", key);
                return Task.FromResult<T?>(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cached value for key: {Key}", key);
                return Task.FromResult<T?>(null);
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var expirationTime = expiration ?? _defaultExpiration;
                
                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expirationTime,
                    SlidingExpiration = TimeSpan.FromMinutes(5), // Refresh if accessed within 5 minutes of expiration
                    Priority = CacheItemPriority.Normal
                };

                options.RegisterPostEvictionCallback((evictedKey, evictedValue, reason, state) =>
                {
                    _keyTracker.TryRemove(evictedKey.ToString() ?? string.Empty, out _);
                    _logger.LogDebug("Cache entry evicted for key: {Key}, reason: {Reason}", evictedKey, reason);
                });

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var serializedValue = JsonSerializer.Serialize(value, jsonOptions);
                _memoryCache.Set(key, serializedValue, options);
                _keyTracker.TryAdd(key, value);

                _logger.LogDebug("Cached value for key: {Key} with expiration: {Expiration}", key, expirationTime);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error caching value for key: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                _memoryCache.Remove(key);
                _keyTracker.TryRemove(key, out _);
                _logger.LogDebug("Removed cached value for key: {Key}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cached value for key: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemovePatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            try
            {
                var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                var keysToRemove = _keyTracker.Keys.Where(key => regex.IsMatch(key)).ToList();

                foreach (var key in keysToRemove)
                {
                    _memoryCache.Remove(key);
                    _keyTracker.TryRemove(key, out _);
                }

                _logger.LogDebug("Removed {Count} cached values matching pattern: {Pattern}", keysToRemove.Count, pattern);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cached values for pattern: {Pattern}", pattern);
                return Task.CompletedTask;
            }
        }

        public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var exists = _memoryCache.TryGetValue(key, out _);
                return Task.FromResult(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if cached value exists for key: {Key}", key);
                return Task.FromResult(false);
            }
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getItem, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var cachedValue = await GetAsync<T>(key, cancellationToken);
                if (cachedValue != null)
                {
                    return cachedValue;
                }

                _logger.LogDebug("Cache miss for key: {Key}, fetching from source", key);
                var value = await getItem();
                
                if (value != null)
                {
                    await SetAsync(key, value, expiration, cancellationToken);
                }

                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrSetAsync for key: {Key}", key);
                
                // Fallback to getting the item directly if caching fails
                try
                {
                    return await getItem();
                }
                catch (Exception innerEx)
                {
                    _logger.LogError(innerEx, "Error in fallback getItem for key: {Key}", key);
                    throw;
                }
            }
        }
    }
}