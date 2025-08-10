namespace TalentManagement.Application.Features.Departments.Queries.GetDepartments
{
    public class GetDepartmentsCountQuery : IRequest<Response<int>>
    {
        public string Name { get; set; }
    }

    public class GetDepartmentsCountQueryHandler : IRequestHandler<GetDepartmentsCountQuery, Response<int>>
    {
        private readonly IDepartmentRepositoryAsync _repository;

        public GetDepartmentsCountQueryHandler(IDepartmentRepositoryAsync repository)
        {
            _repository = repository;
        }

        public async Task<Response<int>> Handle(GetDepartmentsCountQuery request, CancellationToken cancellationToken)
        {
            var count = await _repository.GetDepartmentsCountAsync(request);
            return new Response<int>(count);
        }
    }
}