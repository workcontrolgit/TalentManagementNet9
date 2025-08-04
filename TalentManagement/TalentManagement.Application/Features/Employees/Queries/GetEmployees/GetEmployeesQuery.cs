﻿namespace TalentManagement.Application.Features.Employees.Queries.GetEmployees
{
    /// <summary>
    /// GetAllEmployeesQuery - handles media IRequest
    /// BaseRequestParameter - contains paging parameters
    /// To add filter/search parameters, add search properties to the body of this class
    /// </summary>
    public class GetEmployeesQuery : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string EmployeeNumber { get; set; }
        public string Phone { get; set; }
        public string Prefix { get; set; }
        public string PositionTitle { get; set; }
        public Gender? Gender { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public DateTime? BirthdayFrom { get; set; }
        public DateTime? BirthdayTo { get; set; }

        public ListParameter ShapeParameter { get; set; }
    }

    public class GetAllEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly IEmployeeRepositoryAsync _repository;
        private readonly IModelHelper _modelHelper;

        /// <summary>
        /// Constructor for GetAllEmployeesQueryHandler class.
        /// </summary>
        /// <param name="employeeRepository">IEmployeeRepositoryAsync object.</param>
        /// <param name="modelHelper">IModelHelper object.</param>
        /// <returns>
        /// GetAllEmployeesQueryHandler object.
        /// </returns>
        public GetAllEmployeesQueryHandler(IEmployeeRepositoryAsync employeeRepository, IModelHelper modelHelper)
        {
            _repository = employeeRepository;
            _modelHelper = modelHelper;
        }

        /// <summary>
        /// Handles the GetEmployeesQuery request and returns a PagedResponse containing the requested data.
        /// </summary>
        /// <param name="request">The GetEmployeesQuery request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A PagedResponse containing the requested data.</returns>
        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
        {
            var objRequest = request;
            //filtered fields security
            if (!string.IsNullOrEmpty(objRequest.Fields))
            {
                //limit to fields in view model
                objRequest.Fields = _modelHelper.ValidateModelFields<GetEmployeesViewModel>(objRequest.Fields);
            }
            else
            {
                //default fields from view model
                objRequest.Fields = _modelHelper.GetModelFields<GetEmployeesViewModel>();
            }
            // query based on filter
            var qryResult = await _repository.GetEmployeeResponseAsync(objRequest);
            var data = qryResult.data;
            RecordsCount recordCount = qryResult.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, objRequest.PageNumber, objRequest.PageSize, recordCount);
        }
    }
}