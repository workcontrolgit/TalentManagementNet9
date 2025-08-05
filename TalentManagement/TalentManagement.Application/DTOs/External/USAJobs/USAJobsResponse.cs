using System.Text.Json.Serialization;

namespace TalentManagement.Application.DTOs.External.USAJobs
{
    public class USAJobsResponse
    {
        public string? LanguageCode { get; set; }
        public SearchParameters? SearchParameters { get; set; }
        public SearchResult? SearchResult { get; set; }
    }

    public class SearchParameters
    {
        public string? WhoMayApply { get; set; }
        public string? ResultsPerPage { get; set; }
        public string? SortField { get; set; }
        public string? SortDirection { get; set; }
        public string? Page { get; set; }
        public string? Keyword { get; set; }
        public string? LocationName { get; set; }
    }

    public partial class SearchResult
    {
        [JsonPropertyName("SearchResultCount")]
        public long SearchResultCount { get; set; }

        [JsonPropertyName("SearchResultCountAll")]
        public long SearchResultCountAll { get; set; }

        [JsonPropertyName("SearchResultItems")]
        public SearchResultItem[] SearchResultItems { get; set; }


    }



    public class SearchResultItem
    {
        public int? RelevanceRank { get; set; }
        public MatchedObjectDescriptor? MatchedObjectDescriptor { get; set; }
        public string? MatchedObjectId { get; set; }
    }

    public class MatchedObjectDescriptor
    {
        public string? PositionID { get; set; }
        public string? PositionTitle { get; set; }
        public string? PositionURI { get; set; }
        public List<string>? ApplyURI { get; set; }
        public string? PositionLocationDisplay { get; set; }
        public List<PositionLocation>? PositionLocation { get; set; }
        public string? OrganizationName { get; set; }
        public string? DepartmentName { get; set; }
        public string? SubAgency { get; set; }
        public List<JobCategory>? JobCategory { get; set; }
        public List<JobGrade>? JobGrade { get; set; }
        public List<PositionSchedule>? PositionSchedule { get; set; }
        public List<PositionOfferingType>? PositionOfferingType { get; set; }

        public string? QualificationSummary { get; set; }

        public List<PositionRemuneration>? PositionRemuneration { get; set; }
        public string? PositionStartDate { get; set; }
        public string? PositionEndDate { get; set; }
        public string? PublicationStartDate { get; set; }
        public string? ApplicationCloseDate { get; set; }
        public List<PositionFormattedDescription>? PositionFormattedDescription { get; set; }
        public UserArea? UserArea { get; set; }
    }

    public class PositionLocation
    {
        public string? LocationName { get; set; }
        public string? CountryCode { get; set; }
        public string? CountrySubDivisionCode { get; set; }
        public string? CityName { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
    }

    public class JobGrade
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
    }

    public class JobCategory
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
    }

    public class PositionSchedule
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
    }

    public class PositionOfferingType
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
    }

    public class PositionFormattedDescription
    {
        public string? Label { get; set; }
        public string? LabelDescription { get; set; }
    }


    public class PositionRemuneration
    {
        public string? MinimumRange { get; set; }
        public string? MaximumRange { get; set; }
        public string? RateIntervalCode { get; set; }
        public string? Description { get; set; }
    }

    public class UserArea
    {
        public Details? Details { get; set; }
        public bool? IsRadialSearch { get; set; }
        public string? NumberOfPages { get; set; }
    }

    public class Details
    {
        public string? JobSummary { get; set; }
        public WhoMayApply? WhoMayApply { get; set; }
        public string? LowGrade { get; set; }
        public string? HighGrade { get; set; }
        public string? PromotionPotential { get; set; }
        public string? SubAgencyName { get; set; }
        public string? OrganizationCodes { get; set; }
        public string? Relocation { get; set; }
        public List<string>? HiringPath { get; set; }
        public List<object>? MCOTags { get; set; }
        public string? TotalOpenings { get; set; }
        public string? AgencyMarketingStatement { get; set; }
        public string? TravelCode { get; set; }
        public string? ApplyOnlineUrl { get; set; }
        public string? DetailStatusUrl { get; set; }
        public List<string>? MajorDuties { get; set; }
        public string? Education { get; set; }
        public string? Requirements { get; set; }
        public string? Evaluations { get; set; }
        public string? HowToApply { get; set; }
        public string? WhatToExpectNext { get; set; }
        public string? RequiredDocuments { get; set; }
        public string? Benefits { get; set; }
        public string? BenefitsUrl { get; set; }
        public bool? BenefitsDisplayDefaultText { get; set; }
        public string? OtherInformation { get; set; }
        public List<object>? KeyRequirements { get; set; }
        public string? WithinArea { get; set; }
        public string? CommuteDistance { get; set; }
        public string? ServiceType { get; set; }
        public string? AnnouncementClosingType { get; set; }
        public string? AgencyContactEmail { get; set; }
        public string? AgencyContactPhone { get; set; }
        public string? PreviewQuestionnaireurl { get; set; }
        public string? SecurityClearance { get; set; }
        public string? DrugTestRequired { get; set; }
        public List<object>? AdjudicationType { get; set; }
        public bool? TeleworkEligible { get; set; }
        public bool? RemoteIndicator { get; set; }
        public bool? BargainingUnitStatus { get; set; }
        public string? BargainingUnitStatusAdditionalText { get; set; }
    }

    public class WhoMayApply
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
    }
}