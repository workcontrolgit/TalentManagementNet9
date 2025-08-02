using Moq;
using TalentManagement.Application.Exceptions;
using TalentManagement.Application.Features.Positions.Commands.DeletePosition;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Application.Wrappers;
using TalentManagement.Domain.Entities;

namespace TalentManagement.Tests.Handlers.Positions
{
    public class DeletePositionCommandHandlerTests
    {
        private readonly Mock<IPositionRepositoryAsync> _mockRepository;
        private readonly DeletePositionCommand.DeletePositionCommandHandler _handler;

        public DeletePositionCommandHandlerTests()
        {
            _mockRepository = new Mock<IPositionRepositoryAsync>();
            _handler = new DeletePositionCommand.DeletePositionCommandHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldDeletePositionAndReturnSuccess()
        {
            var positionId = Guid.NewGuid();
            var command = new DeletePositionCommand { Id = positionId };

            var existingPosition = new Position
            {
                Id = positionId,
                PositionTitle = "Software Engineer",
                PositionNumber = "SE001",
                PositionDescription = "Develops software applications"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(positionId)).ReturnsAsync(existingPosition);
            _mockRepository.Setup(r => r.DeleteAsync(existingPosition)).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsType<Response<Guid>>(result);
            Assert.Equal(positionId, result.Data);
            Assert.True(result.Succeeded);

            _mockRepository.Verify(r => r.GetByIdAsync(positionId), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync(existingPosition), Times.Once);
        }

        [Fact]
        public async Task Handle_PositionNotFound_ShouldThrowApiException()
        {
            var positionId = Guid.NewGuid();
            var command = new DeletePositionCommand { Id = positionId };

            _mockRepository.Setup(r => r.GetByIdAsync(positionId)).ReturnsAsync((Position)null);

            var exception = await Assert.ThrowsAsync<ApiException>(() => 
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Position Not Found.", exception.Message);
            _mockRepository.Verify(r => r.GetByIdAsync(positionId), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Position>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NullRequest_ShouldThrowNullReferenceException()
        {
            await Assert.ThrowsAsync<NullReferenceException>(() => 
                _handler.Handle(null, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_EmptyGuidId_ShouldStillCallRepository()
        {
            var command = new DeletePositionCommand { Id = Guid.Empty };

            _mockRepository.Setup(r => r.GetByIdAsync(Guid.Empty)).ReturnsAsync((Position)null);

            var exception = await Assert.ThrowsAsync<ApiException>(() => 
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Position Not Found.", exception.Message);
            _mockRepository.Verify(r => r.GetByIdAsync(Guid.Empty), Times.Once);
        }

        [Fact]
        public async Task Handle_RepositoryGetThrowsException_ShouldPropagateException()
        {
            var positionId = Guid.NewGuid();
            var command = new DeletePositionCommand { Id = positionId };

            _mockRepository.Setup(r => r.GetByIdAsync(positionId))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _handler.Handle(command, CancellationToken.None));

            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Position>()), Times.Never);
        }

        [Fact]
        public async Task Handle_RepositoryDeleteThrowsException_ShouldPropagateException()
        {
            var positionId = Guid.NewGuid();
            var command = new DeletePositionCommand { Id = positionId };

            var existingPosition = new Position { Id = positionId };
            _mockRepository.Setup(r => r.GetByIdAsync(positionId)).ReturnsAsync(existingPosition);
            _mockRepository.Setup(r => r.DeleteAsync(existingPosition))
                .ThrowsAsync(new InvalidOperationException("Delete failed"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _handler.Handle(command, CancellationToken.None));

            _mockRepository.Verify(r => r.GetByIdAsync(positionId), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync(existingPosition), Times.Once);
        }

        [Fact]
        public async Task Handle_CancellationRequested_ShouldHonorCancellation()
        {
            var positionId = Guid.NewGuid();
            var command = new DeletePositionCommand { Id = positionId };

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            _mockRepository.Setup(r => r.GetByIdAsync(positionId))
                .Returns(Task.FromCanceled<Position>(cancellationTokenSource.Token));

            await Assert.ThrowsAsync<TaskCanceledException>(() => 
                _handler.Handle(command, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task Handle_ValidRequestWithComplexPosition_ShouldDeleteSuccessfully()
        {
            var positionId = Guid.NewGuid();
            var command = new DeletePositionCommand { Id = positionId };

            var existingPosition = new Position
            {
                Id = positionId,
                PositionTitle = "Senior Software Architect",
                PositionNumber = "SSA001",
                PositionDescription = "Designs complex software systems",
                DepartmentId = Guid.NewGuid(),
                SalaryRangeId = Guid.NewGuid(),
                Department = new Department { Name = "Engineering" },
                SalaryRange = new SalaryRange { MinSalary = 100000, MaxSalary = 150000 }
            };

            _mockRepository.Setup(r => r.GetByIdAsync(positionId)).ReturnsAsync(existingPosition);
            _mockRepository.Setup(r => r.DeleteAsync(existingPosition)).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(positionId, result.Data);
            Assert.True(result.Succeeded);
        }
    }
}