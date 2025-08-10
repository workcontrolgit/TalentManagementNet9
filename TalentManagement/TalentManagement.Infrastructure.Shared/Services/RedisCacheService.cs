using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.RegularExpressions;
using TalentManagement.Application.Interfaces;
using StackExchange.Redis;

namespace TalentManagement.Infrastructure.Shared.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IDatabase _database;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(30);

        public RedisCacheService(
            IDistributedCache distributedCache, 
            IConnectionMultiplexer connectionMultiplexer,
            ILogger<RedisCacheService> logger)
        {
            _distributedCache = distributedCache;
            _database = connectionMultiplexer.GetDatabase();
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var cachedValue = await _distributedCache.GetStringAsync(key, cancellationToken);
                if (cachedValue == null)
                {
                    _logger.LogDebug("Cache miss for key: {Key}", key);
                    return null;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var result = JsonSerializer.Deserialize<T>(cachedValue, options);
                _logger.LogDebug("Cache hit for key: {Key}", key);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cached value for key: {Key}", key);
                return null;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var expirationTime = expiration ?? _defaultExpiration;
                
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expirationTime,
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                };

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var serializedValue = JsonSerializer.Serialize(value, jsonOptions);
                await _distributedCache.SetStringAsync(key, serializedValue, options, cancellationToken);

                _logger.LogDebug("Cached value for key: {Key} with expiration: {Expiration}", key, expirationTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error caching value for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await _distributedCache.RemoveAsync(key, cancellationToken);
                _logger.LogDebug("Removed cached value for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cached value for key: {Key}", key);
            }
        }

        public async Task RemovePatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            try
            {
                var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
                var keys = server.Keys(pattern: pattern);
                
                var keyArray = keys.ToArray();
                if (keyArray.Length > 0)
                {
                    await _database.KeyDeleteAsync(keyArray);
                    _logger.LogDebug("Removed {Count} cached values matching pattern: {Pattern}", keyArray.Length, pattern);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cached values for pattern: {Pattern}", pattern);
            }
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var cachedValue = await _distributedCache.GetStringAsync(key, cancellationToken);
                return cachedValue != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if cached value exists for key: {Key}", key);
                return false;
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