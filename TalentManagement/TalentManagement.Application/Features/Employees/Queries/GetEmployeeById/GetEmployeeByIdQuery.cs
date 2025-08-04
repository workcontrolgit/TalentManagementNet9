namespace TalentManagement.Application.Features.Employees.Queries.GetEmployeeById
{
    public class GetEmployeeByIdQuery : IRequest<Response<Employee>>
    {
        public Guid Id { get; set; }

        public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, Response<Employee>>
        {
            private readonly IEmployeeRepositoryAsync _repository;

            public GetEmployeeByIdQueryHandler(IEmployeeRepositoryAsync repository)
            {
                _repository = repository;
            }

            public async Task<Response<Employee>> Handle(GetEmployeeByIdQuery query, CancellationToken cancellationToken)
            {
                var employee = await _repository.GetByIdAsync(query.Id);
                if (employee == null) throw new ApiException($"Employee Not Found.");
                return new Response<Employee>(employee);
            }
        }
    }
}