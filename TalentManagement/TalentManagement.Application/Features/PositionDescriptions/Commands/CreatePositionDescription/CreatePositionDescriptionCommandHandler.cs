using AutoMapper;
using MediatR;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Application.Wrappers;
using TalentManagement.Domain.Entities;

namespace TalentManagement.Application.Features.PositionDescriptions.Commands.CreatePositionDescription
{
    public class CreatePositionDescriptionCommandHandler : IRequestHandler<CreatePositionDescriptionCommand, Response<decimal>>
    {
        private readonly IPositionDescriptionRepositoryAsync _positionDescriptionRepository;
        private readonly IMapper _mapper;

        public CreatePositionDescriptionCommandHandler(IPositionDescriptionRepositoryAsync positionDescriptionRepository, IMapper mapper)
        {
            _positionDescriptionRepository = positionDescriptionRepository;
            _mapper = mapper;
        }

        public async Task<Response<decimal>> Handle(CreatePositionDescriptionCommand request, CancellationToken cancellationToken)
        {
            var positionDescription = _mapper.Map<PositionDescription>(request);
            
            // Set audit fields
            positionDescription.PdCreateDate = DateTime.UtcNow;
            positionDescription.PdUpdateDate = DateTime.UtcNow;
            
            // Generate new sequence number (you may need to adjust this based on your sequence logic)
            // For now, using a simple approach - in production you might want to use a proper sequence generator
            var maxSeqNum = await _positionDescriptionRepository.GetAllAsync();
            positionDescription.PdSeqNum = maxSeqNum.Any() ? maxSeqNum.Max(x => x.PdSeqNum) + 1 : 1;

            await _positionDescriptionRepository.AddAsync(positionDescription);
            return new Response<decimal>(positionDescription.PdSeqNum, "Position Description created successfully.");
        }
    }
}