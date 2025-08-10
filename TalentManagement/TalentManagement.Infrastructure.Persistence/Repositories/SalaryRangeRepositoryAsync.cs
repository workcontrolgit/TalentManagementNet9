using TalentManagement.Application.Features.SalaryRanges.Queries.GetSalaryRanges;

namespace TalentManagement.Infrastructure.Persistence.Repositories
{
    // Repository class for handling operations related to the SalaryRange entity asynchronously
    public class SalaryRangeRepositoryAsync : GenericRepositoryAsync<SalaryRange>, ISalaryRangeRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<SalaryRange> _repository;
        private readonly IDataShapeHelper<SalaryRange> _dataShaper;

        // Constructor for the SalaryRangeRepositoryAsync class
        // Takes in the application's database context and passes it to the base class constructor
        public SalaryRangeRepositoryAsync(ApplicationDbContext dbContext, IDataShapeHelper<SalaryRange> dataShaper) : base(dbContext)
        {
            _dbContext = dbContext;
            _repository = dbContext.Set<SalaryRange>();
            _dataShaper = dataShaper;
        }

        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> GetSalaryRangeReponseAsync(GetSalaryRangesQuery requestParameters)
        {
            var name = requestParameters.Name;

            var pageNumber = requestParameters.PageNumber;
            var pageSize = requestParameters.PageSize;
            var orderBy = requestParameters.OrderBy;
            var fields = requestParameters.Fields;

            int recordsTotal, recordsFiltered;

            var result = _repository
                .AsNoTracking()
                .AsExpandable();

            recordsTotal = await result.CountAsync();

            FilterByColumn(ref result, name);

            recordsFiltered = await result.CountAsync();

            var recordsCount = new RecordsCount
            {
                RecordsFiltered = recordsFiltered,
                RecordsTotal = recordsTotal
            };

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                result = result.OrderBy(orderBy);
            }

            if (!string.IsNullOrWhiteSpace(fields))
            {
                result = result.Select<SalaryRange>("new(" + fields + ")");
            }

            result = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var resultData = await result.ToListAsync();
            var shapeData = _dataShaper.ShapeData(resultData, fields);

            return (shapeData, recordsCount);
        }

        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> PagedSalaryRangeReponseAsync(PagedSalaryRangesQuery requestParameters)
        {
            var pageNumber = requestParameters.PageNumber;
            var pageSize = requestParameters.PageSize;
            var orderBy = requestParameters.OrderBy;
            var fields = requestParameters.Fields;

            int recordsTotal, recordsFiltered;

            var result = _repository
                .AsNoTracking()
                .AsExpandable();

            recordsTotal = await result.CountAsync();

            FilterByColumn(ref result, requestParameters.Search.Value);

            recordsFiltered = await result.CountAsync();

            var recordsCount = new RecordsCount
            {
                RecordsFiltered = recordsFiltered,
                RecordsTotal = recordsTotal
            };

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                result = result.OrderBy(orderBy);
            }

            if (!string.IsNullOrWhiteSpace(fields))
            {
                result = result.Select<SalaryRange>("new(" + fields + ")");
            }

            result = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var resultData = await result.ToListAsync();
            var shapeData = _dataShaper.ShapeData(resultData, fields);

            return (shapeData, recordsCount);
        }

        /// <summary>
        /// Gets the count of salary ranges based on the provided filter parameters asynchronously.
        /// </summary>
        /// <param name="requestParameters">The filter parameters.</param>
        /// <returns>A task that represents the asynchronous operation and returns the count of salary ranges.</returns>
        public async Task<int> GetSalaryRangesCountAsync(GetSalaryRangesCountQuery requestParameters)
        {
            var result = _repository
                .AsNoTracking()
                .AsExpandable();

            FilterByColumn(ref result, requestParameters.Name);

            return await result.CountAsync();
        }

        private void FilterByColumn(ref IQueryable<SalaryRange> qry, string keyword)
        {
            if (!qry.Any())
                return;

            if (string.IsNullOrEmpty(keyword))
                return;

            var predicate = PredicateBuilder.New<SalaryRange>();

            if (!string.IsNullOrEmpty(keyword))
                predicate = predicate.Or(s => s.Name.Contains(keyword.Trim()));

            qry = qry.Where(predicate);
        }
    }
}