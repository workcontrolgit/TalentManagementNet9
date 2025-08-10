namespace TalentManagement.Application.Features.Positions.Queries.GetPositions
{
    public class GetPositionsCountQuery : IRequest<Response<int>>
    {
        public string PositionNumber { get; set; }
        public string PositionTitle { get; set; }
        public string Department { get; set; }
    }

    public class GetPositionsCountQueryHandler : IRequestHandler<GetPositionsCountQuery, Response<int>>
    {
        private readonly IPositionRepositoryAsync _repository;

        public GetPositionsCountQueryHandler(IPositionRepositoryAsync repository)
        {
            _repository = repository;
        }

        public async Task<Response<int>> Handle(GetPositionsCountQuery request, CancellationToken cancellationToken)
        {
            var count = await _repository.GetPositionsCountAsync(request);
            return new Response<int>(count);
        }
    }
}