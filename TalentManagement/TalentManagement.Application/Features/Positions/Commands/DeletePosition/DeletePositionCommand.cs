namespace TalentManagement.Application.Features.Positions.Commands.DeletePosition
{
    // Represents a command to delete a position by its ID.
    public class DeletePositionCommand : IRequest<Response<Guid>>
    {
        // The ID of the position to be deleted.
        public Guid Id { get; set; }

        // Represents the handler for deleting a position by its ID.
        public class DeletePositionCommandHandler : IRequestHandler<DeletePositionCommand, Response<Guid>>
        {
            // The repository used to access and manipulate position data.
            private readonly IPositionRepositoryAsync _repository;

            // Constructor that initializes the command handler with the given repository.
            public DeletePositionCommandHandler(IPositionRepositoryAsync repository)
            {
                _repository = repository;
            }

            // Handles the command by deleting the specified position from the repository.
            public async Task<Response<Guid>> Handle(DeletePositionCommand command, CancellationToken cancellationToken)
            {
                // Retrieves the position with the specified ID from the repository.
                var entity = await _repository.GetByIdAsync(command.Id);
                if (entity == null) throw new ApiException($"Position Not Found.");
                // Deletes the retrieved position from the repository.
                await _repository.DeleteAsync(entity);
                // Returns a response indicating the successful deletion of the position.
                return new Response<Guid>(entity.Id);
            }
        }
    }
}