namespace TalentManagement.Application.Features.Employees.Commands.CreateEmployee
{
    public partial class CreateEmployeeCommand : IRequest<Response<Guid>>
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public Guid PositionId { get; set; }
        public decimal Salary { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public string EmployeeNumber { get; set; }
        public string Prefix { get; set; }
        public string Phone { get; set; }
    }

    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Response<Guid>>
    {
        private readonly IEmployeeRepositoryAsync _repository;
        private readonly IMapper _mapper;

        public CreateEmployeeCommandHandler(IEmployeeRepositoryAsync repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Response<Guid>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = _mapper.Map<Employee>(request);
            await _repository.AddAsync(employee);
            return new Response<Guid>(employee.Id);
        }
    }
}