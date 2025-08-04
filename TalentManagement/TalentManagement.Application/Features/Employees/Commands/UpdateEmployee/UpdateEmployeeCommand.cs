namespace TalentManagement.Application.Features.Employees.Commands.UpdateEmployee
{
    public class UpdateEmployeeCommand : IRequest<Response<Guid>>
    {
        public Guid Id { get; set; }
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

        public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Response<Guid>>
        {
            private readonly IEmployeeRepositoryAsync _repository;

            public UpdateEmployeeCommandHandler(IEmployeeRepositoryAsync employeeRepository)
            {
                _repository = employeeRepository;
            }

            public async Task<Response<Guid>> Handle(UpdateEmployeeCommand command, CancellationToken cancellationToken)
            {
                var employee = await _repository.GetByIdAsync(command.Id);

                if (employee == null)
                {
                    throw new ApiException($"Employee Not Found.");
                }
                else
                {
                    employee.FirstName = command.FirstName;
                    employee.MiddleName = command.MiddleName;
                    employee.LastName = command.LastName;
                    employee.PositionId = command.PositionId;
                    employee.Salary = command.Salary;
                    employee.Birthday = command.Birthday;
                    employee.Email = command.Email;
                    employee.Gender = command.Gender;
                    employee.EmployeeNumber = command.EmployeeNumber;
                    employee.Prefix = command.Prefix;
                    employee.Phone = command.Phone;

                    await _repository.UpdateAsync(employee);

                    return new Response<Guid>(employee.Id);
                }
            }
        }
    }
}