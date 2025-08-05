using AutoMapper;
using MediatR;
using TalentManagement.Application.Exceptions;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Application.Wrappers;
using TalentManagement.Domain.Entities;

namespace TalentManagement.Application.Features.PositionDescriptions.Commands.UpdatePositionDescription
{
    public class UpdatePositionDescriptionCommandHandler : IRequestHandler<UpdatePositionDescriptionCommand, Response<Unit>>
    {
        private readonly IPositionDescriptionRepositoryAsync _positionDescriptionRepository;
        private readonly IMapper _mapper;

        public UpdatePositionDescriptionCommandHandler(IPositionDescriptionRepositoryAsync positionDescriptionRepository, IMapper mapper)
        {
            _positionDescriptionRepository = positionDescriptionRepository;
            _mapper = mapper;
        }

        public async Task<Response<Unit>> Handle(UpdatePositionDescriptionCommand request, CancellationToken cancellationToken)
        {
            var positionDescription = await _positionDescriptionRepository.GetByPdSeqNumAsync(request.PdSeqNum);

            if (positionDescription == null)
            {
                throw new ApiException($"Position Description with PdSeqNum {request.PdSeqNum} not found.");
            }

            _mapper.Map(request, positionDescription);
            
            // Update audit fields
            positionDescription.PdUpdateDate = DateTime.UtcNow;

            await _positionDescriptionRepository.UpdateAsync(positionDescription);
            return new Response<Unit>(Unit.Value, "Position Description updated successfully.");
        }
    }
}