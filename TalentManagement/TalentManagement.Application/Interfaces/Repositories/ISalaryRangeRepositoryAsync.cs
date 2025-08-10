using TalentManagement.Application.Features.SalaryRanges.Queries.GetSalaryRanges;

namespace TalentManagement.Application.Interfaces.Repositories
{
    /// <summary>
    /// Represents a repository for performing asynchronous operations on salary ranges.
    /// </summary>
    public interface ISalaryRangeRepositoryAsync : IGenericRepositoryAsync<SalaryRange>
    {
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> GetSalaryRangeReponseAsync(GetSalaryRangesQuery requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> PagedSalaryRangeReponseAsync(PagedSalaryRangesQuery requestParameters);
        
        /// <summary>
        /// Gets the count of salary ranges based on the provided filter parameters asynchronously.
        /// </summary>
        /// <param name="requestParameters">The filter parameters.</param>
        /// <returns>A task that represents the asynchronous operation and returns the count of salary ranges.</returns>
        Task<int> GetSalaryRangesCountAsync(GetSalaryRangesCountQuery requestParameters);
    }
}