namespace TalentManagement.Application.Features.Departments.Queries.GetDepartmentById
{
    public class GetDepartmentByIdQuery : IRequest<Response<Department>>
    {
        public Guid Id { get; set; }

        public class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, Response<Department>>
        {
            private readonly IDepartmentRepositoryAsync _repository;

            public GetDepartmentByIdQueryHandler(IDepartmentRepositoryAsync repository)
            {
                _repository = repository;
            }

            public async Task<Response<Department>> Handle(GetDepartmentByIdQuery query, CancellationToken cancellationToken)
            {
                var entity = await _repository.GetByIdAsync(query.Id);
                if (entity == null) throw new ApiException($"Department Not Found.");
                return new Response<Department>(entity);
            }
        }
    }
}