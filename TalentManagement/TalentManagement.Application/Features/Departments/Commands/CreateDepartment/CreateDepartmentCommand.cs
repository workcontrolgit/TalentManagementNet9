namespace TalentManagement.Application.Features.Departments.Commands.CreateDepartment
{
    public partial class CreateDepartmentCommand : IRequest<Response<Guid>>
    {
        public string Name { get; set; }
    }

    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Response<Guid>>
    {
        private readonly IDepartmentRepositoryAsync _repository;
        private readonly IMapper _mapper;

        public CreateDepartmentCommandHandler(IDepartmentRepositoryAsync repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Response<Guid>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = _mapper.Map<Department>(request);
            await _repository.AddAsync(department);
            return new Response<Guid>(department.Id);
        }
    }
}