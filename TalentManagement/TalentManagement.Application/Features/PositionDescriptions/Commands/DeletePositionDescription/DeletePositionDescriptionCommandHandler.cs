using MediatR;
using TalentManagement.Application.Exceptions;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Application.Wrappers;

namespace TalentManagement.Application.Features.PositionDescriptions.Commands.DeletePositionDescription
{
    public class DeletePositionDescriptionCommandHandler : IRequestHandler<DeletePositionDescriptionCommand, Response<Unit>>
    {
        private readonly IPositionDescriptionRepositoryAsync _positionDescriptionRepository;

        public DeletePositionDescriptionCommandHandler(IPositionDescriptionRepositoryAsync positionDescriptionRepository)
        {
            _positionDescriptionRepository = positionDescriptionRepository;
        }

        public async Task<Response<Unit>> Handle(DeletePositionDescriptionCommand request, CancellationToken cancellationToken)
        {
            var positionDescription = await _positionDescriptionRepository.GetByPdSeqNumAsync(request.PdSeqNum);

            if (positionDescription == null)
            {
                throw new ApiException($"Position Description with PdSeqNum {request.PdSeqNum} not found.");
            }

            await _positionDescriptionRepository.DeleteAsync(positionDescription);
            return new Response<Unit>(Unit.Value, "Position Description deleted successfully.");
        }
    }
}