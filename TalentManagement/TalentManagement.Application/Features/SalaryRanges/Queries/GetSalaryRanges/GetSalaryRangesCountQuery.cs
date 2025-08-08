namespace TalentManagement.Application.Features.SalaryRanges.Queries.GetSalaryRanges
{
    public class GetSalaryRangesCountQuery : IRequest<Response<int>>
    {
        public string Name { get; set; }
    }

    public class GetSalaryRangesCountQueryHandler : IRequestHandler<GetSalaryRangesCountQuery, Response<int>>
    {
        private readonly ISalaryRangeRepositoryAsync _repository;

        public GetSalaryRangesCountQueryHandler(ISalaryRangeRepositoryAsync repository)
        {
            _repository = repository;
        }

        public async Task<Response<int>> Handle(GetSalaryRangesCountQuery request, CancellationToken cancellationToken)
        {
            var count = await _repository.GetSalaryRangesCountAsync(request);
            return new Response<int>(count);
        }
    }
}