using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TalentManagement.Application.DTOs.JobAggregation;
using TalentManagement.Application.DTOs.External.USAJobs;
using TalentManagement.Application.Interfaces;
using TalentManagement.Application.Interfaces.External;
using TalentManagement.Application.Interfaces.Repositories;

namespace TalentManagement.Infrastructure.Shared.Services
{
    public class JobAggregationService : IJobAggregationService
    {
        private readonly IUSAJobsService _usaJobsService;
        private readonly IPositionRepositoryAsync _positionRepository;
        private readonly IEmployeeRepositoryAsync _employeeRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<JobAggregationService> _logger;

        public JobAggregationService(
            IUSAJobsService usaJobsService,
            IPositionRepositoryAsync positionRepository,
            IEmployeeRepositoryAsync employeeRepository,
            ICacheService cacheService,
            ILogger<JobAggregationService> logger)
        {
            _usaJobsService = usaJobsService;
            _positionRepository = positionRepository;
            _employeeRepository = employeeRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<JobSearchResult> SearchJobsAsync(JobSearchRequest request, CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Starting job search with keywords: {Keywords}, sources: {Sources}", 
                request.Keywords, string.Join(", ", request.Sources));

            var result = new JobSearchResult
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Metadata = new JobSearchMetadata
                {
                    SearchTimestamp = DateTime.UtcNow,
                    SearchSources = request.Sources.Select(s => s.ToString()).ToList()
                }
            };

            var allJobs = new List<AggregatedJobListing>();
            var searchTasks = new List<Task>();

            // Search internal positions if requested
            if (request.Sources.Contains(JobSource.Internal))
            {
                searchTasks.Add(SearchInternalJobsAsync(request, allJobs, result.Metadata, cancellationToken));
            }

            // Search USAJobs if requested
            if (request.Sources.Contains(JobSource.USAJobs))
            {
                searchTasks.Add(SearchUSAJobsAsync(request, allJobs, result.Metadata, cancellationToken));
            }

            // Wait for all searches to complete
            await Task.WhenAll(searchTasks);

            // Sort and paginate results
            var sortedJobs = SortJobs(allJobs, request.SortBy, request.SortDirection);
            
            result.TotalCount = sortedJobs.Count;
            result.Jobs = sortedJobs
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Add matching candidates to each job
            foreach (var job in result.Jobs)
            {
                job.MatchingCandidates = await FindMatchingCandidatesForJobAsync(job, cancellationToken);
            }

            stopwatch.Stop();
            result.Metadata.SearchDuration = stopwatch.Elapsed;
            result.Metadata.HasMoreResults = result.TotalCount > (request.Page * request.PageSize);

            _logger.LogInformation("Job search completed in {Duration}ms. Found {TotalJobs} jobs ({InternalJobs} internal, {ExternalJobs} external)",
                stopwatch.ElapsedMilliseconds, result.TotalCount, result.Metadata.InternalJobsCount, result.Metadata.ExternalJobsCount);

            return result;
        }

        private async Task SearchInternalJobsAsync(JobSearchRequest request, List<AggregatedJobListing> allJobs, 
            JobSearchMetadata metadata, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"internal_jobs_{request.Keywords}_{request.Location}_{request.Page}_{request.PageSize}";
                
                var internalJobs = await _cacheService.GetOrSetAsync(cacheKey, async () =>
                {
                    // This would be a more sophisticated search in a real implementation
                    // For simplified implementation, use the base GetPagedReponseAsync method
                    var positions = await _positionRepository.GetPagedReponseAsync(1, 100);

                    return positions?.Select(p => MapInternalPositionToAggregatedJob(p)).ToList() ?? new List<AggregatedJobListing>();
                }, TimeSpan.FromMinutes(15), cancellationToken);

                // Apply filters
                var filteredJobs = ApplyFilters(internalJobs, request);
                
                allJobs.AddRange(filteredJobs);
                metadata.InternalJobsCount = filteredJobs.Count;

                _logger.LogDebug("Found {Count} internal jobs", filteredJobs.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching internal jobs");
                metadata.Warnings.Add(new SearchWarning
                {
                    Source = "Internal",
                    Message = "Failed to search internal positions",
                    Type = WarningType.ServiceUnavailable
                });
            }
        }

        private async Task SearchUSAJobsAsync(JobSearchRequest request, List<AggregatedJobListing> allJobs, 
            JobSearchMetadata metadata, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"usajobs_{request.Keywords}_{request.Location}_{request.Page}_{request.PageSize}";
                
                var usaJobsResult = await _cacheService.GetOrSetAsync(cacheKey, async () =>
                {
                    var usaJobsRequest = new USAJobsSearchRequest
                    {
                        Keyword = request.Keywords,
                        LocationName = request.Location,
                        Page = 1, // Always get first page for aggregation
                        ResultsPerPage = 100, // Get more results for better filtering
                        SortField = "ApplicationCloseDate",
                        SortDirection = "Desc"
                    };

                    return await _usaJobsService.SearchJobsAsync(usaJobsRequest, cancellationToken);
                }, TimeSpan.FromMinutes(30), cancellationToken); // Cache external API calls longer

                if (usaJobsResult?.SearchResult?.SearchResultItems != null)
                {
                    var externalJobs = usaJobsResult.SearchResult.SearchResultItems
                        .Where(item => item.MatchedObjectDescriptor != null)
                        .Select(item => MapUSAJobToAggregatedJob(item.MatchedObjectDescriptor!))
                        .ToList();

                    // Apply filters
                    var filteredJobs = ApplyFilters(externalJobs, request);
                    
                    allJobs.AddRange(filteredJobs);
                    metadata.ExternalJobsCount = filteredJobs.Count;

                    _logger.LogDebug("Found {Count} USAJobs positions", filteredJobs.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching USAJobs");
                metadata.Warnings.Add(new SearchWarning
                {
                    Source = "USAJobs",
                    Message = "Failed to search external job postings",
                    Type = WarningType.ServiceUnavailable
                });
            }
        }

        private static AggregatedJobListing MapInternalPositionToAggregatedJob(dynamic position)
        {
            return new AggregatedJobListing
            {
                Id = position.Id?.ToString() ?? string.Empty,
                Title = position.PositionTitle ?? string.Empty,
                Description = position.PositionDescription ?? string.Empty,
                Organization = "Internal",
                Department = position.Department?.Name ?? string.Empty,
                Location = "Internal",
                PostedDate = position.Created,
                Source = JobSource.Internal,
                JobType = JobType.FullTime,
                IsRemote = false
            };
        }

        private static AggregatedJobListing MapUSAJobToAggregatedJob(MatchedObjectDescriptor job)
        {
            var salary = ParseSalaryInfo(job.PositionRemuneration);
            
            return new AggregatedJobListing
            {
                Id = job.PositionID ?? string.Empty,
                Title = job.PositionTitle ?? string.Empty,
                Description = job.PositionFormattedDescription ?? job.UserArea?.Details?.JobSummary ?? string.Empty,
                Organization = job.OrganizationName ?? string.Empty,
                Department = job.DepartmentName ?? string.Empty,
                Location = job.PositionLocationDisplay ?? string.Empty,
                Salary = salary,
                PostedDate = job.PublicationStartDate,
                ClosingDate = job.ApplicationCloseDate,
                Source = JobSource.USAJobs,
                ExternalUrl = job.PositionURI,
                JobType = MapJobType(job.PositionSchedule?.Code),
                WorkSchedule = job.PositionSchedule?.Name,
                IsRemote = IsRemotePosition(job),
                SecurityClearance = job.UserArea?.Details?.SecurityClearance,
                Keywords = ExtractKeywords(job),
                RequiredSkills = ExtractRequiredSkills(job)
            };
        }

        private static SalaryInfo? ParseSalaryInfo(PositionRemuneration? remuneration)
        {
            if (remuneration == null) return null;

            return new SalaryInfo
            {
                MinSalary = ParseSalaryValue(remuneration.MinimumRange),
                MaxSalary = ParseSalaryValue(remuneration.MaximumRange),
                PayFrequency = remuneration.RateIntervalCode,
                Currency = "USD"
            };
        }

        private static decimal? ParseSalaryValue(string? salaryString)
        {
            if (string.IsNullOrEmpty(salaryString)) return null;
            
            var cleanString = salaryString.Replace("$", "").Replace(",", "").Replace(" ", "");
            return decimal.TryParse(cleanString, out var value) ? value : null;
        }

        private static JobType MapJobType(string? scheduleCode)
        {
            return scheduleCode?.ToUpper() switch
            {
                "1" or "F" => JobType.FullTime,
                "2" or "P" => JobType.PartTime,
                "3" or "T" => JobType.Temporary,
                "4" or "I" => JobType.Internship,
                _ => JobType.FullTime
            };
        }

        private static bool IsRemotePosition(MatchedObjectDescriptor job)
        {
            var remoteIndicator = job.UserArea?.Details?.RemoteIndicator;
            var teleworkEligible = job.UserArea?.Details?.TeleworkEligible;
            
            return !string.IsNullOrEmpty(remoteIndicator) || 
                   (!string.IsNullOrEmpty(teleworkEligible) && teleworkEligible.ToLower().Contains("yes"));
        }

        private static List<string> ExtractKeywords(MatchedObjectDescriptor job)
        {
            var keywords = new List<string>();
            
            if (!string.IsNullOrEmpty(job.JobCategory))
                keywords.Add(job.JobCategory);
                
            if (job.JobGrade?.Any() == true)
                keywords.AddRange(job.JobGrade.Where(g => !string.IsNullOrEmpty(g.Name)).Select(g => g.Name!));
                
            return keywords;
        }

        private static List<string> ExtractRequiredSkills(MatchedObjectDescriptor job)
        {
            var skills = new List<string>();
            
            // This would be more sophisticated in a real implementation
            // analyzing the job description for skills
            var description = job.PositionFormattedDescription ?? job.UserArea?.Details?.Requirements ?? string.Empty;
            
            // Simple skill extraction (would use NLP in production)
            var commonSkills = new[] { "Project Management", "Leadership", "Communication", "Analysis", "Problem Solving" };
            skills.AddRange(commonSkills.Where(skill => description.Contains(skill, StringComparison.OrdinalIgnoreCase)));
            
            return skills;
        }

        private static List<AggregatedJobListing> ApplyFilters(List<AggregatedJobListing> jobs, JobSearchRequest request)
        {
            var filteredJobs = jobs.AsQueryable();

            if (request.SalaryRange != null)
            {
                filteredJobs = filteredJobs.Where(j => 
                    j.Salary != null && 
                    (request.SalaryRange.MinSalary == null || j.Salary.MinSalary >= request.SalaryRange.MinSalary) &&
                    (request.SalaryRange.MaxSalary == null || j.Salary.MaxSalary <= request.SalaryRange.MaxSalary));
            }

            if (request.JobTypes.Any())
            {
                filteredJobs = filteredJobs.Where(j => request.JobTypes.Contains(j.JobType));
            }

            if (request.IsRemote.HasValue)
            {
                filteredJobs = filteredJobs.Where(j => j.IsRemote == request.IsRemote.Value);
            }

            if (request.PostedAfter.HasValue)
            {
                filteredJobs = filteredJobs.Where(j => j.PostedDate >= request.PostedAfter.Value);
            }

            if (request.RequiredSkills.Any())
            {
                filteredJobs = filteredJobs.Where(j => 
                    request.RequiredSkills.Any(skill => 
                        j.RequiredSkills.Any(jobSkill => 
                            jobSkill.Contains(skill, StringComparison.OrdinalIgnoreCase))));
            }

            return filteredJobs.ToList();
        }

        private static List<AggregatedJobListing> SortJobs(List<AggregatedJobListing> jobs, string? sortBy, string? sortDirection)
        {
            var isDescending = sortDirection?.ToLower() == "desc";
            
            return sortBy?.ToLower() switch
            {
                "title" => isDescending ? jobs.OrderByDescending(j => j.Title).ToList() : jobs.OrderBy(j => j.Title).ToList(),
                "posted" or "date" => isDescending ? jobs.OrderByDescending(j => j.PostedDate).ToList() : jobs.OrderBy(j => j.PostedDate).ToList(),
                "salary" => isDescending ? jobs.OrderByDescending(j => j.Salary?.MaxSalary ?? 0).ToList() : jobs.OrderBy(j => j.Salary?.MaxSalary ?? 0).ToList(),
                "organization" => isDescending ? jobs.OrderByDescending(j => j.Organization).ToList() : jobs.OrderBy(j => j.Organization).ToList(),
                _ => jobs.OrderByDescending(j => j.PostedDate).ToList() // Default to newest first
            };
        }

        private async Task<List<MatchingCandidate>> FindMatchingCandidatesForJobAsync(AggregatedJobListing job, CancellationToken cancellationToken)
        {
            try
            {
                // This is a simplified matching algorithm
                // In production, this would be much more sophisticated
                var employees = await _employeeRepository.GetPagedReponseAsync(1, 50);
                
                var matchingCandidates = new List<MatchingCandidate>();
                
                if (employees != null)
                {
                    foreach (var employee in employees.Take(5)) // Limit to top 5 matches
                    {
                        var matchScore = CalculateMatchScore(job, employee);
                        if (matchScore > 0.3m) // Only include if match score > 30%
                        {
                            matchingCandidates.Add(new MatchingCandidate
                            {
                                EmployeeId = employee.Id,
                                FullName = $"{employee.FirstName} {employee.LastName}",
                                Email = employee.Email ?? string.Empty,
                                MatchScore = matchScore,
                                CurrentPosition = employee.Position?.PositionTitle ?? string.Empty,
                                MatchingSkills = GetMatchingSkills(job, employee)
                            });
                        }
                    }
                }
                
                return matchingCandidates.OrderByDescending(c => c.MatchScore).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding matching candidates for job: {JobId}", job.Id);
                return new List<MatchingCandidate>();
            }
        }

        private static decimal CalculateMatchScore(AggregatedJobListing job, dynamic employee)
        {
            // Simplified scoring algorithm
            decimal score = 0;
            
            // Basic scoring based on position title similarity
            if (job.Title.Contains(employee.Position?.PositionTitle ?? string.Empty, StringComparison.OrdinalIgnoreCase))
                score += 0.4m;
                
            // Department similarity
            if (job.Department.Contains(employee.Position?.Department?.Name ?? string.Empty, StringComparison.OrdinalIgnoreCase))
                score += 0.3m;
                
            // Random component for demo purposes
            score += new Random().Next(1, 4) * 0.1m;
            
            return Math.Min(score, 1.0m);
        }

        private static List<string> GetMatchingSkills(AggregatedJobListing job, dynamic employee)
        {
            // Simplified skill matching
            var commonSkills = new List<string> { "Communication", "Leadership", "Problem Solving" };
            return commonSkills.Take(new Random().Next(1, 3)).ToList();
        }

        public async Task<AggregatedJobListing?> GetJobDetailsAsync(string jobId, JobSource source, CancellationToken cancellationToken = default)
        {
            try
            {
                return source switch
                {
                    JobSource.Internal => await GetInternalJobDetailsAsync(jobId, cancellationToken),
                    JobSource.USAJobs => await GetUSAJobDetailsAsync(jobId, cancellationToken),
                    _ => null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting job details for {JobId} from {Source}", jobId, source);
                return null;
            }
        }

        private async Task<AggregatedJobListing?> GetInternalJobDetailsAsync(string jobId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(jobId, out var positionId)) return null;
            
            var position = await _positionRepository.GetByIdAsync(positionId);
            return position != null ? MapInternalPositionToAggregatedJob(position) : null;
        }

        private async Task<AggregatedJobListing?> GetUSAJobDetailsAsync(string jobId, CancellationToken cancellationToken)
        {
            var jobDetails = await _usaJobsService.GetJobDetailsAsync(jobId, cancellationToken);
            return jobDetails != null ? MapUSAJobToAggregatedJob(jobDetails) : null;
        }

        public async Task<List<MatchingCandidate>> FindMatchingCandidatesAsync(string jobId, JobSource source, CancellationToken cancellationToken = default)
        {
            var job = await GetJobDetailsAsync(jobId, source, cancellationToken);
            return job != null ? await FindMatchingCandidatesForJobAsync(job, cancellationToken) : new List<MatchingCandidate>();
        }

        public async Task<JobSearchResult> GetRecommendedJobsAsync(Guid employeeId, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(employeeId);
                if (employee == null)
                {
                    return new JobSearchResult();
                }

                // Create a search request based on employee's current position and skills
                var searchRequest = new JobSearchRequest
                {
                    Keywords = employee.Position?.PositionTitle,
                    PageSize = pageSize,
                    Sources = new List<JobSource> { JobSource.Internal, JobSource.USAJobs }
                };

                return await SearchJobsAsync(searchRequest, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recommended jobs for employee: {EmployeeId}", employeeId);
                return new JobSearchResult();
            }
        }
    }
}