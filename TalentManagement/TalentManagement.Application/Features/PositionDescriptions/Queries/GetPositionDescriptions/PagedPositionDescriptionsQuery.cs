using MediatR;
using TalentManagement.Application.Parameters;
using TalentManagement.Application.Wrappers;
using Entity = TalentManagement.Domain.Entities.Entity;

namespace TalentManagement.Application.Features.PositionDescriptions.Queries.GetPositionDescriptions
{
    public partial class PagedPositionDescriptionsQuery : QueryParameter, IRequest<PagedDataTableResponse<IEnumerable<Entity>>>
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public IList<SortOrder> Order { get; set; }
        public Search Search { get; set; }
        public IList<Column> Columns { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
    }
}