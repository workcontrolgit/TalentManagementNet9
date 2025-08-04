namespace TalentManagement.Application.Features.SalaryRanges.Queries.GetSalaryRanges
{
    /// <summary>
    /// GetAllSalaryRangesQuery - handles media IRequest
    /// BaseRequestParameter - contains paging parameters
    /// To add filter/search parameters, add search properties to the body of this class
    /// </summary>
    public class GetSalaryRangesQuery : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {
        /// <summary>
        /// Property to hold the salary range name for filtering.
        /// </summary>
        public string Name { get; set; }
    }

    public class GetAllSalaryRangesQueryHandler : IRequestHandler<GetSalaryRangesQuery, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly ISalaryRangeRepositoryAsync _repository;
        private readonly IModelHelper _modelHelper;

        /// <summary>
        /// Constructor for GetAllSalaryRangesQueryHandler class.
        /// </summary>
        /// <param name="repository">ISalaryRangeRepositoryAsync object.</param>
        /// <param name="modelHelper">IModelHelper object.</param>
        /// <returns>
        /// GetAllSalaryRangesQueryHandler object.
        /// </returns>
        public GetAllSalaryRangesQueryHandler(ISalaryRangeRepositoryAsync repository, IModelHelper modelHelper)
        {
            _repository = repository;
            _modelHelper = modelHelper;
        }

        /// <summary>
        /// Handles the GetSalaryRangesQuery request and returns a PagedResponse containing the requested data.
        /// </summary>
        /// <param name="request">The GetSalaryRangesQuery request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A PagedResponse containing the requested data.</returns>
        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(GetSalaryRangesQuery request, CancellationToken cancellationToken)
        {
            var objRequest = request;
            // verify request fields are valid field and exist in the DTO
            if (!string.IsNullOrEmpty(objRequest.Fields))
            {
                //limit to fields in view model
                objRequest.Fields = _modelHelper.ValidateModelFields<GetSalaryRangesViewModel>(objRequest.Fields);
            }
            if (string.IsNullOrEmpty(objRequest.Fields))
            {
                //default fields from view model
                objRequest.Fields = _modelHelper.GetModelFields<GetSalaryRangesViewModel>();
            }
            // verify orderby a valid field and exist in the DTO GetSalaryRangesViewModel
            if (!string.IsNullOrEmpty(objRequest.OrderBy))
            {
                //limit to fields in view model
                objRequest.OrderBy = _modelHelper.ValidateModelFields<GetSalaryRangesViewModel>(objRequest.OrderBy);
            }

            // query based on filter
            var qryResult = await _repository.GetSalaryRangeReponseAsync(objRequest);
            var data = qryResult.data;
            RecordsCount recordCount = qryResult.recordsCount;
            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, objRequest.PageNumber, objRequest.PageSize, recordCount);
        }
    }
}