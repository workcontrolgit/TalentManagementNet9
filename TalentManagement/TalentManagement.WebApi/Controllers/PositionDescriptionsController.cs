using MediatR;
using Microsoft.AspNetCore.Mvc;
using TalentManagement.Application.Features.PositionDescriptions.Commands.CreatePositionDescription;
using TalentManagement.Application.Features.PositionDescriptions.Commands.DeletePositionDescription;
using TalentManagement.Application.Features.PositionDescriptions.Commands.UpdatePositionDescription;
using TalentManagement.Application.Features.PositionDescriptions.Queries.GetPositionDescriptionById;
using TalentManagement.Application.Features.PositionDescriptions.Queries.GetPositionDescriptions;
using TalentManagement.Application.Wrappers;
using Entity = TalentManagement.Domain.Entities.Entity;

namespace TalentManagement.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionDescriptionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PositionDescriptionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get Position Description by PdSeqNum
        /// </summary>
        /// <param name="pdSeqNum">Position Description Sequence Number</param>
        /// <returns>Position Description details</returns>
        [HttpGet("{pdSeqNum}")]
        public async Task<IActionResult> GetById(decimal pdSeqNum)
        {
            return Ok(await _mediator.Send(new GetPositionDescriptionByIdQuery { PdSeqNum = pdSeqNum }));
        }

        /// <summary>
        /// Get all Position Descriptions with filtering and paging
        /// </summary>
        /// <param name="filter">Filter parameters</param>
        /// <returns>Paged list of Position Descriptions</returns>
        [HttpGet]
        public async Task<IActionResult> GetPagedListAsync([FromQuery] GetPositionDescriptionsQuery filter)
        {
            return Ok(await _mediator.Send(filter));
        }

        /// <summary>
        /// Get Position Descriptions for DataTable
        /// </summary>
        /// <param name="requestParameters">DataTable request parameters</param>
        /// <returns>DataTable response with Position Descriptions</returns>
        [HttpPost("datatable")]
        public async Task<IActionResult> GetPagedListForDataTable([FromBody] PagedPositionDescriptionsQuery requestParameters)
        {
            return Ok(await _mediator.Send(requestParameters));
        }

        /// <summary>
        /// Create a new Position Description
        /// </summary>
        /// <param name="command">Position Description creation data</param>
        /// <returns>Created Position Description PdSeqNum</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePositionDescriptionCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Update an existing Position Description
        /// </summary>
        /// <param name="pdSeqNum">Position Description Sequence Number</param>
        /// <param name="command">Position Description update data</param>
        /// <returns>Success response</returns>
        [HttpPut("{pdSeqNum}")]
        public async Task<IActionResult> Update(decimal pdSeqNum, [FromBody] UpdatePositionDescriptionCommand command)
        {
            if (pdSeqNum != command.PdSeqNum)
            {
                return BadRequest("PdSeqNum mismatch");
            }
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Delete a Position Description
        /// </summary>
        /// <param name="pdSeqNum">Position Description Sequence Number</param>
        /// <returns>Success response</returns>
        [HttpDelete("{pdSeqNum}")]
        public async Task<IActionResult> Delete(decimal pdSeqNum)
        {
            return Ok(await _mediator.Send(new DeletePositionDescriptionCommand { PdSeqNum = pdSeqNum }));
        }
    }
}