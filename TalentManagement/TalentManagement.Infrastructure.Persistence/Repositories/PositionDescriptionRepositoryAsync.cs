using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using TalentManagement.Application.Features.PositionDescriptions.Queries.GetPositionDescriptions;
using TalentManagement.Application.Interfaces;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Application.Parameters;
using TalentManagement.Domain.Entities;
using TalentManagement.Infrastructure.Persistence.Contexts;
using Entity = TalentManagement.Domain.Entities.Entity;

namespace TalentManagement.Infrastructure.Persistence.Repositories
{
    public class PositionDescriptionRepositoryAsync : GenericRepositoryAsync<PositionDescription>, IPositionDescriptionRepositoryAsync
    {
        private readonly IDataShapeHelper<PositionDescription> _dataShaperPositionDescription;
        private readonly DbSet<PositionDescription> _positionDescriptions;

        public PositionDescriptionRepositoryAsync(ApplicationDbContext dbContext, IDataShapeHelper<PositionDescription> dataShaperPositionDescription) : base(dbContext)
        {
            _dataShaperPositionDescription = dataShaperPositionDescription;
            _positionDescriptions = dbContext.Set<PositionDescription>();
        }

        public async Task<PositionDescription> GetByPdSeqNumAsync(decimal pdSeqNum)
        {
            return await _positionDescriptions
                .Where(p => p.PdSeqNum == pdSeqNum)
                .FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> GetPositionDescriptionReponseAsync(GetPositionDescriptionsQuery requestParameters)
        {
            var queryable = _positionDescriptions.AsQueryable();
            var total = await queryable.CountAsync();

            // Apply filtering
            if (!string.IsNullOrWhiteSpace(requestParameters.PdNbr))
            {
                queryable = queryable.Where(x => x.PdNbr.Contains(requestParameters.PdNbr));
            }

            if (!string.IsNullOrWhiteSpace(requestParameters.PdPositionTitleText))
            {
                queryable = queryable.Where(x => x.PdPositionTitleText.Contains(requestParameters.PdPositionTitleText));
            }

            if (!string.IsNullOrWhiteSpace(requestParameters.GvtOccSeries))
            {
                queryable = queryable.Where(x => x.GvtOccSeries.Contains(requestParameters.GvtOccSeries));
            }

            if (!string.IsNullOrWhiteSpace(requestParameters.GvtPayPlan))
            {
                queryable = queryable.Where(x => x.GvtPayPlan.Contains(requestParameters.GvtPayPlan));
            }

            if (!string.IsNullOrWhiteSpace(requestParameters.PdsStateCd))
            {
                queryable = queryable.Where(x => x.PdsStateCd == requestParameters.PdsStateCd);
            }

            var filteredCount = await queryable.CountAsync();

            // Apply ordering
            if (!string.IsNullOrWhiteSpace(requestParameters.OrderBy))
            {
                queryable = queryable.OrderBy(requestParameters.OrderBy);
            }
            else
            {
                queryable = queryable.OrderBy(x => x.PdSeqNum);
            }

            // Apply paging
            queryable = queryable
                .Skip((requestParameters.PageNumber - 1) * requestParameters.PageSize)
                .Take(requestParameters.PageSize);

            var positionDescriptionList = await queryable.ToListAsync();
            var shapedData = _dataShaperPositionDescription.ShapeData(positionDescriptionList, requestParameters.Fields);

            return (shapedData, new RecordsCount { RecordsTotal = total, RecordsFiltered = filteredCount });
        }

        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> PagedPositionDescriptionReponseAsync(PagedPositionDescriptionsQuery requestParameters)
        {
            var queryable = _positionDescriptions.AsQueryable();
            var total = await queryable.CountAsync();

            // Apply search
            if (requestParameters.Search != null && !string.IsNullOrWhiteSpace(requestParameters.Search.Value))
            {
                var searchValue = requestParameters.Search.Value.ToLower();
                queryable = queryable.Where(x =>
                    x.PdNbr.ToLower().Contains(searchValue) ||
                    x.PdPositionTitleText.ToLower().Contains(searchValue) ||
                    x.PdOrgTitleText.ToLower().Contains(searchValue) ||
                    x.GvtOccSeries.ToLower().Contains(searchValue) ||
                    x.GvtPayPlan.ToLower().Contains(searchValue));
            }

            var filteredCount = await queryable.CountAsync();

            // Apply ordering
            if (requestParameters.Order != null && requestParameters.Order.Any())
            {
                var orderBy = string.Join(",", requestParameters.Order.Select(x => $"{requestParameters.Columns[x.Column].Data} {(x.Dir == "asc" ? "ascending" : "descending")}"));
                queryable = queryable.OrderBy(orderBy);
            }
            else
            {
                queryable = queryable.OrderBy(x => x.PdSeqNum);
            }

            // Apply paging
            queryable = queryable
                .Skip(requestParameters.Start)
                .Take(requestParameters.Length);

            var positionDescriptionList = await queryable.ToListAsync();
            var shapedData = _dataShaperPositionDescription.ShapeData(positionDescriptionList, "");

            return (shapedData, new RecordsCount { RecordsTotal = total, RecordsFiltered = filteredCount });
        }
    }
}