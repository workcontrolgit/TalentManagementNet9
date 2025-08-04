namespace TalentManagement.Application.Features.Departments.Commands.DeleteDepartment
{
    public class DeleteDepartmentCommand : IRequest<Response<Guid>>
    {
        public Guid Id { get; set; }
    }

    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Response<Guid>>
    {
        private readonly IDepartmentRepositoryAsync _repository;

        public DeleteDepartmentCommandHandler(IDepartmentRepositoryAsync repository)
        {
            _repository = repository;
        }

        public async Task<Response<Guid>> Handle(DeleteDepartmentCommand command, CancellationToken cancellationToken)
        {
            var department = await _repository.GetByIdAsync(command.Id);
            if (department == null) throw new ApiException($"Department Not Found.");
            await _repository.DeleteAsync(department);
            return new Response<Guid>(department.Id);
        }
    }
}