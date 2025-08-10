using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TalentManagement.Application.DTOs.External.USAJobs;
using TalentManagement.Application.Interfaces;
using TalentManagement.Application.Interfaces.External;

namespace TalentManagement.Infrastructure.Shared.Services.External
{
    public class CachedUSAJobsService : IUSAJobsService
    {
        private readonly USAJobsService _baseService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<CachedUSAJobsService> _logger;
        private readonly IConfiguration _configuration;
        
        private readonly TimeSpan _searchCacheExpiration;
        private readonly TimeSpan _detailsCacheExpiration;

        public CachedUSAJobsService(
            USAJobsService baseService,
            ICacheService cacheService,
            ILogger<CachedUSAJobsService> logger,
            IConfiguration configuration)
        {
            _baseService = baseService;
            _cacheService = cacheService;
            _logger = logger;
            _configuration = configuration;

            // Get cache settings from configuration
            _searchCacheExpiration = TimeSpan.FromMinutes(
                _configuration.GetValue<int>("USAJobs:CacheSettings:JobSearchExpirationMinutes", 15));
            
            _detailsCacheExpiration = TimeSpan.FromMinutes(
                _configuration.GetValue<int>("USAJobs:CacheSettings:JobDetailsExpirationMinutes", 60));
        }

        public async Task<USAJobsResponse?> SearchJobsAsync(USAJobsSearchRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                // Create cache key based on search parameters
                var cacheKey = CreateSearchCacheKey(request);
                
                _logger.LogDebug("Checking cache for USAJobs search: {CacheKey}", cacheKey);

                return await _cacheService.GetOrSetAsync(
                    cacheKey,
                    async () =>
                    {
                        _logger.LogInformation("Cache miss - calling USAJobs API for search");
                        return await _baseService.SearchJobsAsync(request, cancellationToken);
                    },
                    _searchCacheExpiration,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in cached USAJobs search");
                throw;
            }
        }

        public async Task<MatchedObjectDescriptor?> GetJobDetailsAsync(string positionId, CancellationToken cancellationToken = default)
        {
            try
            {
                var cacheKey = $"usajobs_details_{positionId}";
                
                _logger.LogDebug("Checking cache for USAJobs job details: {CacheKey}", cacheKey);

                return await _cacheService.GetOrSetAsync(
                    cacheKey,
                    async () =>
                    {
                        _logger.LogInformation("Cache miss - calling USAJobs API for job details: {PositionId}", positionId);
                        return await _baseService.GetJobDetailsAsync(positionId, cancellationToken);
                    },
                    _detailsCacheExpiration,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in cached USAJobs job details for position: {PositionId}", positionId);
                throw;
            }
        }

        public async Task<bool> ValidateApiConnectionAsync(CancellationToken cancellationToken = default)
        {
            // Don't cache health checks - always hit the API directly
            return await _baseService.ValidateApiConnectionAsync(cancellationToken);
        }

        private static string CreateSearchCacheKey(USAJobsSearchRequest request)
        {
            var keyParts = new List<string> { "usajobs_search" };
            
            if (!string.IsNullOrEmpty(request.Keyword))
                keyParts.Add($"kw_{request.Keyword.Replace(" ", "_")}");
            
            if (!string.IsNullOrEmpty(request.LocationName))
                keyParts.Add($"loc_{request.LocationName.Replace(" ", "_")}");
            
            if (request.Page.HasValue)
                keyParts.Add($"p_{request.Page}");
            
            if (request.ResultsPerPage.HasValue)
                keyParts.Add($"rpp_{request.ResultsPerPage}");
            
            if (!string.IsNullOrEmpty(request.SortField))
                keyParts.Add($"sf_{request.SortField}");
            
            if (!string.IsNullOrEmpty(request.SortDirection))
                keyParts.Add($"sd_{request.SortDirection}");
            
            if (!string.IsNullOrEmpty(request.Organization))
                keyParts.Add($"org_{request.Organization.Replace(" ", "_")}");
            
            if (!string.IsNullOrEmpty(request.PayGradeHigh))
                keyParts.Add($"pgh_{request.PayGradeHigh}");
            
            if (!string.IsNullOrEmpty(request.PayGradeLow))
                keyParts.Add($"pgl_{request.PayGradeLow}");
            
            if (request.DatePosted.HasValue)
                keyParts.Add($"dp_{request.DatePosted.Value:yyyyMMdd}");

            return string.Join("_", keyParts).ToLowerInvariant();
        }
    }
}