namespace TalentManagement.Application.Features.Departments.Queries.GetDepartments
{
    /// <summary>
    /// GetAllDepartmentsQuery - handles media IRequest
    /// BaseRequestParameter - contains paging parameters
    /// To add filter/search parameters, add search properties to the body of this class
    /// </summary>
    public class GetDepartmentsQuery : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {
        /// <summary>
        /// Property to hold the department name for filtering.
        /// </summary>
        public string Name { get; set; }
    }

    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly IDepartmentRepositoryAsync _repository;
        private readonly IModelHelper _modelHelper;

        /// <summary>
        /// Constructor for GetAllDepartmentsQueryHandler class.
        /// </summary>
        /// <param name="repository">IDepartmentRepositoryAsync object.</param>
        /// <param name="modelHelper">IModelHelper object.</param>
        /// <returns>
        /// GetAllDepartmentsQueryHandler object.
        /// </returns>
        public GetAllDepartmentsQueryHandler(IDepartmentRepositoryAsync repository, IModelHelper modelHelper)
        {
            _repository = repository;
            _modelHelper = modelHelper;
        }

        /// <summary>
        /// Handles the GetDepartmentsQuery request and returns a PagedResponse containing the requested data.
        /// </summary>
        /// <param name="request">The GetDepartmentsQuery request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A PagedResponse containing the requested data.</returns>
        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var objRequest = request;
            // verify request fields are valid field and exist in the DTO
            if (!string.IsNullOrEmpty(objRequest.Fields))
            {
                //limit to fields in view model
                objRequest.Fields = _modelHelper.ValidateModelFields<GetDepartmentsViewModel>(objRequest.Fields);
            }
            if (string.IsNullOrEmpty(objRequest.Fields))
            {
                //default fields from view model
                objRequest.Fields = _modelHelper.GetModelFields<GetDepartmentsViewModel>();
            }
            // verify orderby a valid field and exist in the DTO GetDepartmentsViewModel
            if (!string.IsNullOrEmpty(objRequest.OrderBy))
            {
                //limit to fields in view model
                objRequest.OrderBy = _modelHelper.ValidateModelFields<GetDepartmentsViewModel>(objRequest.OrderBy);
            }

            // query based on filter
            var qryResult = await _repository.GetDepartmentReponseAsync(objRequest);
            var data = qryResult.data;
            RecordsCount recordCount = qryResult.recordsCount;
            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, objRequest.PageNumber, objRequest.PageSize, recordCount);
        }
    }
}