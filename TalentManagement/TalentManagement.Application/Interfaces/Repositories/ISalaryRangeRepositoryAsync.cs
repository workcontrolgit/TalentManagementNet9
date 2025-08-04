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
    }
}