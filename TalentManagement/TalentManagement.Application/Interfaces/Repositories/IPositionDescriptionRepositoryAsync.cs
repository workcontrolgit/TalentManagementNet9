using TalentManagement.Application.Features.PositionDescriptions.Queries.GetPositionDescriptions;
using TalentManagement.Application.Parameters;
using TalentManagement.Domain.Entities;
using Entity = TalentManagement.Domain.Entities.Entity;

namespace TalentManagement.Application.Interfaces.Repositories
{
    public interface IPositionDescriptionRepositoryAsync : IGenericRepositoryAsync<PositionDescription>
    {
        Task<PositionDescription> GetByPdSeqNumAsync(decimal pdSeqNum);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> GetPositionDescriptionReponseAsync(GetPositionDescriptionsQuery requestParameters);
        Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> PagedPositionDescriptionReponseAsync(PagedPositionDescriptionsQuery requestParameters);
    }
}