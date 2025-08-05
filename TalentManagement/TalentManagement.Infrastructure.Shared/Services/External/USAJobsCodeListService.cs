using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;
using TalentManagement.Application.DTOs.External.USAJobs.CodeLists;
using TalentManagement.Application.Interfaces;
using TalentManagement.Application.Interfaces.External;

namespace TalentManagement.Infrastructure.Shared.Services.External
{
    public class USAJobsCodeListService : IUSAJobsCodeListService
    {
        private readonly HttpClient _httpClient;
        private readonly ICacheService _cacheService;
        private readonly ILogger<USAJobsCodeListService> _logger;
        private readonly IConfiguration _configuration;
        
        private readonly string _apiKey;
        private readonly string _userAgent;
        private readonly string _baseUrl;
        private readonly TimeSpan _cacheExpiration;
        
        // Cache keys
        private const string OCCUPATIONAL_SERIES_KEY = "usajobs_codelist_occupationalseries";
        private const string PAY_PLANS_KEY = "usajobs_codelist_payplans";
        private const string HIRING_PATHS_KEY = "usajobs_codelist_hiringpaths";
        private const string POSITION_SCHEDULE_TYPES_KEY = "usajobs_codelist_positionscheduletypes";
        private const string WORK_SCHEDULES_KEY = "usajobs_codelist_workschedules";
        private const string SECURITY_CLEARANCES_KEY = "usajobs_codelist_securityclearances";
        private const string COUNTRIES_KEY = "usajobs_codelist_countries";
        private const string POSTAL_CODES_KEY = "usajobs_codelist_postalcodes";
        private const string GEO_LOCATIONS_KEY = "usajobs_codelist_geoloc";
        private const string TRAVEL_REQUIREMENTS_KEY = "usajobs_codelist_travelrequirements";
        private const string REMOTE_WORK_KEY = "usajobs_codelist_remotework";

        public USAJobsCodeListService(
            HttpClient httpClient,
            ICacheService cacheService,
            ILogger<USAJobsCodeListService> logger,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _cacheService = cacheService;
            _logger = logger;
            _configuration = configuration;

            _apiKey = _configuration["USAJobs:ApiKey"] ?? throw new InvalidOperationException("USAJobs API Key not configured");
            _userAgent = _configuration["USAJobs:UserAgent"] ?? "TalentManagement/1.0";
            _baseUrl = _configuration.GetValue<string>("USAJobs:BaseUrl", "https://data.usajobs.gov/api");
            
            // Code lists change infrequently, so cache for longer (4 hours default)
            _cacheExpiration = TimeSpan.FromHours(
                _configuration.GetValue<int>("USAJobs:CacheSettings:CodeListExpirationHours", 4));

            ConfigureHttpClient();
        }

        private void ConfigureHttpClient()
        {
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", _userAgent);
            _httpClient.DefaultRequestHeaders.Add("Authorization-Key", _apiKey);
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            
            _logger.LogDebug("USAJobs CodeList HTTP Client configured with BaseUrl: {BaseUrl}, UserAgent: {UserAgent}", 
                _baseUrl, _userAgent);
        }

        public async Task<List<OccupationalSeriesItem>?> GetOccupationalSeriesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<OccupationalSeriesItem>(
                "/codelist/occupationalseries", 
                OCCUPATIONAL_SERIES_KEY, 
                cancellationToken);
        }

        public async Task<List<PayPlanItem>?> GetPayPlansAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<PayPlanItem>(
                "/codelist/payplans", 
                PAY_PLANS_KEY, 
                cancellationToken);
        }

        public async Task<List<HiringPathItem>?> GetHiringPathsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<HiringPathItem>(
                "/codelist/hiringpaths", 
                HIRING_PATHS_KEY, 
                cancellationToken);
        }

        public async Task<List<PositionScheduleTypeItem>?> GetPositionScheduleTypesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<PositionScheduleTypeItem>(
                "/codelist/positionscheduletypes", 
                POSITION_SCHEDULE_TYPES_KEY, 
                cancellationToken);
        }

        public async Task<List<WorkScheduleItem>?> GetWorkSchedulesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<WorkScheduleItem>(
                "/codelist/workschedules", 
                WORK_SCHEDULES_KEY, 
                cancellationToken);
        }

        public async Task<List<SecurityClearanceItem>?> GetSecurityClearancesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<SecurityClearanceItem>(
                "/codelist/securityclearances", 
                SECURITY_CLEARANCES_KEY, 
                cancellationToken);
        }

        public async Task<List<CountryItem>?> GetCountriesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<CountryItem>(
                "/codelist/countries", 
                COUNTRIES_KEY, 
                cancellationToken);
        }

        public async Task<List<PostalCodeItem>?> GetPostalCodesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<PostalCodeItem>(
                "/codelist/postalcodes", 
                POSTAL_CODES_KEY, 
                cancellationToken);
        }

        public async Task<List<GeoLocationItem>?> GetGeoLocationsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<GeoLocationItem>(
                "/codelist/geoloc", 
                GEO_LOCATIONS_KEY, 
                cancellationToken);
        }

        public async Task<List<TravelRequirementItem>?> GetTravelRequirementsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<TravelRequirementItem>(
                "/codelist/travelrequirements", 
                TRAVEL_REQUIREMENTS_KEY, 
                cancellationToken);
        }

        public async Task<List<RemoteWorkItem>?> GetRemoteWorkOptionsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<RemoteWorkItem>(
                "/codelist/remotework", 
                REMOTE_WORK_KEY, 
                cancellationToken);
        }

        public async Task<OccupationalSeriesItem?> GetOccupationalSeriesByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            var allSeries = await GetOccupationalSeriesAsync(cancellationToken);
            return allSeries?.FirstOrDefault(x => x.Code?.Equals(code, StringComparison.OrdinalIgnoreCase) == true);
        }

        public async Task<PayPlanItem?> GetPayPlanByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            var allPlans = await GetPayPlansAsync(cancellationToken);
            return allPlans?.FirstOrDefault(x => x.Code?.Equals(code, StringComparison.OrdinalIgnoreCase) == true);
        }

        public async Task<List<OccupationalSeriesItem>?> SearchOccupationalSeriesAsync(string keyword, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new List<OccupationalSeriesItem>();

            var allSeries = await GetOccupationalSeriesAsync(cancellationToken);
            if (allSeries == null)
                return null;

            var searchTerm = keyword.ToLowerInvariant();
            return allSeries
                .Where(x => x.IsActive && (
                    (x.Code?.ToLowerInvariant().Contains(searchTerm) == true) ||
                    (x.Value?.ToLowerInvariant().Contains(searchTerm) == true)))
                .OrderBy(x => x.Code)
                .ToList();
        }

        public async Task RefreshAllCodeListsAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Refreshing all USAJobs code lists from API");

            var cacheKeys = new[]
            {
                OCCUPATIONAL_SERIES_KEY, PAY_PLANS_KEY, HIRING_PATHS_KEY,
                POSITION_SCHEDULE_TYPES_KEY, WORK_SCHEDULES_KEY, SECURITY_CLEARANCES_KEY,
                COUNTRIES_KEY, POSTAL_CODES_KEY, GEO_LOCATIONS_KEY,
                TRAVEL_REQUIREMENTS_KEY, REMOTE_WORK_KEY
            };

            // Remove all cached code lists
            foreach (var key in cacheKeys)
            {
                await _cacheService.RemoveAsync(key, cancellationToken);
            }

            // Pre-populate the most commonly used code lists
            var tasks = new Task[]
            {
                GetOccupationalSeriesAsync(cancellationToken),
                GetPayPlansAsync(cancellationToken),
                GetHiringPathsAsync(cancellationToken),
                GetPositionScheduleTypesAsync(cancellationToken)
            };

            await Task.WhenAll(tasks);
            _logger.LogInformation("Completed refreshing USAJobs code lists");
        }

        public async Task<bool> IsServiceAvailableAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // Test with a simple endpoint
                var response = await _httpClient.GetAsync("/codelist/occupationalseries", cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "USAJobs CodeList service is not available");
                return false;
            }
        }

        private async Task<List<T>?> GetCodeListAsync<T>(string endpoint, string cacheKey, CancellationToken cancellationToken) 
            where T : BaseCodeListItem
        {
            return await _cacheService.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    try
                    {
                        _logger.LogDebug("Fetching code list from USAJobs API: {Endpoint}", endpoint);
                        
                        var response = await _httpClient.GetAsync(endpoint, cancellationToken);
                        
                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogWarning("USAJobs CodeList API returned {StatusCode} for endpoint: {Endpoint}", 
                                response.StatusCode, endpoint);
                            return null;
                        }

                        var content = await response.Content.ReadAsStringAsync(cancellationToken);
                        
                        if (string.IsNullOrEmpty(content))
                        {
                            _logger.LogWarning("USAJobs CodeList API returned empty response for endpoint: {Endpoint}", endpoint);
                            return null;
                        }

                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        };

                        var codeListResponse = JsonSerializer.Deserialize<USAJobsCodeListResponse<T>>(content, options);
                        
                        var items = codeListResponse?.CodeList?.ToList();

                        _logger.LogInformation("Successfully retrieved {Count} items from USAJobs CodeList: {Endpoint}", 
                            items?.Count ?? 0, endpoint);
                        
                        return items;
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Error deserializing USAJobs CodeList response for endpoint: {Endpoint}", endpoint);
                        return null;
                    }
                    catch (HttpRequestException ex)
                    {
                        _logger.LogError(ex, "HTTP error occurred while calling USAJobs CodeList API for endpoint: {Endpoint}", endpoint);
                        return null;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unexpected error occurred while calling USAJobs CodeList API for endpoint: {Endpoint}", endpoint);
                        return null;
                    }
                },
                _cacheExpiration,
                cancellationToken);
        }
    }
}