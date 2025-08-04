namespace TalentManagement.Application.Features.Employees.Commands.DeleteEmployee
{
    public class DeleteEmployeeCommand : IRequest<Response<Guid>>
    {
        public Guid Id { get; set; }

        public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, Response<Guid>>
        {
            private readonly IEmployeeRepositoryAsync _repository;

            public DeleteEmployeeCommandHandler(IEmployeeRepositoryAsync repository)
            {
                _repository = repository;
            }

            public async Task<Response<Guid>> Handle(DeleteEmployeeCommand command, CancellationToken cancellationToken)
            {
                var entity = await _repository.GetByIdAsync(command.Id);
                if (entity == null) throw new ApiException($"Employee Not Found.");
                await _repository.DeleteAsync(entity);
                return new Response<Guid>(entity.Id);
            }
        }
    }
}