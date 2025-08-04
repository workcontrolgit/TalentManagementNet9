namespace TalentManagement.Application.Features.SalaryRanges.Commands.CreateSalaryRange
{
    public partial class CreateSalaryRangeCommand : IRequest<Response<Guid>>
    {
        public string Name { get; set; }
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
    }

    public class CreateSalaryRangeCommandHandler : IRequestHandler<CreateSalaryRangeCommand, Response<Guid>>
    {
        private readonly ISalaryRangeRepositoryAsync _repository;
        private readonly IMapper _mapper;

        public CreateSalaryRangeCommandHandler(ISalaryRangeRepositoryAsync repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Response<Guid>> Handle(CreateSalaryRangeCommand request, CancellationToken cancellationToken)
        {
            var salaryRange = _mapper.Map<SalaryRange>(request);
            await _repository.AddAsync(salaryRange);
            return new Response<Guid>(salaryRange.Id);
        }
    }
}