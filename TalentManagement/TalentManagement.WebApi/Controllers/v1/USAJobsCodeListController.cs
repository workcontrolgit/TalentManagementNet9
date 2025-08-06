using Microsoft.AspNetCore.Mvc;
using TalentManagement.Application.DTOs.External.USAJobs.CodeLists;
using TalentManagement.Application.Interfaces.External;
using TalentManagement.Application.Wrappers;
using TalentManagement.WebApi.Controllers;

namespace TalentManagement.WebApi.Controllers.v1
{
    /// <summary>
    /// Controller for accessing USAJobs code lists
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class USAJobsCodeListController : BaseApiController
    {
        private readonly IUSAJobsCodeListService _codeListService;
        private readonly ILogger<USAJobsCodeListController> _logger;

        public USAJobsCodeListController(
            IUSAJobsCodeListService codeListService,
            ILogger<USAJobsCodeListController> logger)
        {
            _codeListService = codeListService;
            _logger = logger;
        }

        /// <summary>
        /// Get all occupational series codes
        /// </summary>
        /// <returns>List of occupational series</returns>
        [HttpGet("occupational-series")]
        [ProducesResponseType(typeof(Response<List<OccupationalSeriesItem>>), 200)]
        [ProducesResponseType(typeof(Response<string>), 500)]
        public async Task<IActionResult> GetOccupationalSeries(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetOccupationalSeriesAsync(cancellationToken);
                
                if (result == null)
                {
                    return StatusCode(500, new Response<string>("Failed to retrieve occupational series"));
                }

                return Ok(new Response<List<OccupationalSeriesItem>>(result, 
                    $"Retrieved {result.Count} occupational series"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving occupational series");
                return StatusCode(500, new Response<string>("An error occurred while retrieving occupational series"));
            }
        }

        /// <summary>
        /// Search occupational series by keyword
        /// </summary>
        /// <param name="keyword">Search keyword</param>
        /// <returns>Filtered list of occupational series</returns>
        [HttpGet("occupational-series/search")]
        [ProducesResponseType(typeof(Response<List<OccupationalSeriesItem>>), 200)]
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<string>), 500)]
        public async Task<IActionResult> SearchOccupationalSeries(
            [FromQuery] string keyword, 
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new Response<string>("Keyword is required"));
            }

            try
            {
                var result = await _codeListService.SearchOccupationalSeriesAsync(keyword, cancellationToken);
                
                if (result == null)
                {
                    return StatusCode(500, new Response<string>("Failed to search occupational series"));
                }

                return Ok(new Response<List<OccupationalSeriesItem>>(result, 
                    $"Found {result.Count} occupational series matching '{keyword}'"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching occupational series with keyword: {Keyword}", keyword);
                return StatusCode(500, new Response<string>("An error occurred while searching occupational series"));
            }
        }

        /// <summary>
        /// Get a specific occupational series by code
        /// </summary>
        /// <param name="code">Occupational series code</param>
        /// <returns>Occupational series item</returns>
        [HttpGet("occupational-series/{code}")]
        [ProducesResponseType(typeof(Response<OccupationalSeriesItem>), 200)]
        [ProducesResponseType(typeof(Response<string>), 404)]
        [ProducesResponseType(typeof(Response<string>), 500)]
        public async Task<IActionResult> GetOccupationalSeriesByCode(
            string code, 
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetOccupationalSeriesByCodeAsync(code, cancellationToken);
                
                if (result == null)
                {
                    return NotFound(new Response<string>($"Occupational series with code '{code}' not found"));
                }

                return Ok(new Response<OccupationalSeriesItem>(result, 
                    $"Retrieved occupational series: {code}"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving occupational series by code: {Code}", code);
                return StatusCode(500, new Response<string>("An error occurred while retrieving occupational series"));
            }
        }

        /// <summary>
        /// Get all pay plan codes
        /// </summary>
        /// <returns>List of pay plans</returns>
        [HttpGet("pay-plans")]
        [ProducesResponseType(typeof(Response<List<PayPlanItem>>), 200)]
        [ProducesResponseType(typeof(Response<string>), 500)]
        public async Task<IActionResult> GetPayPlans(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetPayPlansAsync(cancellationToken);
                
                if (result == null)
                {
                    return StatusCode(500, new Response<string>("Failed to retrieve pay plans"));
                }

                return Ok(new Response<List<PayPlanItem>>(result, 
                    $"Retrieved {result.Count} pay plans"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pay plans");
                return StatusCode(500, new Response<string>("An error occurred while retrieving pay plans"));
            }
        }

        /// <summary>
        /// Get all hiring path codes
        /// </summary>
        /// <returns>List of hiring paths</returns>
        [HttpGet("hiring-paths")]
        [ProducesResponseType(typeof(Response<List<HiringPathItem>>), 200)]
        [ProducesResponseType(typeof(Response<string>), 500)]
        public async Task<IActionResult> GetHiringPaths(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetHiringPathsAsync(cancellationToken);
                
                if (result == null)
                {
                    return StatusCode(500, new Response<string>("Failed to retrieve hiring paths"));
                }

                return Ok(new Response<List<HiringPathItem>>(result, 
                    $"Retrieved {result.Count} hiring paths"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving hiring paths");
                return StatusCode(500, new Response<string>("An error occurred while retrieving hiring paths"));
            }
        }

        /// <summary>
        /// Get all position schedule type codes
        /// </summary>
        /// <returns>List of position schedule types</returns>
        [HttpGet("position-schedule-types")]
        [ProducesResponseType(typeof(Response<List<PositionScheduleTypeItem>>), 200)]
        [ProducesResponseType(typeof(Response<string>), 500)]
        public async Task<IActionResult> GetPositionScheduleTypes(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetPositionScheduleTypesAsync(cancellationToken);
                
                if (result == null)
                {
                    return StatusCode(500, new Response<string>("Failed to retrieve position schedule types"));
                }

                return Ok(new Response<List<PositionScheduleTypeItem>>(result, 
                    $"Retrieved {result.Count} position schedule types"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving position schedule types");
                return StatusCode(500, new Response<string>("An error occurred while retrieving position schedule types"));
            }
        }


        /// <summary>
        /// Get all security clearance codes
        /// </summary>
        /// <returns>List of security clearances</returns>
        [HttpGet("security-clearances")]
        [ProducesResponseType(typeof(Response<List<SecurityClearanceItem>>), 200)]
        [ProducesResponseType(typeof(Response<string>), 500)]
        public async Task<IActionResult> GetSecurityClearances(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetSecurityClearancesAsync(cancellationToken);
                
                if (result == null)
                {
                    return StatusCode(500, new Response<string>("Failed to retrieve security clearances"));
                }

                return Ok(new Response<List<SecurityClearanceItem>>(result, 
                    $"Retrieved {result.Count} security clearances"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving security clearances");
                return StatusCode(500, new Response<string>("An error occurred while retrieving security clearances"));
            }
        }

        /// <summary>
        /// Get all country codes
        /// </summary>
        /// <returns>List of countries</returns>
        [HttpGet("countries")]
        [ProducesResponseType(typeof(Response<List<CountryItem>>), 200)]
        [ProducesResponseType(typeof(Response<string>), 500)]
        public async Task<IActionResult> GetCountries(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetCountriesAsync(cancellationToken);
                
                if (result == null)
                {
                    return StatusCode(500, new Response<string>("Failed to retrieve countries"));
                }

                return Ok(new Response<List<CountryItem>>(result, 
                    $"Retrieved {result.Count} countries"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving countries");
                return StatusCode(500, new Response<string>("An error occurred while retrieving countries"));
            }
        }

        /// <summary>
        /// Get all postal code (state) codes
        /// </summary>
        /// <returns>List of postal codes</returns>
        [HttpGet("postal-codes")]
        [ProducesResponseType(typeof(Response<List<PostalCodeItem>>), 200)]
        [ProducesResponseType(typeof(Response<string>), 500)]
        public async Task<IActionResult> GetPostalCodes(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetPostalCodesAsync(cancellationToken);
                
                if (result == null)
                {
                    return StatusCode(500, new Response<string>("Failed to retrieve postal codes"));
                }

                return Ok(new Response<List<PostalCodeItem>>(result, 
                    $"Retrieved {result.Count} postal codes"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving postal codes");
                return StatusCode(500, new Response<string>("An error occurred while retrieving postal codes"));
            }
        }

        [HttpGet("agency-subelements")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetAgencySubelements(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetAgencySubelementsAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} agency subelements"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving agency subelements");
                return StatusCode(500, new Response<string>("An error occurred while retrieving agency subelements"));
            }
        }

        [HttpGet("gsa-geo-location-codes")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetGsaGeoLocationCodes(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetGsaGeoLocationCodesAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} GSA geo location codes"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GSA geo location codes");
                return StatusCode(500, new Response<string>("An error occurred while retrieving GSA geo location codes"));
            }
        }

        /// <summary>
        /// Refresh all code lists from the USAJobs API
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [ProducesResponseType(typeof(Response<string>), 500)]
        public async Task<IActionResult> RefreshCodeLists(CancellationToken cancellationToken)
        {
            try
            {
                await _codeListService.RefreshAllCodeListsAsync(cancellationToken);
                return Ok(new Response<string>("All code lists have been refreshed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing code lists");
                return StatusCode(500, new Response<string>("An error occurred while refreshing code lists"));
            }
        }

        /// <summary>
        /// Check if the USAJobs code list service is available
        /// </summary>
        /// <returns>Service availability status</returns>
        [HttpGet("health")]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        public async Task<IActionResult> CheckServiceHealth(CancellationToken cancellationToken)
        {
            try
            {
                var isAvailable = await _codeListService.IsServiceAvailableAsync(cancellationToken);
                var message = isAvailable ? "USAJobs code list service is available" : "USAJobs code list service is unavailable";
                
                return Ok(new Response<bool>(isAvailable, message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking code list service health");
                return Ok(new Response<bool>(false, "Error checking service health"));
            }
        }
    }
}