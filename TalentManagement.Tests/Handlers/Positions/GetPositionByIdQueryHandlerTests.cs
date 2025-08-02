using Moq;
using TalentManagement.Application.Exceptions;
using TalentManagement.Application.Features.Positions.Queries.GetPositionById;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Application.Wrappers;
using TalentManagement.Domain.Entities;

namespace TalentManagement.Tests.Handlers.Positions
{
    public class GetPositionByIdQueryHandlerTests
    {
        private readonly Mock<IPositionRepositoryAsync> _mockRepository;
        private readonly GetPositionByIdQuery.GetPositionByIdQueryHandler _handler;

        public GetPositionByIdQueryHandlerTests()
        {
            _mockRepository = new Mock<IPositionRepositoryAsync>();
            _handler = new GetPositionByIdQuery.GetPositionByIdQueryHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_ValidId_ShouldReturnPositionSuccessfully()
        {
            var positionId = Guid.NewGuid();
            var query = new GetPositionByIdQuery { Id = positionId };

            var expectedPosition = new Position
            {
                Id = positionId,
                PositionTitle = "Software Engineer",
                PositionNumber = "SE001",
                PositionDescription = "Develops software applications",
                DepartmentId = Guid.NewGuid(),
                SalaryRangeId = Guid.NewGuid()
            };

            _mockRepository.Setup(r => r.GetByIdAsync(positionId)).ReturnsAsync(expectedPosition);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsType<Response<Position>>(result);
            Assert.True(result.Succeeded);
            Assert.Equal(expectedPosition, result.Data);
            Assert.Equal(positionId, result.Data.Id);
            Assert.Equal("Software Engineer", result.Data.PositionTitle);

            _mockRepository.Verify(r => r.GetByIdAsync(positionId), Times.Once);
        }

        [Fact]
        public async Task Handle_PositionNotFound_ShouldThrowApiException()
        {
            var positionId = Guid.NewGuid();
            var query = new GetPositionByIdQuery { Id = positionId };

            _mockRepository.Setup(r => r.GetByIdAsync(positionId)).ReturnsAsync((Position)null);

            var exception = await Assert.ThrowsAsync<ApiException>(() => 
                _handler.Handle(query, CancellationToken.None));

            Assert.Equal("Position Not Found.", exception.Message);
            _mockRepository.Verify(r => r.GetByIdAsync(positionId), Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyGuid_ShouldStillCallRepository()
        {
            var query = new GetPositionByIdQuery { Id = Guid.Empty };

            _mockRepository.Setup(r => r.GetByIdAsync(Guid.Empty)).ReturnsAsync((Position)null);

            var exception = await Assert.ThrowsAsync<ApiException>(() => 
                _handler.Handle(query, CancellationToken.None));

            Assert.Equal("Position Not Found.", exception.Message);
            _mockRepository.Verify(r => r.GetByIdAsync(Guid.Empty), Times.Once);
        }

        [Fact]
        public async Task Handle_NullQuery_ShouldThrowNullReferenceException()
        {
            await Assert.ThrowsAsync<NullReferenceException>(() => 
                _handler.Handle(null, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            var positionId = Guid.NewGuid();
            var query = new GetPositionByIdQuery { Id = positionId };

            _mockRepository.Setup(r => r.GetByIdAsync(positionId))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _handler.Handle(query, CancellationToken.None));

            _mockRepository.Verify(r => r.GetByIdAsync(positionId), Times.Once);
        }

        [Fact]
        public async Task Handle_CancellationRequested_ShouldHonorCancellation()
        {
            var positionId = Guid.NewGuid();
            var query = new GetPositionByIdQuery { Id = positionId };

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            _mockRepository.Setup(r => r.GetByIdAsync(positionId))
                .Returns(Task.FromCanceled<Position>(cancellationTokenSource.Token));

            await Assert.ThrowsAsync<TaskCanceledException>(() => 
                _handler.Handle(query, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task Handle_PositionWithNavigationProperties_ShouldReturnCompletePosition()
        {
            var positionId = Guid.NewGuid();
            var query = new GetPositionByIdQuery { Id = positionId };

            var expectedPosition = new Position
            {
                Id = positionId,
                PositionTitle = "Senior Software Engineer",
                PositionNumber = "SSE001",
                PositionDescription = "Leads software development teams",
                DepartmentId = Guid.NewGuid(),
                SalaryRangeId = Guid.NewGuid(),
                Department = new Department 
                { 
                    Id = Guid.NewGuid(),
                    Name = "Engineering" 
                },
                SalaryRange = new SalaryRange 
                { 
                    Id = Guid.NewGuid(),
                    MinSalary = 80000, 
                    MaxSalary = 120000 
                }
            };

            _mockRepository.Setup(r => r.GetByIdAsync(positionId)).ReturnsAsync(expectedPosition);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result.Data.Department);
            Assert.NotNull(result.Data.SalaryRange);
            Assert.Equal("Engineering", result.Data.Department.Name);
            Assert.Equal(80000, result.Data.SalaryRange.MinSalary);
            Assert.Equal(120000, result.Data.SalaryRange.MaxSalary);
        }

        [Fact]
        public async Task Handle_ValidId_ResponseShouldHaveCorrectStructure()
        {
            var positionId = Guid.NewGuid();
            var query = new GetPositionByIdQuery { Id = positionId };

            var expectedPosition = new Position { Id = positionId };
            _mockRepository.Setup(r => r.GetByIdAsync(positionId)).ReturnsAsync(expectedPosition);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.True(result.Succeeded);
            Assert.Null(result.Message);
            Assert.Null(result.Errors);
            Assert.NotNull(result.Data);
        }
    }
}