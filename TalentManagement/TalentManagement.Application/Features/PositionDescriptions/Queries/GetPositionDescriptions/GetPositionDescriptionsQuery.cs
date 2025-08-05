using MediatR;
using TalentManagement.Application.Parameters;
using TalentManagement.Application.Wrappers;
using Entity = TalentManagement.Domain.Entities.Entity;

namespace TalentManagement.Application.Features.PositionDescriptions.Queries.GetPositionDescriptions
{
    public class GetPositionDescriptionsQuery : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {
        public string PdNbr { get; set; }
        public string PdPositionTitleText { get; set; }
        public string GvtOccSeries { get; set; }
        public string GvtPayPlan { get; set; }
        public string PdsStateCd { get; set; }
    }
}