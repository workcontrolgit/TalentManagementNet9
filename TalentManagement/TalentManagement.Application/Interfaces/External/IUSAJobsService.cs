using TalentManagement.Application.DTOs.External.USAJobs;

namespace TalentManagement.Application.Interfaces.External
{
    public interface IUSAJobsService
    {
        Task<USAJobsResponse?> SearchJobsAsync(USAJobsSearchRequest request, CancellationToken cancellationToken = default);
        Task<MatchedObjectDescriptor?> GetJobDetailsAsync(string positionId, CancellationToken cancellationToken = default);
        Task<bool> ValidateApiConnectionAsync(CancellationToken cancellationToken = default);
    }
}