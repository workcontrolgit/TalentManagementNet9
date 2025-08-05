using FluentValidation;

namespace TalentManagement.Application.Features.PositionDescriptions.Commands.UpdatePositionDescription
{
    public class UpdatePositionDescriptionCommandValidator : AbstractValidator<UpdatePositionDescriptionCommand>
    {
        public UpdatePositionDescriptionCommandValidator()
        {
            RuleFor(p => p.PdSeqNum)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

            RuleFor(p => p.PdNbr)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");

            RuleFor(p => p.PdPositionTitleText)
                .MaximumLength(200).WithMessage("{PropertyName} must not exceed 200 characters.");

            RuleFor(p => p.PdOrgTitleText)
                .MaximumLength(200).WithMessage("{PropertyName} must not exceed 200 characters.");

            RuleFor(p => p.GvtOccSeries)
                .MaximumLength(10).WithMessage("{PropertyName} must not exceed 10 characters.");

            RuleFor(p => p.GvtPayPlan)
                .MaximumLength(10).WithMessage("{PropertyName} must not exceed 10 characters.");

            RuleFor(p => p.PdsStateCd)
                .MaximumLength(10).WithMessage("{PropertyName} must not exceed 10 characters.");

            RuleFor(p => p.GrdCode)
                .MaximumLength(10).WithMessage("{PropertyName} must not exceed 10 characters.");

            RuleFor(p => p.JobFunction)
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");

            RuleFor(p => p.PdEffectiveSeqNum)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

            RuleFor(p => p.PdEffectiveDate)
                .LessThanOrEqualTo(DateTime.Today.AddYears(5))
                .WithMessage("{PropertyName} cannot be more than 5 years in the future.");

            RuleFor(p => p.PdClassifiedDate)
                .LessThanOrEqualTo(DateTime.Today)
                .WithMessage("{PropertyName} cannot be in the future.");
        }
    }
}