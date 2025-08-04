using TalentManagement.Application.Features.Departments.Queries.GetDepartments;

// Defines an asynchronous repository interface for the Department entity
namespace TalentManagement.Application.Interfaces.Repositories
{
    public interface IDepartmentRepositoryAsync : IGenericRepositoryAsync<Department>
    {
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> GetDepartmentReponseAsync(GetDepartmentsQuery requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> PagedDepartmentReponseAsync(PagedDepartmentsQuery requestParameters);
    }
}