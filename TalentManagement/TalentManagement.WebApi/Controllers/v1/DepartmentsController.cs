using Swashbuckle.AspNetCore.Annotations;

namespace TalentManagement.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class DepartmentsController : BaseApiController
    {
        /// <summary>
        /// Gets a list of employees based on the specified filter.
        /// </summary>
        /// <param name="filter">The filter used to get the list of employees.</param>
        /// <returns>A list of employees.</returns>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get employees by department",
            Description = "Retrieves all employees belonging to a specific department, with option to include sub-departments.",
            OperationId = "GetEmployeesByDepartment",
            Tags = new[] { "Employees" }
        )]
        public async Task<IActionResult> Get([FromQuery] GetDepartmentsQuery filter)
        {
            return Ok(await Mediator.Send(filter));
        }
    }
}