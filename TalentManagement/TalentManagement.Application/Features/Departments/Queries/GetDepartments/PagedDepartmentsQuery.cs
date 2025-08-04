namespace TalentManagement.Application.Features.Departments.Queries.GetDepartments
{
    public partial class PagedDepartmentsQuery : QueryParameter, IRequest<PagedDataTableResponse<IEnumerable<Entity>>>
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public IList<SortOrder> Order { get; set; }
        public Search Search { get; set; }
        public IList<Column> Columns { get; set; }
    }

    public class PagedDepartmentQueryHandler : IRequestHandler<PagedDepartmentsQuery, PagedDataTableResponse<IEnumerable<Entity>>>
    {
        private readonly IDepartmentRepositoryAsync _repository;
        private readonly IModelHelper _modelHelper;

        public PagedDepartmentQueryHandler(IDepartmentRepositoryAsync repository, IMapper mapper, IModelHelper modelHelper)
        {
            _repository = repository;
            _modelHelper = modelHelper;
        }

        public async Task<PagedDataTableResponse<IEnumerable<Entity>>> Handle(PagedDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var objRequest = request;

            objRequest.PageNumber = (request.Start / request.Length) + 1;
            objRequest.PageSize = request.Length;

            var colOrder = request.Order[0];
            switch (colOrder.Column)
            {
                case 0:
                    objRequest.OrderBy = colOrder.Dir == "asc" ? "Name" : "Name DESC";
                    break;
            }

            if (string.IsNullOrEmpty(objRequest.Fields))
            {
                objRequest.Fields = _modelHelper.GetModelFields<GetDepartmentsViewModel>();
            }

            var qryResult = await _repository.PagedDepartmentReponseAsync(objRequest);
            var data = qryResult.data;
            RecordsCount recordCount = qryResult.recordsCount;

            return new PagedDataTableResponse<IEnumerable<Entity>>(data, request.Draw, recordCount);
        }
    }
}