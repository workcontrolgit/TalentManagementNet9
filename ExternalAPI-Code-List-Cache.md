# External API Cache Strategy and Implementation

## Overview
The TalentManagement system implements a comprehensive caching strategy for external API calls to improve performance, reduce external dependencies, and provide better user experience. This document details the cache implementation for USAJobs API integration.

## Cache Architecture

### Core Components
- **ICacheService**: Abstraction layer for caching operations
- **Service Layer**: Implements cache-aware external API calls
- **Configuration**: Flexible cache expiration settings
- **Cache Keys**: Standardized naming convention for cache entries

## USAJobs CodeList Cache Implementation

### Cache Configuration
```csharp
// Default cache expiration: 4 hours
_cacheExpiration = TimeSpan.FromHours(
    _configuration.GetValue<int>("USAJobs:CacheSettings:CodeListExpirationHours", 4));
```

### Cache Keys Strategy
All cache keys follow the pattern: `usajobs_codelist_{endpoint}`

| Endpoint | Cache Key | Expiration |
|----------|-----------|------------|
| Agency Subelements | `usajobs_codelist_agencysubelements` | 4 hours |
| Occupational Series | `usajobs_codelist_occupationalseries` | 4 hours |
| Pay Plans | `usajobs_codelist_payplans` | 4 hours |
| Postal Codes | `usajobs_codelist_postalcodes` | 4 hours |
| Geo Location Codes | `usajobs_codelist_geoloc` | 4 hours |
| GSA Geo Location Codes | `usajobs_codelist_gsageoloccodes` | 4 hours |
| Countries | `usajobs_codelist_countries` | 4 hours |
| Country Subdivisions | `usajobs_codelist_countrysubdivisions` | 4 hours |
| Travel Percentages | `usajobs_codelist_travelpercentages` | 4 hours |
| Position Schedule Types | `usajobs_codelist_positionscheduletypes` | 4 hours |
| Position Offering Types | `usajobs_codelist_positionofferingtypes` | 4 hours |
| Who May Apply | `usajobs_codelist_whomayapply` | 4 hours |
| Hiring Paths | `usajobs_codelist_hiringpaths` | 4 hours |
| Academic Honors | `usajobs_codelist_academichonors` | 4 hours |
| Action Codes | `usajobs_codelist_actioncodes` | 4 hours |
| Degree Type Codes | `usajobs_codelist_degreetypecodes` | 4 hours |
| Document Formats | `usajobs_codelist_documentformats` | 4 hours |
| Race Codes | `usajobs_codelist_racecodes` | 4 hours |
| Ethnicities | `usajobs_codelist_ethnicities` | 4 hours |
| Documentations | `usajobs_codelist_documentations` | 4 hours |
| Federal Employment Statuses | `usajobs_codelist_federalemploymentstatuses` | 4 hours |
| Language Proficiencies | `usajobs_codelist_languageproficiencies` | 4 hours |
| Language Codes | `usajobs_codelist_languagecodes` | 4 hours |
| Military Status Codes | `usajobs_codelist_militarystatuscodes` | 4 hours |
| Referee Type Codes | `usajobs_codelist_refereetypecodes` | 4 hours |
| Special Hirings | `usajobs_codelist_specialhirings` | 4 hours |
| Remuneration Rate Intervals | `usajobs_codelist_remunerationrateintervalcodes` | 4 hours |
| Application Statuses | `usajobs_codelist_applicationstatuses` | 4 hours |
| Academic Levels | `usajobs_codelist_academiclevels` | 4 hours |
| Security Clearances | `usajobs_codelist_securityclearances` | 4 hours |
| Key Standard Requirements | `usajobs_codelist_keystandardrequirements` | 4 hours |
| Required Standard Documents | `usajobs_codelist_requiredstandarddocuments` | 4 hours |
| Disabilities | `usajobs_codelist_disabilities` | 4 hours |
| Applicant Suppliers | `usajobs_codelist_applicantsuppliers` | 4 hours |
| Mission Critical Codes | `usajobs_codelist_missioncriticalcodes` | 4 hours |
| Announcement Closing Types | `usajobs_codelist_announcementclosingtypes` | 4 hours |
| Service Types | `usajobs_codelist_servicetypes` | 4 hours |
| Location Expansions | `usajobs_codelist_locationexpansions` | 4 hours |
| Travel Requirements | `usajobs_codelist_travelrequirements` | 4 hours |
| Remote Work Options | `usajobs_codelist_remotework` | 4 hours |

## Cache Implementation Pattern

### GetOrSet Pattern
All external API calls follow the GetOrSet pattern:

```csharp
private async Task<List<T>?> GetCodeListAsync<T>(string endpoint, string cacheKey, CancellationToken cancellationToken) 
    where T : BaseCodeListItem
{
    return await _cacheService.GetOrSetAsync(
        cacheKey,
        async () =>
        {
            // API call logic
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            // Process and return data
        },
        _cacheExpiration,
        cancellationToken);
}
```

### Cache Flow
1. **Check Cache**: Look for existing cached data using the cache key
2. **Cache Hit**: Return cached data immediately (fast response)
3. **Cache Miss**: 
   - Make API call to USAJobs
   - Process and deserialize response
   - Store in cache with expiration time
   - Return data to client

## Custom Endpoint Caching

### Search and Lookup Endpoints
Custom endpoints leverage the same cached data:

#### Occupational Series Search
```csharp
public async Task<List<OccupationalSeriesItem>?> SearchOccupationalSeriesAsync(string keyword, CancellationToken cancellationToken = default)
{
    var allSeries = await GetOccupationalSeriesAsync(cancellationToken); // Uses cache
    return allSeries?.Where(x => /* filter logic */).ToList();
}
```

#### Code Lookup
```csharp
public async Task<OccupationalSeriesItem?> GetOccupationalSeriesByCodeAsync(string code, CancellationToken cancellationToken = default)
{
    var allSeries = await GetOccupationalSeriesAsync(cancellationToken); // Uses cache
    return allSeries?.FirstOrDefault(x => x.Code?.Equals(code, StringComparison.OrdinalIgnoreCase) == true);
}
```

### Benefits of Custom Endpoint Caching
- ✅ **No Additional API Calls**: Search and lookup use existing cached data
- ✅ **Fast Response Times**: In-memory filtering after cache hit
- ✅ **Consistent Data**: All endpoints use the same cached dataset
- ✅ **Reduced External Dependencies**: Less reliance on USAJobs API availability

## Cache Management

### Manual Cache Refresh
The system provides a manual cache refresh endpoint:

```csharp
POST /api/v1/usajobscodelist/refresh
```

**Process:**
1. Removes all cached code list entries
2. Pre-populates commonly used code lists:
   - Occupational Series
   - Pay Plans
   - Hiring Paths
   - Position Schedule Types

### Cache Invalidation Strategy
- **Time-based**: All cache entries expire after 4 hours
- **Manual**: Explicit refresh via API endpoint
- **Startup**: Cache is populated on first request (lazy loading)

## Performance Characteristics

### Cache Hit Scenarios
- **Response Time**: ~1-5ms (in-memory retrieval)
- **External API Calls**: 0
- **Data Freshness**: Up to 4 hours old

### Cache Miss Scenarios
- **Response Time**: ~200-2000ms (network + processing)
- **External API Calls**: 1 per endpoint
- **Data Freshness**: Real-time from USAJobs API

### Pre-warming Strategy
Common code lists are pre-warmed during cache refresh to ensure better performance for frequently accessed data.

## Configuration Options

### App Settings
```json
{
  "USAJobs": {
    "CacheSettings": {
      "CodeListExpirationHours": 4
    }
  }
}
```

### Environment-Specific Settings
- **Development**: Shorter cache duration for testing (1 hour)
- **Staging**: Standard cache duration (4 hours)
- **Production**: Standard cache duration (4 hours)

## Monitoring and Observability

### Cache Health Check
```csharp
GET /api/v1/usajobscodelist/health
```
Tests USAJobs API availability without affecting cache.

### Logging
The system logs cache operations:
- Cache hits/misses
- API call durations
- Error conditions
- Cache refresh operations

## Error Handling

### Cache Failure Scenarios
1. **Cache Service Unavailable**: Falls back to direct API calls
2. **External API Failure**: Returns cached data (if available) or null
3. **Deserialization Errors**: Logged and returns null
4. **Network Timeouts**: Configurable retry logic

### Graceful Degradation
- Stale cache data is served if external API is unavailable
- Search/lookup endpoints return empty results gracefully
- System continues to function with reduced performance

## Best Practices

### Cache Key Naming
- Use consistent prefix: `usajobs_codelist_`
- Use lowercase endpoint names
- Avoid spaces and special characters

### Cache Expiration
- Code lists change infrequently (4-hour expiration is appropriate)
- Consider business requirements for data freshness
- Balance between performance and data accuracy

### Memory Considerations
- Monitor cache size in production
- Implement cache size limits if necessary
- Consider distributed caching for scale-out scenarios

## Future Enhancements

### Potential Improvements
1. **Distributed Caching**: Redis/SQL Server cache for multi-instance deployments
2. **Selective Refresh**: Refresh individual code lists instead of all
3. **Background Refresh**: Proactive cache warming before expiration
4. **Cache Analytics**: Detailed metrics on cache hit rates and performance
5. **Conditional Requests**: Use ETags/Last-Modified headers to reduce bandwidth

### Monitoring Additions
1. **Cache Hit Rate Metrics**: Track performance over time
2. **API Response Time Tracking**: Monitor external dependency health
3. **Cache Size Monitoring**: Prevent memory issues
4. **Error Rate Tracking**: Monitor external API reliability