using TalentManagement.Application.DTOs.JobAggregation;
using TalentManagement.Application.DTOs.External.USAJobs;

namespace TalentManagement.Application.Interfaces
{
    public interface IJobAggregationService
    {
        Task<JobSearchResult> SearchJobsAsync(JobSearchRequest request, CancellationToken cancellationToken = default);
        Task<AggregatedJobListing?> GetJobDetailsAsync(string jobId, JobSource source, CancellationToken cancellationToken = default);
        Task<List<MatchingCandidate>> FindMatchingCandidatesAsync(string jobId, JobSource source, CancellationToken cancellationToken = default);
        Task<JobSearchResult> GetRecommendedJobsAsync(Guid employeeId, int pageSize = 10, CancellationToken cancellationToken = default);
    }

    public class JobSearchRequest
    {
        public string? Keywords { get; set; }
        public string? Location { get; set; }
        public List<JobSource> Sources { get; set; } = new() { JobSource.Internal, JobSource.USAJobs };
        public SalaryFilter? SalaryRange { get; set; }
        public List<JobType> JobTypes { get; set; } = new();
        public bool? IsRemote { get; set; }
        public string? Organization { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public string? SortBy { get; set; } = "relevance";
        public string? SortDirection { get; set; } = "desc";
        public DateTime? PostedAfter { get; set; }
        public List<string> RequiredSkills { get; set; } = new();
        public string? SecurityClearance { get; set; }
    }

    public class SalaryFilter
    {
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
    }
}