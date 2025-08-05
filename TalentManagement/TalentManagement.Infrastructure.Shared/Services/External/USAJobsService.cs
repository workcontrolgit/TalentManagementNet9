using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;
using TalentManagement.Application.DTOs.External.USAJobs;
using TalentManagement.Application.Interfaces.External;

namespace TalentManagement.Infrastructure.Shared.Services.External
{
    public class USAJobsService : IUSAJobsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<USAJobsService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _userAgent;
        private readonly string _baseUrl;

        public USAJobsService(
            HttpClient httpClient,
            ILogger<USAJobsService> logger,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            
            _apiKey = _configuration["USAJobs:ApiKey"] ?? throw new InvalidOperationException("USAJobs API Key not configured");
            _userAgent = _configuration["USAJobs:UserAgent"] ?? "TalentManagement/1.0";
            _baseUrl = _configuration["USAJobs:BaseUrl"] ?? "https://data.usajobs.gov/api";

            ConfigureHttpClient();
        }

        private void ConfigureHttpClient()
        {
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Authorization-Key", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", _userAgent);
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<USAJobsResponse?> SearchJobsAsync(USAJobsSearchRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var queryParams = BuildSearchQueryString(request);
                var endpoint = $"/search?{queryParams}";

                _logger.LogInformation("Searching USAJobs with endpoint: {Endpoint}", endpoint);

                var response = await _httpClient.GetAsync(endpoint, cancellationToken);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("USAJobs API returned error status: {StatusCode} - {ReasonPhrase}", 
                        response.StatusCode, response.ReasonPhrase);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                
                if (string.IsNullOrEmpty(content))
                {
                    _logger.LogWarning("USAJobs API returned empty response");
                    return null;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var result = JsonSerializer.Deserialize<USAJobsResponse>(content, options);
                
                _logger.LogInformation("Successfully retrieved {Count} jobs from USAJobs API", 
                    result?.SearchResult?.SearchResultItems?.Count ?? 0);

                return result;
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                _logger.LogError(ex, "Timeout occurred while calling USAJobs API");
                throw new HttpRequestException("USAJobs API request timed out", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error occurred while calling USAJobs API");
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing USAJobs API response");
                throw new InvalidOperationException("Failed to parse USAJobs API response", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while calling USAJobs API");
                throw;
            }
        }

        public async Task<MatchedObjectDescriptor?> GetJobDetailsAsync(string positionId, CancellationToken cancellationToken = default)
        {
            try
            {
                var endpoint = $"/search?PositionID={Uri.EscapeDataString(positionId)}";

                _logger.LogInformation("Getting job details for position: {PositionId}", positionId);

                var response = await _httpClient.GetAsync(endpoint, cancellationToken);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("USAJobs API returned error status: {StatusCode} - {ReasonPhrase}", 
                        response.StatusCode, response.ReasonPhrase);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                
                if (string.IsNullOrEmpty(content))
                {
                    _logger.LogWarning("USAJobs API returned empty response for position: {PositionId}", positionId);
                    return null;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var result = JsonSerializer.Deserialize<USAJobsResponse>(content, options);
                var jobDetails = result?.SearchResult?.SearchResultItems?.FirstOrDefault()?.MatchedObjectDescriptor;

                if (jobDetails == null)
                {
                    _logger.LogWarning("No job details found for position: {PositionId}", positionId);
                }

                return jobDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting job details for position: {PositionId}", positionId);
                throw;
            }
        }

        public async Task<bool> ValidateApiConnectionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var testRequest = new USAJobsSearchRequest
                {
                    Keyword = "test",
                    ResultsPerPage = 1
                };

                var response = await SearchJobsAsync(testRequest, cancellationToken);
                return response != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API connection validation failed");
                return false;
            }
        }

        private static string BuildSearchQueryString(USAJobsSearchRequest request)
        {
            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(request.Keyword))
                queryParams.Add($"Keyword={Uri.EscapeDataString(request.Keyword)}");

            if (!string.IsNullOrEmpty(request.LocationName))
                queryParams.Add($"LocationName={Uri.EscapeDataString(request.LocationName)}");

            if (request.Page.HasValue && request.Page > 0)
                queryParams.Add($"Page={request.Page}");

            if (request.ResultsPerPage.HasValue && request.ResultsPerPage > 0)
                queryParams.Add($"ResultsPerPage={request.ResultsPerPage}");

            if (!string.IsNullOrEmpty(request.SortField))
                queryParams.Add($"SortField={Uri.EscapeDataString(request.SortField)}");

            if (!string.IsNullOrEmpty(request.SortDirection))
                queryParams.Add($"SortDirection={Uri.EscapeDataString(request.SortDirection)}");

            if (!string.IsNullOrEmpty(request.Organization))
                queryParams.Add($"Organization={Uri.EscapeDataString(request.Organization)}");

            if (!string.IsNullOrEmpty(request.PayGradeHigh))
                queryParams.Add($"PayGradeHigh={Uri.EscapeDataString(request.PayGradeHigh)}");

            if (!string.IsNullOrEmpty(request.PayGradeLow))
                queryParams.Add($"PayGradeLow={Uri.EscapeDataString(request.PayGradeLow)}");

            if (!string.IsNullOrEmpty(request.PositionScheduleTypeCode))
                queryParams.Add($"PositionScheduleTypeCode={Uri.EscapeDataString(request.PositionScheduleTypeCode)}");

            if (!string.IsNullOrEmpty(request.PositionOfferingTypeCode))
                queryParams.Add($"PositionOfferingTypeCode={Uri.EscapeDataString(request.PositionOfferingTypeCode)}");

            if (request.DatePosted.HasValue)
                queryParams.Add($"DatePosted={request.DatePosted.Value:yyyy-MM-dd}");

            return string.Join("&", queryParams);
        }
    }
}