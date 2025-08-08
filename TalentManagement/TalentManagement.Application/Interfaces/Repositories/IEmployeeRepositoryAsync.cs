namespace TalentManagement.Application.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for Employee entity with asynchronous methods.
    /// </summary>
    public interface IEmployeeRepositoryAsync : IGenericRepositoryAsync<Employee>
    {
        /// <summary>
        /// Checks if the given employee number is unique in the database.
        /// </summary>
        /// <param name="employeeNumber">Employee number to check for uniqueness.</param>
        /// <returns>
        /// Task indicating whether the employee number is unique.
        /// </returns>
        Task<bool> IsUniqueEmployeeNumberAsync(string employeeNumber);

        /// <summary>
        /// Retrieves a list of employees based on the provided query parameters asynchronously.
        /// </summary>
        /// <param name="requestParameters">The request parameters.</param>
        /// <returns>A task that represents the asynchronous operation and returns a tuple containing the list of employees and the total number of records.</returns>
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> GetEmployeeResponseAsync(GetEmployeesQuery requestParameters);

        /// <summary>
        /// Retrieves a paged list of employees based on the provided query parameters asynchronously.
        /// </summary>
        /// <param name="requestParameters">The request parameters.</param>
        /// <returns>A task that represents the asynchronous operation and returns a tuple containing the paged list of employees and the total number of records.</returns>
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> GetPagedEmployeeResponseAsync(PagedEmployeesQuery requestParameters);

        /// <summary>
        /// Gets the count of employees based on the provided filter parameters asynchronously.
        /// </summary>
        /// <param name="requestParameters">The filter parameters.</param>
        /// <returns>A task that represents the asynchronous operation and returns the count of employees.</returns>
        Task<int> GetEmployeesCountAsync(GetEmployeesCountQuery requestParameters);
    }
}