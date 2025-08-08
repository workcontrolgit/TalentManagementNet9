namespace TalentManagement.Infrastructure.Persistence.Repositories
{
    public class EmployeeRepositoryAsync : GenericRepositoryAsync<Employee>, IEmployeeRepositoryAsync
    {
        private readonly DbSet<Employee> _repository;
        private readonly IDataShapeHelper<Employee> _dataShaper;

        /// <summary>
        /// Constructor for EmployeeRepositoryAsync class.
        /// </summary>
        /// <param name="dataShaper">IDataShapeHelper object.</param>
        /// <param name="mockData">IMockService object.</param>
        /// <returns>
        ///
        /// </returns>
        public EmployeeRepositoryAsync(ApplicationDbContext dbContext,
            IDataShapeHelper<Employee> dataShaper) : base(dbContext)
        {
            _repository = dbContext.Set<Employee>();
            _dataShaper = dataShaper;
        }

        /// <summary>
        /// Retrieves a paged list of employees based on the provided query parameters.
        /// </summary>
        /// <param name="requestParameters">The query parameters used to filter and page the data.</param>
        /// <returns>A tuple containing the paged list of employees and the total number of records.</returns>
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> GetEmployeeResponseAsync(GetEmployeesQuery requestParameters)
        {
            var pageNumber = requestParameters.PageNumber;
            var pageSize = requestParameters.PageSize;
            var orderBy = requestParameters.OrderBy;
            var fields = requestParameters.Fields;

            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var result = _repository
                .Include(e => e.Position)
                .AsNoTracking()
                .AsExpandable();

            // Count records total
            recordsTotal = await result.CountAsync();

            // filter data
            FilterByColumn(ref result, requestParameters);

            // Count records after filter
            recordsFiltered = await result.CountAsync();

            //set Record counts
            var recordsCount = new RecordsCount
            {
                RecordsFiltered = recordsFiltered,
                RecordsTotal = recordsTotal
            };

            // set order by
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                result = result.OrderBy(orderBy);
            }

            //limit query fields
            if (!string.IsNullOrWhiteSpace(fields))
            {
                // Map ViewModel fields to Entity fields, excluding computed properties that don't exist on Entity
                var entityFields = MapViewModelFieldsToEntityFields(fields);
                if (!string.IsNullOrWhiteSpace(entityFields))
                {
                    result = result.Select<Employee>("new(" + entityFields + ")");
                }
            }
            // paging
            result = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = await result.ToListAsync();

            // shape data
            var shapeData = _dataShaper.ShapeData(resultData, fields);

            return (shapeData, recordsCount);
        }

        /// <summary>
        /// Checks if the given employee number is unique in the database.
        /// </summary>
        /// <param name="employeeNumber">Employee number to check for uniqueness.</param>
        /// <returns>
        /// Task indicating whether the employee number is unique.
        /// </returns>
        public async Task<bool> IsUniqueEmployeeNumberAsync(string employeeNumber)
        {
            return await _repository
                .AllAsync(p => p.EmployeeNumber != employeeNumber);
        }

        /// <summary>
        /// Retrieves a paged list of employees based on the provided query parameters.
        /// </summary>
        /// <param name="requestParameters">The query parameters used to filter and page the data.</param>
        /// <returns>A tuple containing the paged list of employees and the total number of records.</returns>
        public async Task<(IEnumerable<Entity> data, RecordsCount recordsCount)> GetPagedEmployeeResponseAsync(PagedEmployeesQuery requestParameters)
        {
            //searchable fields

            var pageNumber = requestParameters.PageNumber;
            var pageSize = requestParameters.PageSize;
            var orderBy = requestParameters.OrderBy;
            var fields = requestParameters.Fields;

            int recordsTotal, recordsFiltered;

            // Setup IQueryable
            var result = _repository
                .Include(e => e.Position)
                .AsNoTracking()
                .AsExpandable();

            // Count records total
            recordsTotal = await result.CountAsync();

            // filter data
            FilterByColumn(ref result, requestParameters.Search.Value);

            // Count records after filter
            recordsFiltered = await result.CountAsync();

            //set Record counts
            var recordsCount = new RecordsCount
            {
                RecordsFiltered = recordsFiltered,
                RecordsTotal = recordsTotal
            };

            // set order by
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                result = result.OrderBy(orderBy);
            }

            //limit query fields
            if (!string.IsNullOrWhiteSpace(fields))
            {
                // Map ViewModel fields to Entity fields, excluding computed properties that don't exist on Entity
                var entityFields = MapViewModelFieldsToEntityFields(fields);
                if (!string.IsNullOrWhiteSpace(entityFields))
                {
                    result = result.Select<Employee>("new(" + entityFields + ")");
                }
            }
            // paging
            result = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // retrieve data to list
            var resultData = await result.ToListAsync();

            // shape data
            var shapeData = _dataShaper.ShapeData(resultData, fields);

            return (shapeData, recordsCount);
        }

        /// <summary>
        /// Filters an IQueryable of employees based on the provided query parameters.
        /// </summary>
        /// <param name="qry">The IQueryable of employees to filter.</param>
        /// <param name="parameters">The query parameters containing filter criteria.</param>
        private void FilterByColumn(ref IQueryable<Employee> qry, GetEmployeesQuery parameters)
        {
            if (!qry.Any())
                return;

            var predicate = PredicateBuilder.New<Employee>();
            bool hasFilters = false;

            // String-based filters
            if (!string.IsNullOrEmpty(parameters.FirstName))
            {
                predicate = predicate.And(p => p.FirstName.ToLower().Contains(parameters.FirstName.ToLower().Trim()));
                hasFilters = true;
            }

            if (!string.IsNullOrEmpty(parameters.MiddleName))
            {
                predicate = predicate.And(p => p.MiddleName.ToLower().Contains(parameters.MiddleName.ToLower().Trim()));
                hasFilters = true;
            }

            if (!string.IsNullOrEmpty(parameters.LastName))
            {
                predicate = predicate.And(p => p.LastName.ToLower().Contains(parameters.LastName.ToLower().Trim()));
                hasFilters = true;
            }

            if (!string.IsNullOrEmpty(parameters.Email))
            {
                predicate = predicate.And(p => p.Email.ToLower().Contains(parameters.Email.ToLower().Trim()));
                hasFilters = true;
            }

            if (!string.IsNullOrEmpty(parameters.EmployeeNumber))
            {
                predicate = predicate.And(p => p.EmployeeNumber.ToLower().Contains(parameters.EmployeeNumber.ToLower().Trim()));
                hasFilters = true;
            }

            if (!string.IsNullOrEmpty(parameters.Phone))
            {
                predicate = predicate.And(p => p.Phone.ToLower().Contains(parameters.Phone.ToLower().Trim()));
                hasFilters = true;
            }

            if (!string.IsNullOrEmpty(parameters.Prefix))
            {
                predicate = predicate.And(p => p.Prefix.ToLower().Contains(parameters.Prefix.ToLower().Trim()));
                hasFilters = true;
            }

            if (!string.IsNullOrEmpty(parameters.PositionTitle))
            {
                predicate = predicate.And(p => p.Position.PositionTitle.ToLower().Contains(parameters.PositionTitle.ToLower().Trim()));
                hasFilters = true;
            }

            // Enum filter
            if (parameters.Gender.HasValue)
            {
                predicate = predicate.And(p => p.Gender == parameters.Gender.Value);
                hasFilters = true;
            }

            // Salary range filters
            if (parameters.MinSalary.HasValue)
            {
                predicate = predicate.And(p => p.Salary >= parameters.MinSalary.Value);
                hasFilters = true;
            }

            if (parameters.MaxSalary.HasValue)
            {
                predicate = predicate.And(p => p.Salary <= parameters.MaxSalary.Value);
                hasFilters = true;
            }

            // Birthday range filters
            if (parameters.BirthdayFrom.HasValue)
            {
                predicate = predicate.And(p => p.Birthday >= parameters.BirthdayFrom.Value);
                hasFilters = true;
            }

            if (parameters.BirthdayTo.HasValue)
            {
                predicate = predicate.And(p => p.Birthday <= parameters.BirthdayTo.Value);
                hasFilters = true;
            }

            if (hasFilters)
            {
                qry = qry.Where(predicate);
            }
        }

        /// <summary>
        /// Filters an IQueryable of employees based on the provided parameters.
        /// </summary>
        /// <param name="qry">The IQueryable of employees to filter.</param>
        /// <param name="keyword">The employee title to filter by.</param>
        private void FilterByColumn(ref IQueryable<Employee> qry, string keyword)
        {
            if (!qry.Any())
                return;

            if (string.IsNullOrEmpty(keyword))
                return;

            var predicate = PredicateBuilder.New<Employee>();

            predicate = predicate.Or(p => p.LastName.ToLower().Contains(keyword.ToLower().Trim()));
            predicate = predicate.Or(p => p.FirstName.ToLower().Contains(keyword.ToLower().Trim()));
            predicate = predicate.Or(p => p.Email.ToLower().Contains(keyword.ToLower().Trim()));
            predicate = predicate.Or(p => p.EmployeeNumber.ToLower().Contains(keyword.ToLower().Trim()));
            predicate = predicate.Or(p => p.Position.PositionTitle.ToLower().Contains(keyword.ToLower().Trim()));

            qry = qry.Where(predicate);
        }

        /// <summary>
        /// Gets the count of employees based on the provided filter parameters asynchronously.
        /// </summary>
        /// <param name="requestParameters">The filter parameters.</param>
        /// <returns>A task that represents the asynchronous operation and returns the count of employees.</returns>
        public async Task<int> GetEmployeesCountAsync(GetEmployeesCountQuery requestParameters)
        {
            var result = _repository
                .Include(e => e.Position)
                .AsNoTracking()
                .AsExpandable();

            var getEmployeesQuery = new GetEmployeesQuery
            {
                FirstName = requestParameters.FirstName,
                MiddleName = requestParameters.MiddleName,
                LastName = requestParameters.LastName,
                Email = requestParameters.Email,
                EmployeeNumber = requestParameters.EmployeeNumber,
                Phone = requestParameters.Phone,
                Prefix = requestParameters.Prefix,
                PositionTitle = requestParameters.PositionTitle,
                Gender = requestParameters.Gender,
                MinSalary = requestParameters.MinSalary,
                MaxSalary = requestParameters.MaxSalary,
                BirthdayFrom = requestParameters.BirthdayFrom,
                BirthdayTo = requestParameters.BirthdayTo
            };

            FilterByColumn(ref result, getEmployeesQuery);

            return await result.CountAsync();
        }

        /// <summary>
        /// Maps ViewModel field names to Entity field names, excluding computed properties that don't exist on the Entity.
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
                
                // Skip computed properties that don't exist on Employee entity
                if (trimmedField.Equals("FullName", StringComparison.OrdinalIgnoreCase) ||
                    trimmedField.Equals("PositionTitle", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                
                entityFields.Add(trimmedField);
            }

            return string.Join(",", entityFields);
        }
    }
}