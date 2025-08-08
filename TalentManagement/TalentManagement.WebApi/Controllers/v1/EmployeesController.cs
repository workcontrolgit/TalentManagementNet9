using TalentManagement.Application.Features.Employees.Commands.CreateEmployee;
using TalentManagement.Application.Features.Employees.Commands.UpdateEmployee;
using TalentManagement.Application.Features.Employees.Commands.DeleteEmployee;
using TalentManagement.Application.Features.Employees.Queries.GetEmployeeById;
using TalentManagement.Application.Features.Employees.Queries.GetEmployees;

namespace TalentManagement.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class EmployeesController : BaseApiController
    {

        /// <summary>
        /// Gets a list of employees based on the specified filter.
        /// </summary>
        /// <param name="filter">The filter used to get the list of employees.</param>
        /// <returns>A list of employees.</returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetEmployeesQuery filter)
        {
            return Ok(await Mediator.Send(filter));
        }

        /// <summary>
        /// Gets an employee by its Id.
        /// </summary>
        /// <param name="id">The Id of the employee.</param>
        /// <returns>The employee with the specified Id.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return Ok(await Mediator.Send(new GetEmployeeByIdQuery { Id = id }));
        }

        /// <summary>
        /// Creates a new employee.
        /// </summary>
        /// <param name="command">The command containing the data for the new employee.</param>
        /// <returns>A 201 Created response containing the newly created employee.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(CreateEmployeeCommand command)
        {
            var resp = await Mediator.Send(command);
            return CreatedAtAction(nameof(Post), resp);
        }

        /// <summary>
        /// Retrieves a paged list of employees.
        /// Support Ngx-DataTables https://medium.com/scrum-and-coke/angular-11-pagination-of-zillion-rows-45d8533538c0
        /// </summary>
        /// <param name="query">The query parameters for the paged list.</param>
        /// <returns>A paged list of employees.</returns>
        [HttpPost]
        [Route("Paged")]
        public async Task<IActionResult> Paged(PagedEmployeesQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        /// <summary>
        /// Updates an employee with the given id using the provided command.
        /// </summary>
        /// <param name="id">The id of the employee to update.</param>
        /// <param name="command">The command containing the updated information.</param>
        /// <returns>The updated employee.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, UpdateEmployeeCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Deletes an employee by its Id.
        /// </summary>
        /// <param name="id">The Id of the employee to delete.</param>
        /// <returns>The result of the deletion.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await Mediator.Send(new DeleteEmployeeCommand { Id = id }));
        }

        /// <summary>
        /// Gets the count of employees based on the specified filter.
        /// </summary>
        /// <param name="filter">The filter used to count employees.</param>
        /// <returns>The number of employees matching the filter.</returns>
        [HttpGet("count")]
        public async Task<IActionResult> GetCount([FromQuery] GetEmployeesCountQuery filter)
        {
            return Ok(await Mediator.Send(filter));
        }

    }
}