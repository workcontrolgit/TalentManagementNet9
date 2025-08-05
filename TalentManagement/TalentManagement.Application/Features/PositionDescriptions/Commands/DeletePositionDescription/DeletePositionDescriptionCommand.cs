using MediatR;
using TalentManagement.Application.Wrappers;

namespace TalentManagement.Application.Features.PositionDescriptions.Commands.DeletePositionDescription
{
    public class DeletePositionDescriptionCommand : IRequest<Response<Unit>>
    {
        public decimal PdSeqNum { get; set; }
    }
}