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

        [HttpGet("geo-locations")]
        [ProducesResponseType(typeof(Response<List<GeoLocationItem>>), 200)]
        [ProducesResponseType(typeof(Response<string>), 500)]
        public async Task<IActionResult> GetGeoLocations(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetGeoLocationsAsync(cancellationToken);
                
                if (result == null)
                {
                    return StatusCode(500, new Response<string>("Failed to retrieve geo locations"));
                }

                return Ok(new Response<List<GeoLocationItem>>(result, 
                    $"Retrieved {result.Count} geo locations"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving geo locations");
                return StatusCode(500, new Response<string>("An error occurred while retrieving geo locations"));
            }
        }

        [HttpGet("travel-requirements")]
        [ProducesResponseType(typeof(Response<List<TravelRequirementItem>>), 200)]
        [ProducesResponseType(typeof(Response<string>), 500)]
        public async Task<IActionResult> GetTravelRequirements(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetTravelRequirementsAsync(cancellationToken);
                
                if (result == null)
                {
                    return StatusCode(500, new Response<string>("Failed to retrieve travel requirements"));
                }

                return Ok(new Response<List<TravelRequirementItem>>(result, 
                    $"Retrieved {result.Count} travel requirements"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving travel requirements");
                return StatusCode(500, new Response<string>("An error occurred while retrieving travel requirements"));
            }
        }

        [HttpGet("remote-work-options")]
        [ProducesResponseType(typeof(Response<List<RemoteWorkItem>>), 200)]
        [ProducesResponseType(typeof(Response<string>), 500)]
        public async Task<IActionResult> GetRemoteWorkOptions(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetRemoteWorkOptionsAsync(cancellationToken);
                
                if (result == null)
                {
                    return StatusCode(500, new Response<string>("Failed to retrieve remote work options"));
                }

                return Ok(new Response<List<RemoteWorkItem>>(result, 
                    $"Retrieved {result.Count} remote work options"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving remote work options");
                return StatusCode(500, new Response<string>("An error occurred while retrieving remote work options"));
            }
        }

        [HttpGet("country-subdivisions")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetCountrySubdivisions(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetCountrySubdivisionsAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} country subdivisions"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving country subdivisions");
                return StatusCode(500, new Response<string>("An error occurred while retrieving country subdivisions"));
            }
        }

        [HttpGet("travel-percentages")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetTravelPercentages(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetTravelPercentagesAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} travel percentages"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving travel percentages");
                return StatusCode(500, new Response<string>("An error occurred while retrieving travel percentages"));
            }
        }

        [HttpGet("position-offering-types")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetPositionOfferingTypes(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetPositionOfferingTypesAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} position offering types"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving position offering types");
                return StatusCode(500, new Response<string>("An error occurred while retrieving position offering types"));
            }
        }

        [HttpGet("who-may-apply")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetWhoMayApply(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetWhoMayApplyAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} who may apply codes"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving who may apply codes");
                return StatusCode(500, new Response<string>("An error occurred while retrieving who may apply codes"));
            }
        }

        [HttpGet("academic-honors")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetAcademicHonors(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetAcademicHonorsAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} academic honors"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving academic honors");
                return StatusCode(500, new Response<string>("An error occurred while retrieving academic honors"));
            }
        }

        [HttpGet("action-codes")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetActionCodes(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetActionCodesAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} action codes"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving action codes");
                return StatusCode(500, new Response<string>("An error occurred while retrieving action codes"));
            }
        }

        [HttpGet("degree-type-codes")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetDegreeTypeCodes(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetDegreeTypeCodesAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} degree type codes"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving degree type codes");
                return StatusCode(500, new Response<string>("An error occurred while retrieving degree type codes"));
            }
        }

        [HttpGet("document-formats")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetDocumentFormats(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetDocumentFormatsAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} document formats"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document formats");
                return StatusCode(500, new Response<string>("An error occurred while retrieving document formats"));
            }
        }

        [HttpGet("race-codes")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetRaceCodes(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetRaceCodesAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} race codes"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving race codes");
                return StatusCode(500, new Response<string>("An error occurred while retrieving race codes"));
            }
        }

        [HttpGet("ethnicities")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetEthnicities(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetEthnicitiesAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} ethnicities"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ethnicities");
                return StatusCode(500, new Response<string>("An error occurred while retrieving ethnicities"));
            }
        }

        [HttpGet("documentations")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetDocumentations(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetDocumentationsAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} documentations"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving documentations");
                return StatusCode(500, new Response<string>("An error occurred while retrieving documentations"));
            }
        }

        [HttpGet("federal-employment-statuses")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetFederalEmploymentStatuses(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetFederalEmploymentStatusesAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} federal employment statuses"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving federal employment statuses");
                return StatusCode(500, new Response<string>("An error occurred while retrieving federal employment statuses"));
            }
        }

        [HttpGet("language-proficiencies")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetLanguageProficiencies(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetLanguageProficienciesAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} language proficiencies"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving language proficiencies");
                return StatusCode(500, new Response<string>("An error occurred while retrieving language proficiencies"));
            }
        }

        [HttpGet("language-codes")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetLanguageCodes(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetLanguageCodesAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} language codes"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving language codes");
                return StatusCode(500, new Response<string>("An error occurred while retrieving language codes"));
            }
        }

        [HttpGet("military-status-codes")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetMilitaryStatusCodes(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetMilitaryStatusCodesAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} military status codes"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving military status codes");
                return StatusCode(500, new Response<string>("An error occurred while retrieving military status codes"));
            }
        }

        [HttpGet("referee-type-codes")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetRefereeTypeCodes(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetRefereeTypeCodesAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} referee type codes"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving referee type codes");
                return StatusCode(500, new Response<string>("An error occurred while retrieving referee type codes"));
            }
        }

        [HttpGet("special-hirings")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetSpecialHirings(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetSpecialHiringsAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} special hirings"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving special hirings");
                return StatusCode(500, new Response<string>("An error occurred while retrieving special hirings"));
            }
        }

        [HttpGet("remuneration-rate-interval-codes")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetRemunerationRateIntervalCodes(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetRemunerationRateIntervalCodesAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} remuneration rate interval codes"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving remuneration rate interval codes");
                return StatusCode(500, new Response<string>("An error occurred while retrieving remuneration rate interval codes"));
            }
        }

        [HttpGet("application-statuses")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetApplicationStatuses(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetApplicationStatusesAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} application statuses"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving application statuses");
                return StatusCode(500, new Response<string>("An error occurred while retrieving application statuses"));
            }
        }

        [HttpGet("academic-levels")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetAcademicLevels(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetAcademicLevelsAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} academic levels"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving academic levels");
                return StatusCode(500, new Response<string>("An error occurred while retrieving academic levels"));
            }
        }

        [HttpGet("key-standard-requirements")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetKeyStandardRequirements(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetKeyStandardRequirementsAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} key standard requirements"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving key standard requirements");
                return StatusCode(500, new Response<string>("An error occurred while retrieving key standard requirements"));
            }
        }

        [HttpGet("required-standard-documents")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetRequiredStandardDocuments(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetRequiredStandardDocumentsAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} required standard documents"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving required standard documents");
                return StatusCode(500, new Response<string>("An error occurred while retrieving required standard documents"));
            }
        }

        [HttpGet("disabilities")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetDisabilities(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetDisabilitiesAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} disabilities"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving disabilities");
                return StatusCode(500, new Response<string>("An error occurred while retrieving disabilities"));
            }
        }

        [HttpGet("applicant-suppliers")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetApplicantSuppliers(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetApplicantSuppliersAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} applicant suppliers"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving applicant suppliers");
                return StatusCode(500, new Response<string>("An error occurred while retrieving applicant suppliers"));
            }
        }

        [HttpGet("mission-critical-codes")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetMissionCriticalCodes(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetMissionCriticalCodesAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} mission critical codes"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mission critical codes");
                return StatusCode(500, new Response<string>("An error occurred while retrieving mission critical codes"));
            }
        }

        [HttpGet("announcement-closing-types")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetAnnouncementClosingTypes(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetAnnouncementClosingTypesAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} announcement closing types"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving announcement closing types");
                return StatusCode(500, new Response<string>("An error occurred while retrieving announcement closing types"));
            }
        }

        [HttpGet("service-types")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetServiceTypes(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetServiceTypesAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} service types"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving service types");
                return StatusCode(500, new Response<string>("An error occurred while retrieving service types"));
            }
        }

        [HttpGet("location-expansions")]
        [ProducesResponseType(typeof(Response<List<BaseCodeListItem>>), 200)]
        public async Task<IActionResult> GetLocationExpansions(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetLocationExpansionsAsync(cancellationToken);
                return Ok(new Response<List<BaseCodeListItem>>(result ?? new List<BaseCodeListItem>(), 
                    $"Retrieved {result?.Count ?? 0} location expansions"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving location expansions");
                return StatusCode(500, new Response<string>("An error occurred while retrieving location expansions"));
            }
        }

        [HttpGet("pay-plans/{code}")]
        [ProducesResponseType(typeof(Response<PayPlanItem>), 200)]
        [ProducesResponseType(typeof(Response<string>), 404)]
        [ProducesResponseType(typeof(Response<string>), 500)]
        public async Task<IActionResult> GetPayPlanByCode(
            string code, 
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _codeListService.GetPayPlanByCodeAsync(code, cancellationToken);
                
                if (result == null)
                {
                    return NotFound(new Response<string>($"Pay plan with code '{code}' not found"));
                }

                return Ok(new Response<PayPlanItem>(result, 
                    $"Retrieved pay plan: {code}"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pay plan by code: {Code}", code);
                return StatusCode(500, new Response<string>("An error occurred while retrieving pay plan"));
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