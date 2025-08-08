using TalentManagement.Application.Features.Departments.Queries.GetDepartments;

namespace TalentManagement.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Represents a repository for asynchronous operations on the Department entity.
    /// </summary>
    public class DepartmentRepositoryAsync : GenericRepositoryAsync<Department>, IDepartmentRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Department> _repository;
        private readonly IDataShapeHelper<Department> _dataShaper;

        /// <summary>
        /// Initializes a new instance of the DepartmentRepositoryAsync class with the provided database context.
        /// </summary>
        /// <param name="dbContext">The application's DbContext.</param>
        public DepartmentRepositoryAsync(ApplicationDbContext dbContext, IDataShapeHelper<Department> dataShaper) : base(dbContext)
        {
            _dbContext = dbContext;
            _repository = dbContext.Set<Department>();
            _dataShaper = dataShaper;
        }

        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> GetDepartmentReponseAsync(GetDepartmentsQuery requestParameters)
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
                // Map ViewModel fields to Entity fields, excluding any properties that don't exist on Entity
                var entityFields = MapViewModelFieldsToEntityFields(fields);
                if (!string.IsNullOrWhiteSpace(entityFields))
                {
                    result = result.Select<Department>("new(" + entityFields + ")");
                }
            }

            result = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var resultData = await result.ToListAsync();
            var shapeData = _dataShaper.ShapeData(resultData, fields);

            return (shapeData, recordsCount);
        }

        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> PagedDepartmentReponseAsync(PagedDepartmentsQuery requestParameters)
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
                // Map ViewModel fields to Entity fields, excluding any properties that don't exist on Entity
                var entityFields = MapViewModelFieldsToEntityFields(fields);
                if (!string.IsNullOrWhiteSpace(entityFields))
                {
                    result = result.Select<Department>("new(" + entityFields + ")");
                }
            }

            result = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var resultData = await result.ToListAsync();
            var shapeData = _dataShaper.ShapeData(resultData, fields);

            return (shapeData, recordsCount);
        }

        /// <summary>
        /// Gets the count of departments based on the provided filter parameters asynchronously.
        /// </summary>
        /// <param name="requestParameters">The filter parameters.</param>
        /// <returns>A task that represents the asynchronous operation and returns the count of departments.</returns>
        public async Task<int> GetDepartmentsCountAsync(GetDepartmentsCountQuery requestParameters)
        {
            var result = _repository
                .AsNoTracking()
                .AsExpandable();

            FilterByColumn(ref result, requestParameters.Name);

            return await result.CountAsync();
        }

        private void FilterByColumn(ref IQueryable<Department> qry, string keyword)
        {
            if (!qry.Any())
                return;

            if (string.IsNullOrEmpty(keyword))
                return;

            var predicate = PredicateBuilder.New<Department>();

            if (!string.IsNullOrEmpty(keyword))
                predicate = predicate.Or(d => d.Name.Contains(keyword.Trim()));

            qry = qry.Where(predicate);
        }

        /// <summary>
        /// Maps ViewModel field names to Entity field names, excluding any properties that don't exist on the Entity.
        /// </summary>
        /// <param name="fields">Comma-separated field names from ViewModel</param>
        /// <returns>Comma-separated field names that exist on the Entity</returns>
        private string MapViewModelFieldsToEntityFields(string fields)
        {
            var fieldArray = fields.Split(',');
            var entityFields = new List<string>();

            foreach (var field in fieldArray)
            {
                var trimmedField = field.Trim();
                
                // For Department, all ViewModel properties exist on Entity, but we keep this method for consistency
                // and future-proofing in case computed properties are added later
                entityFields.Add(trimmedField);
            }

            return string.Join(",", entityFields);
        }
    }
}