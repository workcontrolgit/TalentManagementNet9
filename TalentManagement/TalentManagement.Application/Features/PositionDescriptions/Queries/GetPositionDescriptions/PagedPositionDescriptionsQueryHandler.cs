using MediatR;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Application.Wrappers;
using Entity = TalentManagement.Domain.Entities.Entity;

namespace TalentManagement.Application.Features.PositionDescriptions.Queries.GetPositionDescriptions
{
    public class PagedPositionDescriptionsQueryHandler : IRequestHandler<PagedPositionDescriptionsQuery, PagedDataTableResponse<IEnumerable<Entity>>>
    {
        private readonly IPositionDescriptionRepositoryAsync _positionDescriptionRepository;

        public PagedPositionDescriptionsQueryHandler(IPositionDescriptionRepositoryAsync positionDescriptionRepository)
        {
            _positionDescriptionRepository = positionDescriptionRepository;
        }

        public async Task<PagedDataTableResponse<IEnumerable<Entity>>> Handle(PagedPositionDescriptionsQuery request, CancellationToken cancellationToken)
        {
            var objWithRecordsCount = await _positionDescriptionRepository.PagedPositionDescriptionReponseAsync(request);
            return new PagedDataTableResponse<IEnumerable<Entity>>(objWithRecordsCount.data, request.Draw, objWithRecordsCount.recordsCount);
        }
    }
}