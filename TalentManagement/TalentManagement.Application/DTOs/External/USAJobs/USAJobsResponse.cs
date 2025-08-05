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

    public class SearchResult
    {
        public string? SearchResultCount { get; set; }
        public string? SearchResultCountAll { get; set; }
        public List<SearchResultItem>? SearchResultItems { get; set; }
        public UserArea? UserArea { get; set; }
    }

    public class SearchResultItem
    {
        public string? RelevanceRank { get; set; }
        public MatchedObjectDescriptor? MatchedObjectDescriptor { get; set; }
        public string? MatchedObjectId { get; set; }
    }

    public class MatchedObjectDescriptor
    {
        public string? PositionID { get; set; }
        public string? PositionTitle { get; set; }
        public string? PositionURI { get; set; }
        public DateTime? ApplyURI { get; set; }
        public string? PositionLocationDisplay { get; set; }
        public List<PositionLocation>? PositionLocation { get; set; }
        public string? OrganizationName { get; set; }
        public string? DepartmentName { get; set; }
        public string? SubAgency { get; set; }
        public string? JobCategory { get; set; }
        public List<JobGrade>? JobGrade { get; set; }
        public PositionSchedule? PositionSchedule { get; set; }
        public PositionOfferingType? PositionOfferingType { get; set; }
        public QualificationSummary? QualificationSummary { get; set; }
        public PositionRemuneration? PositionRemuneration { get; set; }
        public DateTime? PositionStartDate { get; set; }
        public DateTime? PositionEndDate { get; set; }
        public DateTime? PublicationStartDate { get; set; }
        public DateTime? ApplicationCloseDate { get; set; }
        public string? PositionFormattedDescription { get; set; }
        public UserArea? UserArea { get; set; }
    }

    public class PositionLocation
    {
        public string? LocationName { get; set; }
        public string? CountryCode { get; set; }
        public string? CountrySubDivisionCode { get; set; }
        public string? CityName { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
    }

    public class JobGrade
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
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

    public class QualificationSummary
    {
        public string? QualificationSummaryText { get; set; }
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
    }

    public class Details
    {
        public string? JobSummary { get; set; }
        public WhoMayApply? WhoMayApply { get; set; }
        public string? LowGrade { get; set; }
        public string? HighGrade { get; set; }
        public string? PromotionPotential { get; set; }
        public string? OrganizationCodes { get; set; }
        public string? Relocation { get; set; }
        public string? HiringPath { get; set; }
        public string? TotalOpenings { get; set; }
        public string? AgencyMarketingStatement { get; set; }
        public string? TravelCode { get; set; }
        public string? DetailStatusUrl { get; set; }
        public string? MajorDuties { get; set; }
        public string? Education { get; set; }
        public string? Requirements { get; set; }
        public string? Evaluations { get; set; }
        public string? HowToApply { get; set; }
        public string? WhatToExpectNext { get; set; }
        public string? RequiredDocuments { get; set; }
        public string? Benefits { get; set; }
        public string? BenefitsUrl { get; set; }
        public string? OtherInformation { get; set; }
        public string? KeyRequirements { get; set; }
        public List<string>? WithinArea { get; set; }
        public string? CommuteDistance { get; set; }
        public List<string>? ServiceType { get; set; }
        public string? AnnouncementClosingType { get; set; }
        public string? AgencyContactEmail { get; set; }
        public string? AgencyContactPhone { get; set; }
        public string? SecurityClearance { get; set; }
        public string? DrugTest { get; set; }
        public string? AdjudicationType { get; set; }
        public string? TeleworkEligible { get; set; }
        public string? RemoteIndicator { get; set; }
    }

    public class WhoMayApply
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
    }
}