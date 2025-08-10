namespace TalentManagement.Application.DTOs.External.USAJobs
{
    public class USAJobsSearchRequest
    {
        public string? Keyword { get; set; }
        public string? LocationName { get; set; }
        public int? Page { get; set; } = 1;
        public int? ResultsPerPage { get; set; } = 25;
        public string? SortField { get; set; }
        public string? SortDirection { get; set; }
        public string? Organization { get; set; }
        public string? PayGradeHigh { get; set; }
        public string? PayGradeLow { get; set; }
        public string? PositionScheduleTypeCode { get; set; }
        public string? PositionOfferingTypeCode { get; set; }
        public DateTime? DatePosted { get; set; }
        public DateTime? JobCategoryCode { get; set; }
    }
}