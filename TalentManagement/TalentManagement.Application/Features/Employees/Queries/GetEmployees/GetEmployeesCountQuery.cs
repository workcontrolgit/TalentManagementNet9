namespace TalentManagement.Application.Features.Employees.Queries.GetEmployees
{
    public class GetEmployeesCountQuery : IRequest<Response<int>>
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
    }

    public class GetEmployeesCountQueryHandler : IRequestHandler<GetEmployeesCountQuery, Response<int>>
    {
        private readonly IEmployeeRepositoryAsync _repository;

        public GetEmployeesCountQueryHandler(IEmployeeRepositoryAsync repository)
        {
            _repository = repository;
        }

        public async Task<Response<int>> Handle(GetEmployeesCountQuery request, CancellationToken cancellationToken)
        {
            var count = await _repository.GetEmployeesCountAsync(request);
            return new Response<int>(count);
        }
    }
}