namespace TalentManagement.Application.Features.Employees.Commands.CreateEmployee
{
    public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
    {
        private readonly IEmployeeRepositoryAsync _employeeRepository;

        public CreateEmployeeCommandValidator(IEmployeeRepositoryAsync employeeRepository)
        {
            _employeeRepository = employeeRepository;

            RuleFor(p => p.FirstName)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");

            RuleFor(p => p.LastName)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");

            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .EmailAddress().WithMessage("{PropertyName} must be a valid email address.")
                .MaximumLength(100).WithMessage("{PropertyName} must not exceed 100 characters.");

            RuleFor(p => p.EmployeeNumber)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MaximumLength(20).WithMessage("{PropertyName} must not exceed 20 characters.")
                .MustAsync(IsUniqueEmployeeNumber).WithMessage("{PropertyName} already exists.");

            RuleFor(p => p.PositionId)
                .NotEmpty().WithMessage("{PropertyName} is required.");

            RuleFor(p => p.Salary)
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

            RuleFor(p => p.Birthday)
                .LessThan(DateTime.Today).WithMessage("{PropertyName} must be in the past.");

            RuleFor(p => p.Phone)
                .MaximumLength(20).WithMessage("{PropertyName} must not exceed 20 characters.");

            RuleFor(p => p.Prefix)
                .MaximumLength(10).WithMessage("{PropertyName} must not exceed 10 characters.");

            RuleFor(p => p.MiddleName)
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
        }

        private async Task<bool> IsUniqueEmployeeNumber(string employeeNumber, CancellationToken token)
        {
            return await _employeeRepository.IsUniqueEmployeeNumberAsync(employeeNumber);
        }
    }
}