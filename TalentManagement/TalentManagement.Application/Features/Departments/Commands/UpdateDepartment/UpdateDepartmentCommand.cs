namespace TalentManagement.Application.Features.Departments.Commands.UpdateDepartment
{
    public class UpdateDepartmentCommand : IRequest<Response<Guid>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, Response<Guid>>
    {
        private readonly IDepartmentRepositoryAsync _repository;

        public UpdateDepartmentCommandHandler(IDepartmentRepositoryAsync repository)
        {
            _repository = repository;
        }

        public async Task<Response<Guid>> Handle(UpdateDepartmentCommand command, CancellationToken cancellationToken)
        {
            var department = await _repository.GetByIdAsync(command.Id);

            if (department == null)
            {
                throw new ApiException($"Department Not Found.");
            }
            else
            {
                department.Name = command.Name;
                await _repository.UpdateAsync(department);
                return new Response<Guid>(department.Id);
            }
        }
    }
}