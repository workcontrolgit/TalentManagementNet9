namespace TalentManagement.Application.Features.SalaryRanges.Commands.UpdateSalaryRange
{
    public class UpdateSalaryRangeCommand : IRequest<Response<Guid>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
    }

    public class UpdateSalaryRangeCommandHandler : IRequestHandler<UpdateSalaryRangeCommand, Response<Guid>>
    {
        private readonly ISalaryRangeRepositoryAsync _repository;

        public UpdateSalaryRangeCommandHandler(ISalaryRangeRepositoryAsync repository)
        {
            _repository = repository;
        }

        public async Task<Response<Guid>> Handle(UpdateSalaryRangeCommand command, CancellationToken cancellationToken)
        {
            var salaryRange = await _repository.GetByIdAsync(command.Id);

            if (salaryRange == null)
            {
                throw new ApiException($"SalaryRange Not Found.");
            }
            else
            {
                salaryRange.Name = command.Name;
                salaryRange.MinSalary = command.MinSalary;
                salaryRange.MaxSalary = command.MaxSalary;
                await _repository.UpdateAsync(salaryRange);
                return new Response<Guid>(salaryRange.Id);
            }
        }
    }
}