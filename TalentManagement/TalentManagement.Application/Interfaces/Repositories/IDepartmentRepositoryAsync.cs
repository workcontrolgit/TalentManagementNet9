using TalentManagement.Application.Features.Departments.Queries.GetDepartments;

// Defines an asynchronous repository interface for the Department entity
namespace TalentManagement.Application.Interfaces.Repositories
{
    public interface IDepartmentRepositoryAsync : IGenericRepositoryAsync<Department>
    {
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> GetDepartmentReponseAsync(GetDepartmentsQuery requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> PagedDepartmentReponseAsync(PagedDepartmentsQuery requestParameters);
        
        /// <summary>
        /// Gets the count of departments based on the provided filter parameters asynchronously.
        /// </summary>
        /// <param name="requestParameters">The filter parameters.</param>
        /// <returns>A task that represents the asynchronous operation and returns the count of departments.</returns>
        Task<int> GetDepartmentsCountAsync(GetDepartmentsCountQuery requestParameters);
    }
}