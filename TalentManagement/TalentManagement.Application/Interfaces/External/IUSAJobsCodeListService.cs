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

        // Additional code lists
        Task<List<BaseCodeListItem>?> GetAgencySubelementsAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetGsaGeoLocationCodesAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetCountrySubdivisionsAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetTravelPercentagesAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetPositionOfferingTypesAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetWhoMayApplyAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetAcademicHonorsAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetActionCodesAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetDegreeTypeCodesAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetDocumentFormatsAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetRaceCodesAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetEthnicitiesAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetDocumentationsAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetFederalEmploymentStatusesAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetLanguageProficienciesAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetLanguageCodesAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetMilitaryStatusCodesAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetRefereeTypeCodesAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetSpecialHiringsAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetRemunerationRateIntervalCodesAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetApplicationStatusesAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetAcademicLevelsAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetKeyStandardRequirementsAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetRequiredStandardDocumentsAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetDisabilitiesAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetApplicantSuppliersAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetMissionCriticalCodesAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetAnnouncementClosingTypesAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetServiceTypesAsync(CancellationToken cancellationToken = default);
        Task<List<BaseCodeListItem>?> GetLocationExpansionsAsync(CancellationToken cancellationToken = default);

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