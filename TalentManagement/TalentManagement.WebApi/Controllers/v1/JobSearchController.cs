using Microsoft.AspNetCore.Mvc;
using TalentManagement.Application.DTOs.JobAggregation;
using TalentManagement.Application.Interfaces;
using TalentManagement.Application.Wrappers;

namespace TalentManagement.WebApi.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class JobSearchController : BaseApiController
    {
        private readonly IJobAggregationService _jobAggregationService;
        private readonly ILogger<JobSearchController> _logger;

        public JobSearchController(
            IJobAggregationService jobAggregationService,
            ILogger<JobSearchController> logger)
        {
            _jobAggregationService = jobAggregationService;
            _logger = logger;
        }

        /// <summary>
        /// Search for jobs across multiple sources (internal and external)
        /// </summary>
        /// <param name="request">Job search parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Aggregated job search results</returns>
        [HttpPost("search")]
        [ProducesResponseType(typeof(Response<JobSearchResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchJobs(
            [FromBody] JobSearchRequest request, 
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Job search requested with keywords: {Keywords}, sources: {Sources}", 
                    request.Keywords, string.Join(", ", request.Sources));

                // Validate request
                if (request.PageSize > 100)
                {
                    return BadRequest(new Response<string>("Page size cannot exceed 100"));
                }

                if (request.Page < 1)
                {
                    return BadRequest(new Response<string>("Page number must be greater than 0"));
                }

                var result = await _jobAggregationService.SearchJobsAsync(request, cancellationToken);
                
                _logger.LogInformation("Job search completed. Found {TotalJobs} jobs in {Duration}ms", 
                    result.TotalCount, result.Metadata.SearchDuration.TotalMilliseconds);

                return Ok(new Response<JobSearchResult>(result, "Job search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during job search");
                return StatusCode(500, new Response<string>("An error occurred while searching for jobs"));
            }
        }

        /// <summary>
        /// Get detailed information about a specific job
        /// </summary>
        /// <param name="jobId">Job identifier</param>
        /// <param name="source">Job source (Internal, USAJobs, etc.)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Detailed job information</returns>
        [HttpGet("{jobId}")]
        [ProducesResponseType(typeof(Response<AggregatedJobListing>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetJobDetails(
            string jobId, 
            [FromQuery] JobSource source = JobSource.Internal,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting job details for {JobId} from {Source}", jobId, source);

                var job = await _jobAggregationService.GetJobDetailsAsync(jobId, source, cancellationToken);
                
                if (job == null)
                {
                    return NotFound(new Response<string>($"Job {jobId} not found in {source}"));
                }

                return Ok(new Response<AggregatedJobListing>(job, "Job details retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting job details for {JobId}", jobId);
                return StatusCode(500, new Response<string>("An error occurred while retrieving job details"));
            }
        }

        /// <summary>
        /// Find matching candidates for a specific job
        /// </summary>
        /// <param name="jobId">Job identifier</param>
        /// <param name="source">Job source</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of matching candidates with scores</returns>
        [HttpGet("{jobId}/matching-candidates")]
        [ProducesResponseType(typeof(Response<List<MatchingCandidate>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMatchingCandidates(
            string jobId,
            [FromQuery] JobSource source = JobSource.Internal,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Finding matching candidates for job {JobId} from {Source}", jobId, source);

                var candidates = await _jobAggregationService.FindMatchingCandidatesAsync(jobId, source, cancellationToken);
                
                return Ok(new Response<List<MatchingCandidate>>(candidates, 
                    $"Found {candidates.Count} matching candidates"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while finding matching candidates for job {JobId}", jobId);
                return StatusCode(500, new Response<string>("An error occurred while finding matching candidates"));
            }
        }

        /// <summary>
        /// Get recommended jobs for a specific employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="pageSize">Number of recommendations to return</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Recommended jobs for the employee</returns>
        [HttpGet("recommendations/{employeeId:guid}")]
        [ProducesResponseType(typeof(Response<JobSearchResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRecommendedJobs(
            Guid employeeId,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting job recommendations for employee {EmployeeId}", employeeId);

                if (pageSize > 50)
                {
                    return BadRequest(new Response<string>("Page size for recommendations cannot exceed 50"));
                }

                var recommendations = await _jobAggregationService.GetRecommendedJobsAsync(employeeId, pageSize, cancellationToken);
                
                return Ok(new Response<JobSearchResult>(recommendations, 
                    $"Found {recommendations.TotalCount} recommended jobs"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting job recommendations for employee {EmployeeId}", employeeId);
                return StatusCode(500, new Response<string>("An error occurred while getting job recommendations"));
            }
        }

        /// <summary>
        /// Get available job sources and their status
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Status of available job sources</returns>
        [HttpGet("sources/status")]
        [ProducesResponseType(typeof(Response<Dictionary<string, object>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSourceStatus(CancellationToken cancellationToken = default)
        {
            try
            {
                var status = new Dictionary<string, object>
                {
                    ["Internal"] = new { Available = true, LastUpdated = DateTime.UtcNow },
                    ["USAJobs"] = new { Available = true, LastUpdated = DateTime.UtcNow, RateLimit = "Healthy" }
                };

                return Ok(new Response<Dictionary<string, object>>(status, "Source status retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting source status");
                return StatusCode(500, new Response<string>("An error occurred while getting source status"));
            }
        }

        /// <summary>
        /// Search for jobs with GET method for simpler client integration
        /// </summary>
        /// <param name="keywords">Search keywords</param>
        /// <param name="location">Location filter</param>
        /// <param name="sources">Comma-separated list of sources (Internal,USAJobs)</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Results per page</param>
        /// <param name="sortBy">Sort field</param>
        /// <param name="sortDirection">Sort direction (asc/desc)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Job search results</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(Response<JobSearchResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchJobsGet(
            [FromQuery] string? keywords = null,
            [FromQuery] string? location = null,
            [FromQuery] string? sources = "Internal,USAJobs",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25,
            [FromQuery] string? sortBy = "date",
            [FromQuery] string? sortDirection = "desc",
            CancellationToken cancellationToken = default)
        {
            try
            {
                var sourcesEnum = new List<JobSource>();
                if (!string.IsNullOrEmpty(sources))
                {
                    foreach (var source in sources.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (Enum.TryParse<JobSource>(source.Trim(), true, out var sourceEnum))
                        {
                            sourcesEnum.Add(sourceEnum);
                        }
                    }
                }

                if (!sourcesEnum.Any())
                {
                    sourcesEnum.AddRange(new[] { JobSource.Internal, JobSource.USAJobs });
                }

                var request = new JobSearchRequest
                {
                    Keywords = keywords,
                    Location = location,
                    Sources = sourcesEnum,
                    Page = page,
                    PageSize = pageSize,
                    SortBy = sortBy,
                    SortDirection = sortDirection
                };

                return await SearchJobs(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during GET job search");
                return StatusCode(500, new Response<string>("An error occurred while searching for jobs"));
            }
        }
    }
}