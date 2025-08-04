using Swashbuckle.AspNetCore.Annotations;
using TalentManagement.Application.Features.Departments.Commands.CreateDepartment;
using TalentManagement.Application.Features.Departments.Commands.UpdateDepartment;
using TalentManagement.Application.Features.Departments.Commands.DeleteDepartment;
using TalentManagement.Application.Features.Departments.Queries.GetDepartmentById;
using TalentManagement.Application.Features.Departments.Queries.GetDepartments;

namespace TalentManagement.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class DepartmentsController : BaseApiController
    {
        /// <summary>
        /// Gets a list of departments based on the specified filter.
        /// </summary>
        /// <param name="filter">The filter used to get the list of departments. Supports filtering by Name.</param>
        /// <returns>A list of departments.</returns>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get departments",
            Description = "Retrieves departments with optional filtering by Name and paging support.",
            OperationId = "GetDepartments",
            Tags = new[] { "Departments" }
        )]
        public async Task<IActionResult> Get([FromQuery] GetDepartmentsQuery filter)
        {
            return Ok(await Mediator.Send(filter));
        }

        /// <summary>
        /// Gets a department by its Id.
        /// </summary>
        /// <param name="id">The Id of the department.</param>
        /// <returns>The department with the specified Id.</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get department by ID",
            Description = "Retrieves a specific department by its ID.",
            OperationId = "GetDepartmentById",
            Tags = new[] { "Departments" }
        )]
        public async Task<IActionResult> Get(Guid id)
        {
            return Ok(await Mediator.Send(new GetDepartmentByIdQuery { Id = id }));
        }

        /// <summary>
        /// Creates a new department.
        /// </summary>
        /// <param name="command">The command containing the data for the new department.</param>
        /// <returns>A 201 Created response containing the newly created department.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "Create department",
            Description = "Creates a new department.",
            OperationId = "CreateDepartment",
            Tags = new[] { "Departments" }
        )]
        public async Task<IActionResult> Post(CreateDepartmentCommand command)
        {
            var resp = await Mediator.Send(command);
            return CreatedAtAction(nameof(Post), resp);
        }

        /// <summary>
        /// Retrieves a paged list of departments.
        /// </summary>
        /// <param name="query">The query parameters for the paged list.</param>
        /// <returns>A paged list of departments.</returns>
        [HttpPost]
        [Route("Paged")]
        [SwaggerOperation(
            Summary = "Get paged departments",
            Description = "Retrieves a paged list of departments.",
            OperationId = "GetPagedDepartments",
            Tags = new[] { "Departments" }
        )]
        public async Task<IActionResult> Paged(PagedDepartmentsQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        /// <summary>
        /// Updates a department with the given id using the provided command.
        /// </summary>
        /// <param name="id">The id of the department to update.</param>
        /// <param name="command">The command containing the updated information.</param>
        /// <returns>The updated department.</returns>
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Update department",
            Description = "Updates an existing department.",
            OperationId = "UpdateDepartment",
            Tags = new[] { "Departments" }
        )]
        public async Task<IActionResult> Put(Guid id, UpdateDepartmentCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Deletes a department by its Id.
        /// </summary>
        /// <param name="id">The Id of the department to delete.</param>
        /// <returns>The result of the deletion.</returns>
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete department",
            Description = "Deletes a department by its ID.",
            OperationId = "DeleteDepartment",
            Tags = new[] { "Departments" }
        )]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await Mediator.Send(new DeleteDepartmentCommand { Id = id }));
        }
    }
}