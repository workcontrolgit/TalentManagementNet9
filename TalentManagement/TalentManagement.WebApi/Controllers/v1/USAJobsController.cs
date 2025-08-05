using Microsoft.AspNetCore.Mvc;
using TalentManagement.Application.DTOs.External.USAJobs;
using TalentManagement.Application.Interfaces.External;
using TalentManagement.Application.Wrappers;

namespace TalentManagement.WebApi.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class USAJobsController : BaseApiController
    {
        private readonly IUSAJobsService _usaJobsService;
        private readonly ILogger<USAJobsController> _logger;

        public USAJobsController(
            IUSAJobsService usaJobsService,
            ILogger<USAJobsController> logger)
        {
            _usaJobsService = usaJobsService;
            _logger = logger;
        }

        /// <summary>
        /// Search for jobs from USAJobs API
        /// </summary>
        /// <param name="request">Search parameters for USAJobs</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>USAJobs search results</returns>
        [HttpPost("search")]
        [ProducesResponseType(typeof(Response<USAJobsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchJobs(
            [FromBody] USAJobsSearchRequest request, 
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("USAJobs search requested with keywords: {Keywords}", request.Keyword);

                // Validate request
                if (request.ResultsPerPage > 500)
                {
                    return BadRequest(new Response<string>("Results per page cannot exceed 500"));
                }

                if (request.Page < 1)
                {
                    return BadRequest(new Response<string>("Page number must be greater than 0"));
                }

                var result = await _usaJobsService.SearchJobsAsync(request, cancellationToken);
                
                if (result == null)
                {
                    return StatusCode(500, new Response<string>("No results returned from USAJobs API"));
                }

                _logger.LogInformation("USAJobs search completed. Found {Count} jobs", 
                    result.SearchResult?.SearchResultCount ?? 0);

                return Ok(new Response<USAJobsResponse>(result, "USAJobs search completed successfully"));
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error occurred while calling USAJobs API");
                return StatusCode(503, new Response<string>("USAJobs API is currently unavailable"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during USAJobs search");
                return StatusCode(500, new Response<string>("An error occurred while searching USAJobs"));
            }
        }

        /// <summary>
        /// Get detailed information about a specific USAJobs posting
        /// </summary>
        /// <param name="positionId">USAJobs position ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Detailed job information from USAJobs</returns>
        [HttpGet("{positionId}")]
        [ProducesResponseType(typeof(Response<MatchedObjectDescriptor>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetJobDetails(
            string positionId, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting USAJobs details for position: {PositionId}", positionId);

                var job = await _usaJobsService.GetJobDetailsAsync(positionId, cancellationToken);
                
                if (job == null)
                {
                    return NotFound(new Response<string>($"USAJobs position {positionId} not found"));
                }

                return Ok(new Response<MatchedObjectDescriptor>(job, "Job details retrieved successfully"));
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error occurred while getting USAJobs details for {PositionId}", positionId);
                return StatusCode(503, new Response<string>("USAJobs API is currently unavailable"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting job details for {PositionId}", positionId);
                return StatusCode(500, new Response<string>("An error occurred while retrieving job details"));
            }
        }

        /// <summary>
        /// Test connectivity to USAJobs API
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>API connection status</returns>
        [HttpGet("health")]
        [ProducesResponseType(typeof(Response<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> CheckHealth(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Checking USAJobs API health");

                var isHealthy = await _usaJobsService.ValidateApiConnectionAsync(cancellationToken);
                
                if (isHealthy)
                {
                    return Ok(new Response<object>(new { 
                        Status = "Healthy", 
                        Timestamp = DateTime.UtcNow,
                        Service = "USAJobs API"
                    }, "USAJobs API is accessible"));
                }
                else
                {
                    return StatusCode(503, new Response<string>("USAJobs API is not accessible"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking USAJobs API health");
                return StatusCode(503, new Response<string>("USAJobs API health check failed"));
            }
        }

        /// <summary>
        /// Search for USAJobs with GET method for simple integration
        /// </summary>
        /// <param name="keyword">Search keyword</param>
        /// <param name="locationName">Location filter</param>
        /// <param name="page">Page number</param>
        /// <param name="resultsPerPage">Results per page</param>
        /// <param name="sortField">Sort field</param>
        /// <param name="sortDirection">Sort direction</param>
        /// <param name="organization">Organization filter</param>
        /// <param name="payGradeHigh">High pay grade</param>
        /// <param name="payGradeLow">Low pay grade</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>USAJobs search results</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(Response<USAJobsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchJobsGet(
            [FromQuery] string? keyword = null,
            [FromQuery] string? locationName = null,
            [FromQuery] int page = 1,
            [FromQuery] int resultsPerPage = 25,
            [FromQuery] string? sortField = null,
            [FromQuery] string? sortDirection = null,
            [FromQuery] string? organization = null,
            [FromQuery] string? payGradeHigh = null,
            [FromQuery] string? payGradeLow = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new USAJobsSearchRequest
                {
                    Keyword = keyword,
                    LocationName = locationName,
                    Page = page,
                    ResultsPerPage = resultsPerPage,
                    SortField = sortField,
                    SortDirection = sortDirection,
                    Organization = organization,
                    PayGradeHigh = payGradeHigh,
                    PayGradeLow = payGradeLow
                };

                return await SearchJobs(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during GET USAJobs search");
                return StatusCode(500, new Response<string>("An error occurred while searching USAJobs"));
            }
        }

        /// <summary>
        /// Get information about USAJobs API usage and limits
        /// </summary>
        /// <returns>API usage information</returns>
        [HttpGet("info")]
        [ProducesResponseType(typeof(Response<object>), StatusCodes.Status200OK)]
        public IActionResult GetApiInfo()
        {
            var apiInfo = new
            {
                ServiceName = "USAJobs API Integration",
                Version = "1.0",
                BaseUrl = "https://data.usajobs.gov/api",
                RateLimit = new
                {
                    RequestsPerHour = 400,
                    RequestsPerDay = 5000
                },
                Documentation = "https://developer.usajobs.gov/",
                SupportedOperations = new[]
                {
                    "Job Search",
                    "Job Details Lookup",
                    "Health Check"
                }
            };

            return Ok(new Response<object>(apiInfo, "USAJobs API information retrieved"));
        }
    }
}