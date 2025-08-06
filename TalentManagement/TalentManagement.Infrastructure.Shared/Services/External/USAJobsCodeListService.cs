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
        private const string AGENCY_SUBELEMENTS_KEY = "usajobs_codelist_agencysubelements";
        private const string OCCUPATIONAL_SERIES_KEY = "usajobs_codelist_occupationalseries";
        private const string PAY_PLANS_KEY = "usajobs_codelist_payplans";
        private const string POSTAL_CODES_KEY = "usajobs_codelist_postalcodes";
        private const string GEO_LOCATIONS_KEY = "usajobs_codelist_geoloc";
        private const string GSA_GEO_LOCATIONS_KEY = "usajobs_codelist_gsageoloccodes";
        private const string COUNTRIES_KEY = "usajobs_codelist_countries";
        private const string COUNTRY_SUBDIVISIONS_KEY = "usajobs_codelist_countrysubdivisions";
        private const string TRAVEL_PERCENTAGES_KEY = "usajobs_codelist_travelpercentages";
        private const string POSITION_SCHEDULE_TYPES_KEY = "usajobs_codelist_positionscheduletypes";
        private const string POSITION_OFFERING_TYPES_KEY = "usajobs_codelist_positionofferingtypes";
        private const string WHO_MAY_APPLY_KEY = "usajobs_codelist_whomayapply";
        private const string HIRING_PATHS_KEY = "usajobs_codelist_hiringpaths";
        private const string ACADEMIC_HONORS_KEY = "usajobs_codelist_academichonors";
        private const string ACTION_CODES_KEY = "usajobs_codelist_actioncodes";
        private const string DEGREE_TYPE_CODES_KEY = "usajobs_codelist_degreetypecodes";
        private const string DOCUMENT_FORMATS_KEY = "usajobs_codelist_documentformats";
        private const string RACE_CODES_KEY = "usajobs_codelist_racecodes";
        private const string ETHNICITIES_KEY = "usajobs_codelist_ethnicities";
        private const string DOCUMENTATIONS_KEY = "usajobs_codelist_documentations";
        private const string FEDERAL_EMPLOYMENT_STATUSES_KEY = "usajobs_codelist_federalemploymentstatuses";
        private const string LANGUAGE_PROFICIENCIES_KEY = "usajobs_codelist_languageproficiencies";
        private const string LANGUAGE_CODES_KEY = "usajobs_codelist_languagecodes";
        private const string MILITARY_STATUS_CODES_KEY = "usajobs_codelist_militarystatuscodes";
        private const string REFEREE_TYPE_CODES_KEY = "usajobs_codelist_refereetypecodes";
        private const string SPECIAL_HIRINGS_KEY = "usajobs_codelist_specialhirings";
        private const string REMUNERATION_RATE_INTERVAL_CODES_KEY = "usajobs_codelist_remunerationrateintervalcodes";
        private const string APPLICATION_STATUSES_KEY = "usajobs_codelist_applicationstatuses";
        private const string ACADEMIC_LEVELS_KEY = "usajobs_codelist_academiclevels";
        private const string SECURITY_CLEARANCES_KEY = "usajobs_codelist_securityclearances";
        private const string KEY_STANDARD_REQUIREMENTS_KEY = "usajobs_codelist_keystandardrequirements";
        private const string REQUIRED_STANDARD_DOCUMENTS_KEY = "usajobs_codelist_requiredstandarddocuments";
        private const string DISABILITIES_KEY = "usajobs_codelist_disabilities";
        private const string APPLICANT_SUPPLIERS_KEY = "usajobs_codelist_applicantsuppliers";
        private const string MISSION_CRITICAL_CODES_KEY = "usajobs_codelist_missioncriticalcodes";
        private const string ANNOUNCEMENT_CLOSING_TYPES_KEY = "usajobs_codelist_announcementclosingtypes";
        private const string SERVICE_TYPES_KEY = "usajobs_codelist_servicetypes";
        private const string LOCATION_EXPANSIONS_KEY = "usajobs_codelist_locationexpansions";
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
            _baseUrl = _configuration["USAJobs:CodeListBaseUrl"] ?? "https://data.usajobs.gov/api/codelist";

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
            
            _logger.LogInformation("USAJobs CodeList HTTP Client configured with BaseUrl: {BaseUrl}, UserAgent: {UserAgent}", 
                _baseUrl, _userAgent);
        }


        public async Task<List<OccupationalSeriesItem>?> GetOccupationalSeriesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<OccupationalSeriesItem>(
                "/occupationalseries", 
                OCCUPATIONAL_SERIES_KEY, 
                cancellationToken);
        }

        public async Task<List<PayPlanItem>?> GetPayPlansAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<PayPlanItem>(
                "/payplans", 
                PAY_PLANS_KEY, 
                cancellationToken);
        }

        public async Task<List<HiringPathItem>?> GetHiringPathsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<HiringPathItem>(
                "/hiringpaths", 
                HIRING_PATHS_KEY, 
                cancellationToken);
        }

        public async Task<List<PositionScheduleTypeItem>?> GetPositionScheduleTypesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<PositionScheduleTypeItem>(
                "/positionscheduletypes", 
                POSITION_SCHEDULE_TYPES_KEY, 
                cancellationToken);
        }


        public async Task<List<SecurityClearanceItem>?> GetSecurityClearancesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<SecurityClearanceItem>(
                "/securityclearances", 
                SECURITY_CLEARANCES_KEY, 
                cancellationToken);
        }

        public async Task<List<CountryItem>?> GetCountriesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<CountryItem>(
                "/countries", 
                COUNTRIES_KEY, 
                cancellationToken);
        }

        public async Task<List<PostalCodeItem>?> GetPostalCodesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<PostalCodeItem>(
                "/postalcodes", 
                POSTAL_CODES_KEY, 
                cancellationToken);
        }

        public async Task<List<GeoLocationItem>?> GetGeoLocationsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<GeoLocationItem>(
                "/geoloc", 
                GEO_LOCATIONS_KEY, 
                cancellationToken);
        }

        public async Task<List<TravelRequirementItem>?> GetTravelRequirementsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<TravelRequirementItem>(
                "/travelrequirements", 
                TRAVEL_REQUIREMENTS_KEY, 
                cancellationToken);
        }

        public async Task<List<RemoteWorkItem>?> GetRemoteWorkOptionsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<RemoteWorkItem>(
                "/remotework", 
                REMOTE_WORK_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetAgencySubelementsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/agencysubelements", 
                AGENCY_SUBELEMENTS_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetGsaGeoLocationCodesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/gsageoloccodes", 
                GSA_GEO_LOCATIONS_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetCountrySubdivisionsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/countrysubdivisions", 
                COUNTRY_SUBDIVISIONS_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetTravelPercentagesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/travelpercentages", 
                TRAVEL_PERCENTAGES_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetPositionOfferingTypesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/positionofferingtypes", 
                POSITION_OFFERING_TYPES_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetWhoMayApplyAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/whomayapply", 
                WHO_MAY_APPLY_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetAcademicHonorsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/academichonors", 
                ACADEMIC_HONORS_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetActionCodesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/actioncodes", 
                ACTION_CODES_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetDegreeTypeCodesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/degreetypecodes", 
                DEGREE_TYPE_CODES_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetDocumentFormatsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/documentformats", 
                DOCUMENT_FORMATS_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetRaceCodesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/racecodes", 
                RACE_CODES_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetEthnicitiesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/ethnicities", 
                ETHNICITIES_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetDocumentationsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/documentations", 
                DOCUMENTATIONS_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetFederalEmploymentStatusesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/federalemploymentstatuses", 
                FEDERAL_EMPLOYMENT_STATUSES_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetLanguageProficienciesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/languageproficiencies", 
                LANGUAGE_PROFICIENCIES_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetLanguageCodesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/languagecodes", 
                LANGUAGE_CODES_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetMilitaryStatusCodesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/militarystatuscodes", 
                MILITARY_STATUS_CODES_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetRefereeTypeCodesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/refereetypecodes", 
                REFEREE_TYPE_CODES_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetSpecialHiringsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/specialhirings", 
                SPECIAL_HIRINGS_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetRemunerationRateIntervalCodesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/remunerationrateintervalcodes", 
                REMUNERATION_RATE_INTERVAL_CODES_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetApplicationStatusesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/applicationstatuses", 
                APPLICATION_STATUSES_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetAcademicLevelsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/academiclevels", 
                ACADEMIC_LEVELS_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetKeyStandardRequirementsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/keystandardrequirements", 
                KEY_STANDARD_REQUIREMENTS_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetRequiredStandardDocumentsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/requiredstandarddocuments", 
                REQUIRED_STANDARD_DOCUMENTS_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetDisabilitiesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/disabilities", 
                DISABILITIES_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetApplicantSuppliersAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/applicantsuppliers", 
                APPLICANT_SUPPLIERS_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetMissionCriticalCodesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/missioncriticalcodes", 
                MISSION_CRITICAL_CODES_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetAnnouncementClosingTypesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/announcementclosingtypes", 
                ANNOUNCEMENT_CLOSING_TYPES_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetServiceTypesAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/servicetypes", 
                SERVICE_TYPES_KEY, 
                cancellationToken);
        }

        public async Task<List<BaseCodeListItem>?> GetLocationExpansionsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCodeListAsync<BaseCodeListItem>(
                "/locationexpansions", 
                LOCATION_EXPANSIONS_KEY, 
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
                AGENCY_SUBELEMENTS_KEY, OCCUPATIONAL_SERIES_KEY, PAY_PLANS_KEY, POSTAL_CODES_KEY,
                GEO_LOCATIONS_KEY, GSA_GEO_LOCATIONS_KEY, COUNTRIES_KEY, COUNTRY_SUBDIVISIONS_KEY,
                TRAVEL_PERCENTAGES_KEY, POSITION_SCHEDULE_TYPES_KEY, POSITION_OFFERING_TYPES_KEY,
                WHO_MAY_APPLY_KEY, HIRING_PATHS_KEY, ACADEMIC_HONORS_KEY, ACTION_CODES_KEY,
                DEGREE_TYPE_CODES_KEY, DOCUMENT_FORMATS_KEY, RACE_CODES_KEY, ETHNICITIES_KEY,
                DOCUMENTATIONS_KEY, FEDERAL_EMPLOYMENT_STATUSES_KEY, LANGUAGE_PROFICIENCIES_KEY,
                LANGUAGE_CODES_KEY, MILITARY_STATUS_CODES_KEY, REFEREE_TYPE_CODES_KEY,
                SPECIAL_HIRINGS_KEY, REMUNERATION_RATE_INTERVAL_CODES_KEY, APPLICATION_STATUSES_KEY,
                ACADEMIC_LEVELS_KEY, SECURITY_CLEARANCES_KEY, KEY_STANDARD_REQUIREMENTS_KEY,
                REQUIRED_STANDARD_DOCUMENTS_KEY, DISABILITIES_KEY, APPLICANT_SUPPLIERS_KEY,
                MISSION_CRITICAL_CODES_KEY, ANNOUNCEMENT_CLOSING_TYPES_KEY, SERVICE_TYPES_KEY,
                LOCATION_EXPANSIONS_KEY, TRAVEL_REQUIREMENTS_KEY, REMOTE_WORK_KEY
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
                var response = await _httpClient.GetAsync("/occupationalseries", cancellationToken);
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
                        var fullUrl = $"{_httpClient.BaseAddress?.ToString().TrimEnd('/')}{endpoint}";
                        _logger.LogInformation("Fetching code list from USAJobs API: {FullUrl} (BaseAddress: {BaseAddress}, Endpoint: {Endpoint})", 
                            fullUrl, _httpClient.BaseAddress, endpoint);
                        
                        var response = await _httpClient.GetAsync(fullUrl, cancellationToken);
                        
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
                        
                        var items = codeListResponse?.CodeList?
                            .SelectMany(item => item.ValidValue ?? new List<T>())
                            .ToList();

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