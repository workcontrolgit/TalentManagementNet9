# Cache Implementation for External Web APIs

This document describes how the caching system works for external web API calls in the TalentManagement application, specifically for the USAJobs API integration.

## 🏗️ Architecture Overview

The caching system uses a **Decorator Pattern** with multiple layers:

```
Controller → CachedUSAJobsService → MemoryCacheService → USAJobsService → External API
             (Decorator)             (In-Memory Cache)   (Base Service)
```

## 🔧 Service Registration

In `ServiceRegistration.cs`, the services are registered with dependency injection:

```csharp
// Base HTTP service
services.AddHttpClient<USAJobsService>(client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "TalentManagement/1.0");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Caching infrastructure
services.AddMemoryCache();
services.AddSingleton<ICacheService, MemoryCacheService>();

// Service registrations
services.AddScoped<USAJobsService>();                    // Base service
services.AddScoped<IUSAJobsService, CachedUSAJobsService>(); // Cached decorator
```

**Key Points:**
- Controllers receive `IUSAJobsService`, which resolves to `CachedUSAJobsService`
- `CachedUSAJobsService` wraps the base `USAJobsService`  
- `MemoryCacheService` provides the caching infrastructure

## 📦 Cache Key Strategy

### Search Cache Keys
Format: `usajobs_search_[parameters]`

Example:
```
usajobs_search_kw_software_engineer_loc_washington_dc_p_1_rpp_25
```

Built from search parameters in `CachedUSAJobsService.CreateSearchCacheKey()`:
- **Keyword**: `kw_{keyword}` (spaces replaced with underscores)
- **Location**: `loc_{location}` (spaces replaced with underscores)  
- **Page**: `p_{pageNumber}`
- **Results per page**: `rpp_{resultsPerPage}`
- **Sort field**: `sf_{sortField}`
- **Sort direction**: `sd_{sortDirection}`
- **Organization**: `org_{organization}`
- **Pay grades**: `pgh_{payGradeHigh}`, `pgl_{payGradeLow}`
- **Date posted**: `dp_{datePosted:yyyyMMdd}`

### Job Details Cache Keys
Format: `usajobs_details_{positionId}`

Example:
```
usajobs_details_AGL-ERR-20-2101-63651
```

## ⏰ Cache Expiration

Cache expiration is configurable via `appsettings.json`:

```json
{
  "USAJobs": {
    "CacheSettings": {
      "JobSearchExpirationMinutes": 15,    // Search results cache
      "JobDetailsExpirationMinutes": 60    // Job details cache
    }
  }
}
```

**Default Values:**
- **Search Results**: 15 minutes (shorter due to frequent updates)
- **Job Details**: 60 minutes (longer due to less frequent changes)

## 🔄 Cache Flow

### Search Request Flow

1. **Controller** calls `IUSAJobsService.SearchJobsAsync(request)`
2. **CachedUSAJobsService** creates cache key from search parameters
3. **MemoryCacheService.GetOrSetAsync()** executes:
   - **Cache Hit**: Returns cached data immediately
   - **Cache Miss**: Executes fallback function
4. **Fallback Function**: Calls `USAJobsService.SearchJobsAsync()` → external USAJobs API
5. **Fresh Data**: Cached with configured expiration time
6. **Result**: Returned to controller

### Job Details Request Flow

1. **Controller** calls `IUSAJobsService.GetJobDetailsAsync(positionId)`
2. **CachedUSAJobsService** creates cache key: `usajobs_details_{positionId}`
3. **Same cache flow** as search but with longer expiration (60 min vs 15 min)
4. **Result**: Job details returned to controller

## 💾 Memory Cache Implementation

The `MemoryCacheService` provides advanced caching features:

### Storage Strategy
- **JSON Serialization**: Objects stored as JSON strings for consistency
- **Type Safety**: Generic methods with compile-time type checking
- **Memory Efficiency**: Automatic cleanup when items expire

### Cache Entry Options
```csharp
var options = new MemoryCacheEntryOptions
{
    AbsoluteExpirationRelativeToNow = expirationTime,
    SlidingExpiration = TimeSpan.FromMinutes(5), // Refresh if accessed within 5 minutes
    Priority = CacheItemPriority.Normal
};
```

### Key Features

#### 1. **GetOrSetAsync Pattern**
```csharp
return await _cacheService.GetOrSetAsync(
    cacheKey,
    async () => await _baseService.SearchJobsAsync(request, cancellationToken),
    _searchCacheExpiration,
    cancellationToken);
```

#### 2. **Pattern-Based Removal**
```csharp
await _cacheService.RemovePatternAsync("usajobs_search.*");
```

#### 3. **Key Tracking**
- Uses `ConcurrentDictionary<string, object>` to track all cache keys
- Enables pattern-based cache invalidation
- Automatic cleanup on eviction

#### 4. **Sliding Expiration**
- Items accessed within 5 minutes of expiration get refreshed
- Balances freshness with performance

## 🛡️ Error Handling & Resilience

### Cache Failure Resilience
```csharp
try
{
    return await getItem(); // Fallback to direct API call
}
catch (Exception innerEx)
{
    _logger.LogError(innerEx, "Error in fallback getItem for key: {Key}", key);
    throw;
}
```

### Error Scenarios Handled
1. **Cache Serialization Errors**: Falls back to direct API call
2. **Memory Pressure**: Automatic eviction of low-priority items
3. **Cache Service Unavailable**: Direct API calls continue working
4. **API Errors**: Not cached - errors propagated to caller

### Health Checks Exception
```csharp
public async Task<bool> ValidateApiConnectionAsync(CancellationToken cancellationToken = default)
{
    // Don't cache health checks - always hit the API directly
    return await _baseService.ValidateApiConnectionAsync(cancellationToken);
}
```

## 🚫 What's NOT Cached

- **Health checks** (`ValidateApiConnectionAsync`) - always hit API directly
- **Failed API responses** - errors not cached, only successful responses  
- **Null responses** - only non-null successful responses are cached
- **Real-time data** - cache expiration ensures reasonable freshness

## 📊 Cache Benefits

### Performance Benefits
- **Reduced Latency**: Cached responses return in ~1ms vs ~2000ms API calls
- **Improved Throughput**: Handle more concurrent requests
- **Better User Experience**: Faster page loads for repeated searches

### Operational Benefits  
- **Rate Limiting Protection**: Reduces external API usage
- **Cost Optimization**: Fewer billable API calls (if rate-limited/paid)
- **Availability**: Partial functionality during API outages (if cached data available)
- **Bandwidth Reduction**: Less network traffic

### Monitoring & Observability
- **Cache Hit/Miss Logging**: Debug-level logs for cache performance analysis
- **Error Tracking**: Comprehensive error logging for troubleshooting
- **Key Tracking**: Visibility into cached items for debugging

## 🔍 Usage Examples

### Basic Search (Cached)
```csharp
// First call - cache miss, hits API
var result1 = await _usaJobsService.SearchJobsAsync(new USAJobsSearchRequest 
{ 
    Keyword = "Software Engineer",
    Page = 1,
    ResultsPerPage = 25
});

// Same call within 15 minutes - cache hit, no API call
var result2 = await _usaJobsService.SearchJobsAsync(new USAJobsSearchRequest 
{ 
    Keyword = "Software Engineer", 
    Page = 1,
    ResultsPerPage = 25
});
```

### Cache Key Variations
```csharp
// Different cache keys due to different parameters
"usajobs_search_kw_software_engineer_p_1_rpp_25"     // Page 1
"usajobs_search_kw_software_engineer_p_2_rpp_25"     // Page 2  
"usajobs_search_kw_data_scientist_p_1_rpp_25"        // Different keyword
```

## 🚀 Future Enhancements

### Potential Improvements
1. **Distributed Caching**: Redis for multi-instance deployments
2. **Cache Warming**: Pre-populate common searches
3. **Adaptive Expiration**: Dynamic expiration based on data volatility  
4. **Cache Metrics**: Detailed performance metrics and dashboards
5. **Conditional Requests**: ETags/If-Modified-Since for API efficiency

### Configuration Extensions
```json
{
  "USAJobs": {
    "CacheSettings": {
      "JobSearchExpirationMinutes": 15,
      "JobDetailsExpirationMinutes": 60,
      "MaxCacheSize": 1000,
      "EnableDistributedCache": false,
      "CacheMetricsEnabled": true
    }
  }
}
```

---

## 📝 Implementation Notes

- The cache is **transparent** to controllers - they use `IUSAJobsService` without knowing about caching
- Cache keys are **case-insensitive** and use underscores for consistency  
- The system is **fail-safe** - if caching fails, the API call still succeeds
- **Memory usage** is managed automatically by .NET's `IMemoryCache`
- All cache operations are **thread-safe** using `ConcurrentDictionary`

This implementation provides a robust, performant, and maintainable caching layer for external API integrations.