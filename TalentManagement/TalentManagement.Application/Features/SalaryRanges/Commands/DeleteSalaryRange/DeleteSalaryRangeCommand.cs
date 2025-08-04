namespace TalentManagement.Application.Features.SalaryRanges.Commands.DeleteSalaryRange
{
    public class DeleteSalaryRangeCommand : IRequest<Response<Guid>>
    {
        public Guid Id { get; set; }
    }

    public class DeleteSalaryRangeCommandHandler : IRequestHandler<DeleteSalaryRangeCommand, Response<Guid>>
    {
        private readonly ISalaryRangeRepositoryAsync _repository;

        public DeleteSalaryRangeCommandHandler(ISalaryRangeRepositoryAsync repository)
        {
            _repository = repository;
        }

        public async Task<Response<Guid>> Handle(DeleteSalaryRangeCommand command, CancellationToken cancellationToken)
        {
            var salaryRange = await _repository.GetByIdAsync(command.Id);
            if (salaryRange == null) throw new ApiException($"SalaryRange Not Found.");
            await _repository.DeleteAsync(salaryRange);
            return new Response<Guid>(salaryRange.Id);
        }
    }
}