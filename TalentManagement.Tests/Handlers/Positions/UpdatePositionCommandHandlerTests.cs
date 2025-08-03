using Moq;
using TalentManagement.Application.Exceptions;
using TalentManagement.Application.Features.Positions.Commands.UpdatePosition;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Application.Wrappers;
using TalentManagement.Domain.Entities;

namespace TalentManagement.Tests.Handlers.Positions
{
    public class UpdatePositionCommandHandlerTests
    {
        private readonly Mock<IPositionRepositoryAsync> _mockRepository;
        private readonly UpdatePositionCommand.UpdatePositionCommandHandler _handler;

        public UpdatePositionCommandHandlerTests()
        {
            _mockRepository = new Mock<IPositionRepositoryAsync>();
            _handler = new UpdatePositionCommand.UpdatePositionCommandHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldUpdatePositionAndReturnSuccess()
        {
            var positionId = Guid.NewGuid();
            var command = new UpdatePositionCommand
            {
                Id = positionId,
                PositionTitle = "Senior Software Engineer",
                PositionDescription = "Updated description",
                DepartmentId = Guid.NewGuid(),
                SalaryRangeId = Guid.NewGuid()
            };

            var existingPosition = new Position
            {
                Id = positionId,
                PositionTitle = "Software Engineer",
                PositionDescription = "Original description",
                DepartmentId = Guid.NewGuid(),
                SalaryRangeId = Guid.NewGuid()
            };

            _mockRepository.Setup(r => r.GetByIdAsync(positionId)).ReturnsAsync(existingPosition);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Position>())).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsType<Response<Guid>>(result);
            Assert.Equal(positionId, result.Data);
            Assert.True(result.Succeeded);

            Assert.Equal(command.PositionTitle, existingPosition.PositionTitle);
            Assert.Equal(command.PositionDescription, existingPosition.PositionDescription);

            _mockRepository.Verify(r => r.GetByIdAsync(positionId), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(existingPosition), Times.Once);
        }

        [Fact]
        public async Task Handle_PositionNotFound_ShouldThrowApiException()
        {
            var positionId = Guid.NewGuid();
            var command = new UpdatePositionCommand
            {
                Id = positionId,
                PositionTitle = "Senior Software Engineer",
                PositionDescription = "Updated description"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(positionId)).ReturnsAsync((Position)null);

            var exception = await Assert.ThrowsAsync<ApiException>(() => 
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Position Not Found.", exception.Message);
            _mockRepository.Verify(r => r.GetByIdAsync(positionId), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Position>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NullRequest_ShouldThrowNullReferenceException()
        {
            await Assert.ThrowsAsync<NullReferenceException>(() => 
                _handler.Handle(null, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_RepositoryGetThrowsException_ShouldPropagateException()
        {
            var positionId = Guid.NewGuid();
            var command = new UpdatePositionCommand { Id = positionId };

            _mockRepository.Setup(r => r.GetByIdAsync(positionId))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_RepositoryUpdateThrowsException_ShouldPropagateException()
        {
            var positionId = Guid.NewGuid();
            var command = new UpdatePositionCommand
            {
                Id = positionId,
                PositionTitle = "Senior Software Engineer",
                PositionDescription = "Updated description"
            };

            var existingPosition = new Position { Id = positionId };
            _mockRepository.Setup(r => r.GetByIdAsync(positionId)).ReturnsAsync(existingPosition);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Position>()))
                .ThrowsAsync(new InvalidOperationException("Update failed"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_EmptyGuidId_ShouldStillCallRepository()
        {
            var command = new UpdatePositionCommand
            {
                Id = Guid.Empty,
                PositionTitle = "Senior Software Engineer"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(Guid.Empty)).ReturnsAsync((Position)null);

            var exception = await Assert.ThrowsAsync<ApiException>(() => 
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Position Not Found.", exception.Message);
            _mockRepository.Verify(r => r.GetByIdAsync(Guid.Empty), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task Handle_EmptyPositionTitle_ShouldUpdateWithEmptyValue(string title)
        {
            var positionId = Guid.NewGuid();
            var command = new UpdatePositionCommand
            {
                Id = positionId,
                PositionTitle = title,
                PositionDescription = "Updated description"
            };

            var existingPosition = new Position
            {
                Id = positionId,
                PositionTitle = "Original Title",
                PositionDescription = "Original description"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(positionId)).ReturnsAsync(existingPosition);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Position>())).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(title, existingPosition.PositionTitle);
            Assert.Equal(command.PositionDescription, existingPosition.PositionDescription);
            Assert.True(result.Succeeded);
        }
    }
}