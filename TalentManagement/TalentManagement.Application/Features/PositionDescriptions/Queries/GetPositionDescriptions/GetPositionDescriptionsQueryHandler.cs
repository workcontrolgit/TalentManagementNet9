using MediatR;
using TalentManagement.Application.Interfaces;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Application.Wrappers;
using Entity = TalentManagement.Domain.Entities.Entity;

namespace TalentManagement.Application.Features.PositionDescriptions.Queries.GetPositionDescriptions
{
    public class GetAllPositionDescriptionsQueryHandler : IRequestHandler<GetPositionDescriptionsQuery, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly IPositionDescriptionRepositoryAsync _positionDescriptionRepository;
        private readonly IModelHelper _modelHelper;

        public GetAllPositionDescriptionsQueryHandler(IPositionDescriptionRepositoryAsync positionDescriptionRepository, IModelHelper modelHelper)
        {
            _positionDescriptionRepository = positionDescriptionRepository;
            _modelHelper = modelHelper;
        }

        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(GetPositionDescriptionsQuery request, CancellationToken cancellationToken)
        {
            var validFilter = new GetPositionDescriptionsQuery();
            validFilter.PageNumber = request.PageNumber;
            validFilter.PageSize = request.PageSize;
            validFilter.PdNbr = request.PdNbr;
            validFilter.PdPositionTitleText = request.PdPositionTitleText;
            validFilter.GvtOccSeries = request.GvtOccSeries;
            validFilter.GvtPayPlan = request.GvtPayPlan;
            validFilter.PdsStateCd = request.PdsStateCd;

            string fields = null;
            if (string.IsNullOrWhiteSpace(request.Fields))
            {
                fields = _modelHelper.GetModelFields<GetPositionDescriptionsViewModel>();
            }
            else
            {
                fields = _modelHelper.ValidateModelFields<GetPositionDescriptionsViewModel>(request.Fields);
                if (string.IsNullOrWhiteSpace(fields))
                {
                    fields = _modelHelper.GetModelFields<GetPositionDescriptionsViewModel>();
                }
            }
            validFilter.Fields = fields;

            string orderBy = null;
            if (string.IsNullOrWhiteSpace(request.OrderBy))
            {
                validFilter.OrderBy = orderBy;
            }
            else
            {
                orderBy = _modelHelper.ValidateModelFields<GetPositionDescriptionsViewModel>(request.OrderBy);
                validFilter.OrderBy = orderBy;
            }

            var objWithRecordsCount = await _positionDescriptionRepository.GetPositionDescriptionReponseAsync(validFilter);
            return new PagedResponse<IEnumerable<Entity>>(objWithRecordsCount.data, validFilter.PageNumber, validFilter.PageSize, objWithRecordsCount.recordsCount);
        }
    }
}