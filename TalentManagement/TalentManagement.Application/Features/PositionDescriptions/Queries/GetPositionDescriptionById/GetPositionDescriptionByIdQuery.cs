using AutoMapper;
using MediatR;
using TalentManagement.Application.Exceptions;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Application.Wrappers;

namespace TalentManagement.Application.Features.PositionDescriptions.Queries.GetPositionDescriptionById
{
    public class GetPositionDescriptionByIdQuery : IRequest<Response<GetPositionDescriptionByIdViewModel>>
    {
        public decimal PdSeqNum { get; set; }

        public class GetPositionDescriptionByIdQueryHandler : IRequestHandler<GetPositionDescriptionByIdQuery, Response<GetPositionDescriptionByIdViewModel>>
        {
            private readonly IPositionDescriptionRepositoryAsync _positionDescriptionRepository;
            private readonly IMapper _mapper;

            public GetPositionDescriptionByIdQueryHandler(IPositionDescriptionRepositoryAsync positionDescriptionRepository, IMapper mapper)
            {
                _positionDescriptionRepository = positionDescriptionRepository;
                _mapper = mapper;
            }

            public async Task<Response<GetPositionDescriptionByIdViewModel>> Handle(GetPositionDescriptionByIdQuery query, CancellationToken cancellationToken)
            {
                var positionDescription = await _positionDescriptionRepository.GetByPdSeqNumAsync(query.PdSeqNum);
                if (positionDescription == null)
                {
                    throw new ApiException($"Position Description with PdSeqNum {query.PdSeqNum} not found.");
                }
                var positionDescriptionViewModel = _mapper.Map<GetPositionDescriptionByIdViewModel>(positionDescription);
                return new Response<GetPositionDescriptionByIdViewModel>(positionDescriptionViewModel);
            }
        }
    }
}