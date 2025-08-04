namespace TalentManagement.Application.Features.SalaryRanges.Commands.UpdateSalaryRange
{
    public class UpdateSalaryRangeCommandValidator : AbstractValidator<UpdateSalaryRangeCommand>
    {
        public UpdateSalaryRangeCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull();

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MaximumLength(100).WithMessage("{PropertyName} must not exceed 100 characters.");

            RuleFor(p => p.MinSalary)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

            RuleFor(p => p.MaxSalary)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.")
                .GreaterThan(p => p.MinSalary).WithMessage("{PropertyName} must be greater than MinSalary.");
        }
    }
}