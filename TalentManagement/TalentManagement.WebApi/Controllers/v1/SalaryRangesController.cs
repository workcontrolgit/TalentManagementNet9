using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using TalentManagement.Application.Features.SalaryRanges.Commands.CreateSalaryRange;
using TalentManagement.Application.Features.SalaryRanges.Commands.UpdateSalaryRange;
using TalentManagement.Application.Features.SalaryRanges.Commands.DeleteSalaryRange;
using TalentManagement.Application.Features.SalaryRanges.Queries.GetSalaryRangeById;
using TalentManagement.Application.Features.SalaryRanges.Queries.GetSalaryRanges;

namespace TalentManagement.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class SalaryRangesController : BaseApiController
    {
        /// <summary>
        /// Gets a list of salary ranges based on the specified filter.
        /// </summary>
        /// <param name="filter">The filter used to get the list of salary ranges. Supports filtering by Name.</param>
        /// <returns>A list of salary ranges.</returns>
        [HttpGet]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get salary ranges",
            Description = "Retrieves salary ranges with optional filtering by Name and paging support.",
            OperationId = "GetSalaryRanges",
            Tags = new[] { "SalaryRanges" }
        )]
        public async Task<IActionResult> Get([FromQuery] GetSalaryRangesQuery filter)
        {
            return Ok(await Mediator.Send(filter));
        }

        /// <summary>
        /// Gets a salary range by its Id.
        /// </summary>
        /// <param name="id">The Id of the salary range.</param>
        /// <returns>The salary range with the specified Id.</returns>
        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get salary range by ID",
            Description = "Retrieves a specific salary range by its ID.",
            OperationId = "GetSalaryRangeById",
            Tags = new[] { "SalaryRanges" }
        )]
        public async Task<IActionResult> Get(Guid id)
        {
            return Ok(await Mediator.Send(new GetSalaryRangeByIdQuery { Id = id }));
        }

        /// <summary>
        /// Creates a new salary range.
        /// </summary>
        /// <param name="command">The command containing the data for the new salary range.</param>
        /// <returns>A 201 Created response containing the newly created salary range.</returns>
        [HttpPost]
        [Authorize(Policy = AuthorizationConsts.AdminPolicy)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "Create salary range",
            Description = "Creates a new salary range.",
            OperationId = "CreateSalaryRange",
            Tags = new[] { "SalaryRanges" }
        )]
        public async Task<IActionResult> Post(CreateSalaryRangeCommand command)
        {
            var resp = await Mediator.Send(command);
            return CreatedAtAction(nameof(Post), resp);
        }

        /// <summary>
        /// Retrieves a paged list of salary ranges.
        /// </summary>
        /// <param name="query">The query parameters for the paged list.</param>
        /// <returns>A paged list of salary ranges.</returns>
        [HttpPost]
        [Authorize]
        [Route("Paged")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get paged salary ranges",
            Description = "Retrieves a paged list of salary ranges.",
            OperationId = "GetPagedSalaryRanges",
            Tags = new[] { "SalaryRanges" }
        )]
        public async Task<IActionResult> Paged(PagedSalaryRangesQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        /// <summary>
        /// Updates a salary range with the given id using the provided command.
        /// </summary>
        /// <param name="id">The id of the salary range to update.</param>
        /// <param name="command">The command containing the updated information.</param>
        /// <returns>The updated salary range.</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = AuthorizationConsts.AdminPolicy)]
        [SwaggerOperation(
            Summary = "Update salary range",
            Description = "Updates an existing salary range.",
            OperationId = "UpdateSalaryRange",
            Tags = new[] { "SalaryRanges" }
        )]
        public async Task<IActionResult> Put(Guid id, UpdateSalaryRangeCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Deletes a salary range by its Id.
        /// </summary>
        /// <param name="id">The Id of the salary range to delete.</param>
        /// <returns>The result of the deletion.</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationConsts.AdminPolicy)]
        [SwaggerOperation(
            Summary = "Delete salary range",
            Description = "Deletes a salary range by its ID.",
            OperationId = "DeleteSalaryRange",
            Tags = new[] { "SalaryRanges" }
        )]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await Mediator.Send(new DeleteSalaryRangeCommand { Id = id }));
        }

        /// <summary>
        /// Gets the count of salary ranges based on the specified filter.
        /// </summary>
        /// <param name="filter">The filter used to count salary ranges.</param>
        /// <returns>The number of salary ranges matching the filter.</returns>
        [HttpGet("count")]
        [SwaggerOperation(
            Summary = "Get salary ranges count",
            Description = "Retrieves the count of salary ranges with optional filtering by Name.",
            OperationId = "GetSalaryRangesCount",
            Tags = new[] { "SalaryRanges" }
        )]
        public async Task<IActionResult> GetCount([FromQuery] GetSalaryRangesCountQuery filter)
        {
            return Ok(await Mediator.Send(filter));
        }
    }
}