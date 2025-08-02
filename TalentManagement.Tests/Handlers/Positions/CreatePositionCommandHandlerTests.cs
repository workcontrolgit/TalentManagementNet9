using AutoMapper;
using Moq;
using TalentManagement.Application.Features.Positions.Commands.CreatePosition;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Application.Wrappers;
using TalentManagement.Domain.Entities;

namespace TalentManagement.Tests.Handlers.Positions
{
    public class CreatePositionCommandHandlerTests
    {
        private readonly Mock<IPositionRepositoryAsync> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CreatePositionCommandHandler _handler;

        public CreatePositionCommandHandlerTests()
        {
            _mockRepository = new Mock<IPositionRepositoryAsync>();
            _mockMapper = new Mock<IMapper>();
            _handler = new CreatePositionCommandHandler(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnSuccessResponse()
        {
            var command = new CreatePositionCommand
            {
                PositionTitle = "Software Engineer",
                PositionNumber = "SE001",
                PositionDescription = "Develops software applications",
                DepartmentId = Guid.NewGuid(),
                SalaryRangeId = Guid.NewGuid()
            };

            var position = new Position
            {
                Id = Guid.NewGuid(),
                PositionTitle = command.PositionTitle,
                PositionNumber = command.PositionNumber,
                PositionDescription = command.PositionDescription,
                DepartmentId = command.DepartmentId,
                SalaryRangeId = command.SalaryRangeId
            };

            _mockMapper.Setup(m => m.Map<Position>(command)).Returns(position);
            _mockRepository.Setup(r => r.AddAsync(position)).Returns(Task.FromResult(position));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsType<Response<Guid>>(result);
            Assert.Equal(position.Id, result.Data);
            Assert.True(result.Succeeded);

            _mockMapper.Verify(m => m.Map<Position>(command), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(position), Times.Once);
        }

        [Fact]
        public async Task Handle_NullRequest_ShouldThrowNullReferenceException()
        {
            await Assert.ThrowsAsync<NullReferenceException>(() => 
                _handler.Handle(null, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_MapperReturnsNull_ShouldThrowNullReferenceException()
        {
            var command = new CreatePositionCommand
            {
                PositionTitle = "Software Engineer",
                PositionNumber = "SE001"
            };

            _mockMapper.Setup(m => m.Map<Position>(command)).Returns((Position)null);

            await Assert.ThrowsAsync<NullReferenceException>(() => 
                _handler.Handle(command, CancellationToken.None));

            _mockMapper.Verify(m => m.Map<Position>(command), Times.Once);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            var command = new CreatePositionCommand
            {
                PositionTitle = "Software Engineer",
                PositionNumber = "SE001"
            };

            var position = new Position { Id = Guid.NewGuid() };
            _mockMapper.Setup(m => m.Map<Position>(command)).Returns(position);
            _mockRepository.Setup(r => r.AddAsync(position))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_CancellationRequested_ShouldHonorCancellation()
        {
            var command = new CreatePositionCommand
            {
                PositionTitle = "Software Engineer",
                PositionNumber = "SE001"
            };

            var position = new Position { Id = Guid.NewGuid() };
            _mockMapper.Setup(m => m.Map<Position>(command)).Returns(position);

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            _mockRepository.Setup(r => r.AddAsync(position))
                .Returns(Task.FromCanceled<Position>(cancellationTokenSource.Token));

            await Assert.ThrowsAsync<TaskCanceledException>(() => 
                _handler.Handle(command, cancellationTokenSource.Token));
        }
    }
}