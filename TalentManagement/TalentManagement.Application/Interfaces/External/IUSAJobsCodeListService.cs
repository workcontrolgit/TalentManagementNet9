using TalentManagement.Application.DTOs.External.USAJobs.CodeLists;

namespace TalentManagement.Application.Interfaces.External
{
    /// <summary>
    /// Service interface for accessing USAJobs code lists
    /// </summary>
    public interface IUSAJobsCodeListService
    {
        /// <summary>
        /// Get all occupational series codes
        /// </summary>
        Task<List<OccupationalSeriesItem>?> GetOccupationalSeriesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all pay plan codes
        /// </summary>
        Task<List<PayPlanItem>?> GetPayPlansAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all hiring path codes
        /// </summary>
        Task<List<HiringPathItem>?> GetHiringPathsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all position schedule type codes
        /// </summary>
        Task<List<PositionScheduleTypeItem>?> GetPositionScheduleTypesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all work schedule codes
        /// </summary>
        Task<List<WorkScheduleItem>?> GetWorkSchedulesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all security clearance codes
        /// </summary>
        Task<List<SecurityClearanceItem>?> GetSecurityClearancesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all country codes
        /// </summary>
        Task<List<CountryItem>?> GetCountriesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all postal code (state) codes
        /// </summary>
        Task<List<PostalCodeItem>?> GetPostalCodesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all geographic location codes
        /// </summary>
        Task<List<GeoLocationItem>?> GetGeoLocationsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all travel requirement codes
        /// </summary>
        Task<List<TravelRequirementItem>?> GetTravelRequirementsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all remote work codes
        /// </summary>
        Task<List<RemoteWorkItem>?> GetRemoteWorkOptionsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a specific occupational series by code
        /// </summary>
        Task<OccupationalSeriesItem?> GetOccupationalSeriesByCodeAsync(string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a specific pay plan by code
        /// </summary>
        Task<PayPlanItem?> GetPayPlanByCodeAsync(string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Search occupational series by keyword
        /// </summary>
        Task<List<OccupationalSeriesItem>?> SearchOccupationalSeriesAsync(string keyword, CancellationToken cancellationToken = default);

        /// <summary>
        /// Refresh all code lists from the API (cache invalidation)
        /// </summary>
        Task RefreshAllCodeListsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if the code list service is available
        /// </summary>
        Task<bool> IsServiceAvailableAsync(CancellationToken cancellationToken = default);
    }
}