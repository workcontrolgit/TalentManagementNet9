namespace TalentManagement.Application.Features.SalaryRanges.Queries.GetSalaryRanges
{
    public partial class PagedSalaryRangesQuery : QueryParameter, IRequest<PagedDataTableResponse<IEnumerable<Entity>>>
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public IList<SortOrder> Order { get; set; }
        public Search Search { get; set; }
        public IList<Column> Columns { get; set; }
    }

    public class PagedSalaryRangeQueryHandler : IRequestHandler<PagedSalaryRangesQuery, PagedDataTableResponse<IEnumerable<Entity>>>
    {
        private readonly ISalaryRangeRepositoryAsync _repository;
        private readonly IModelHelper _modelHelper;

        public PagedSalaryRangeQueryHandler(ISalaryRangeRepositoryAsync repository, IMapper mapper, IModelHelper modelHelper)
        {
            _repository = repository;
            _modelHelper = modelHelper;
        }

        public async Task<PagedDataTableResponse<IEnumerable<Entity>>> Handle(PagedSalaryRangesQuery request, CancellationToken cancellationToken)
        {
            var objRequest = request;

            objRequest.PageNumber = (request.Start / request.Length) + 1;
            objRequest.PageSize = request.Length;

            if (request.Order != null && request.Order.Count > 0)
            {
                var colOrder = request.Order[0];
                switch (colOrder.Column)
                {
                    case 0:
                        objRequest.OrderBy = colOrder.Dir == "asc" ? "Name" : "Name DESC";
                        break;
                    case 1:
                        objRequest.OrderBy = colOrder.Dir == "asc" ? "MinSalary" : "MinSalary DESC";
                        break;
                    case 2:
                        objRequest.OrderBy = colOrder.Dir == "asc" ? "MaxSalary" : "MaxSalary DESC";
                        break;
                }
            }
            else
            {
                objRequest.OrderBy = "Name";
            }

            if (string.IsNullOrEmpty(objRequest.Fields))
            {
                objRequest.Fields = _modelHelper.GetModelFields<GetSalaryRangesViewModel>();
            }

            var qryResult = await _repository.PagedSalaryRangeReponseAsync(objRequest);
            var data = qryResult.data;
            RecordsCount recordCount = qryResult.recordsCount;

            return new PagedDataTableResponse<IEnumerable<Entity>>(data, request.Draw, recordCount);
        }
    }
}