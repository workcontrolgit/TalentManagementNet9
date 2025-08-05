using TalentManagement.Application.DTOs.External.USAJobs;

namespace TalentManagement.Application.DTOs.JobAggregation
{
    public class JobSearchResult
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<AggregatedJobListing> Jobs { get; set; } = new();
        public JobSearchMetadata Metadata { get; set; } = new();
    }

    public class AggregatedJobListing
    {
        // Common properties for both internal and external jobs
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public SalaryInfo? Salary { get; set; }
        public DateTime? PostedDate { get; set; }
        public DateTime? ClosingDate { get; set; }
        public JobSource Source { get; set; }
        public string? ExternalUrl { get; set; }
        public List<string> Keywords { get; set; } = new();
        public List<string> RequiredSkills { get; set; } = new();
        public JobType JobType { get; set; }
        public string? WorkSchedule { get; set; }
        public bool IsRemote { get; set; }
        public string? SecurityClearance { get; set; }
        
        // Internal matching data
        public List<MatchingCandidate> MatchingCandidates { get; set; } = new();
        public int InternalApplications { get; set; }
        public List<string> RelatedPositions { get; set; } = new();
    }

    public class SalaryInfo
    {
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string? Currency { get; set; } = "USD";
        public string? PayFrequency { get; set; } // Annual, Hourly, etc.
        public string? PayGrade { get; set; }
    }

    public class MatchingCandidate
    {
        public Guid EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal MatchScore { get; set; }
        public List<string> MatchingSkills { get; set; } = new();
        public string CurrentPosition { get; set; } = string.Empty;
    }

    public class JobSearchMetadata
    {
        public DateTime SearchTimestamp { get; set; }
        public TimeSpan SearchDuration { get; set; }
        public int ExternalJobsCount { get; set; }
        public int InternalJobsCount { get; set; }
        public List<string> SearchSources { get; set; } = new();
        public bool HasMoreResults { get; set; }
        public List<SearchWarning> Warnings { get; set; } = new();
    }

    public class SearchWarning
    {
        public string Source { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public WarningType Type { get; set; }
    }

    public enum JobSource
    {
        Internal,
        USAJobs,
        Other
    }

    public enum JobType
    {
        FullTime,
        PartTime,
        Contract,
        Temporary,
        Internship
    }

    public enum WarningType
    {
        RateLimitApproaching,
        PartialResults,
        ServiceUnavailable,
        DataIncomplete
    }
}