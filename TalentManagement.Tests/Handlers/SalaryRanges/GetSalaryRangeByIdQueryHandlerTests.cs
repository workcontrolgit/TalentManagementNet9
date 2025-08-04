using Moq;
using TalentManagement.Application.Exceptions;
using TalentManagement.Application.Features.SalaryRanges.Queries.GetSalaryRangeById;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Domain.Entities;

namespace TalentManagement.Tests.Handlers.SalaryRanges
{
    public class GetSalaryRangeByIdQueryHandlerTests
    {
        private readonly Mock<ISalaryRangeRepositoryAsync> _mockRepository;
        private readonly GetSalaryRangeByIdQuery.GetSalaryRangeByIdQueryHandler _handler;

        public GetSalaryRangeByIdQueryHandlerTests()
        {
            _mockRepository = new Mock<ISalaryRangeRepositoryAsync>();
            _handler = new GetSalaryRangeByIdQuery.GetSalaryRangeByIdQueryHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_ValidId_ShouldReturnSalaryRangeSuccessfully()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var salaryRange = new SalaryRange { Id = salaryRangeId, Name = "Senior Level", MinSalary = 80000m, MaxSalary = 120000m };
            var query = new GetSalaryRangeByIdQuery { Id = salaryRangeId };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync(salaryRange);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(salaryRange, result.Data);
            Assert.Equal(salaryRangeId, result.Data.Id);
            Assert.Equal("Senior Level", result.Data.Name);
            Assert.Equal(80000m, result.Data.MinSalary);
            Assert.Equal(120000m, result.Data.MaxSalary);
        }

        [Fact]
        public async Task Handle_ValidId_ResponseShouldHaveCorrectStructure()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var salaryRange = new SalaryRange { Id = salaryRangeId, Name = "Mid Level", MinSalary = 60000m, MaxSalary = 80000m };
            var query = new GetSalaryRangeByIdQuery { Id = salaryRangeId };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync(salaryRange);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.IsType<SalaryRange>(result.Data);
            Assert.True(result.Succeeded);
            Assert.Null(result.Errors);
            Assert.Null(result.Message);
        }

        [Fact]
        public async Task Handle_SalaryRangeNotFound_ShouldThrowApiException()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var query = new GetSalaryRangeByIdQuery { Id = salaryRangeId };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync((SalaryRange)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("SalaryRange Not Found.", exception.Message);
        }

        [Fact]
        public async Task Handle_EmptyGuid_ShouldStillCallRepository()
        {
            // Arrange
            var salaryRangeId = Guid.Empty;
            var query = new GetSalaryRangeByIdQuery { Id = salaryRangeId };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync((SalaryRange)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("SalaryRange Not Found.", exception.Message);
            _mockRepository.Verify(r => r.GetByIdAsync(salaryRangeId), Times.Once);
        }

        [Fact]
        public async Task Handle_NullQuery_ShouldThrowNullReferenceException()
        {
            // Arrange
            GetSalaryRangeByIdQuery query = null;

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var query = new GetSalaryRangeByIdQuery { Id = salaryRangeId };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("Database error", exception.Message);
        }

        [Fact]
        public async Task Handle_SalaryRangeWithZeroValues_ShouldReturnCorrectData()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var salaryRange = new SalaryRange { Id = salaryRangeId, Name = "Intern", MinSalary = 0m, MaxSalary = 0m };
            var query = new GetSalaryRangeByIdQuery { Id = salaryRangeId };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync(salaryRange);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(0m, result.Data.MinSalary);
            Assert.Equal(0m, result.Data.MaxSalary);
        }

        [Fact]
        public async Task Handle_SalaryRangeWithHighValues_ShouldReturnCorrectData()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var salaryRange = new SalaryRange { Id = salaryRangeId, Name = "Executive", MinSalary = 200000m, MaxSalary = 500000m };
            var query = new GetSalaryRangeByIdQuery { Id = salaryRangeId };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync(salaryRange);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(200000m, result.Data.MinSalary);
            Assert.Equal(500000m, result.Data.MaxSalary);
        }

        [Fact]
        public async Task Handle_SalaryRangeWithNavigationProperties_ShouldReturnCompleteSalaryRange()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var salaryRange = new SalaryRange 
            { 
                Id = salaryRangeId, 
                Name = "Development Range",
                MinSalary = 60000m,
                MaxSalary = 90000m,
                Positions = new List<Position>
                {
                    new Position { Id = Guid.NewGuid(), PositionTitle = "Junior Developer" },
                    new Position { Id = Guid.NewGuid(), PositionTitle = "Senior Developer" }
                }
            };
            var query = new GetSalaryRangeByIdQuery { Id = salaryRangeId };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync(salaryRange);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(salaryRange, result.Data);
            Assert.Equal(2, result.Data.Positions.Count);
        }

        [Fact]
        public async Task Handle_SalaryRangeWithPreciseValues_ShouldReturnExactPrecision()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var salaryRange = new SalaryRange 
            { 
                Id = salaryRangeId, 
                Name = "Precise Range",
                MinSalary = 50000.99m,
                MaxSalary = 75000.01m
            };
            var query = new GetSalaryRangeByIdQuery { Id = salaryRangeId };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync(salaryRange);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(50000.99m, result.Data.MinSalary);
            Assert.Equal(75000.01m, result.Data.MaxSalary);
        }
    }
}