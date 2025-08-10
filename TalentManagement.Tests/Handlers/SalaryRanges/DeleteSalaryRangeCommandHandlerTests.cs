using Moq;
using TalentManagement.Application.Exceptions;
using TalentManagement.Application.Features.SalaryRanges.Commands.DeleteSalaryRange;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Domain.Entities;

namespace TalentManagement.Tests.Handlers.SalaryRanges
{
    public class DeleteSalaryRangeCommandHandlerTests
    {
        private readonly Mock<ISalaryRangeRepositoryAsync> _mockRepository;
        private readonly DeleteSalaryRangeCommandHandler _handler;

        public DeleteSalaryRangeCommandHandlerTests()
        {
            _mockRepository = new Mock<ISalaryRangeRepositoryAsync>();
            _handler = new DeleteSalaryRangeCommandHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldDeleteSalaryRangeAndReturnSuccess()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var command = new DeleteSalaryRangeCommand { Id = salaryRangeId };
            var existingSalaryRange = new SalaryRange { Id = salaryRangeId, Name = "Range to Delete", MinSalary = 50000m, MaxSalary = 70000m };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync(existingSalaryRange);
            _mockRepository.Setup(r => r.DeleteAsync(existingSalaryRange)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(salaryRangeId, result.Data);
            _mockRepository.Verify(r => r.DeleteAsync(existingSalaryRange), Times.Once);
        }

        [Fact]
        public async Task Handle_SalaryRangeNotFound_ShouldThrowApiException()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var command = new DeleteSalaryRangeCommand { Id = salaryRangeId };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync((SalaryRange)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("SalaryRange Not Found.", exception.Message);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<SalaryRange>()), Times.Never);
        }

        [Fact]
        public async Task Handle_EmptyGuidId_ShouldStillCallRepository()
        {
            // Arrange
            var salaryRangeId = Guid.Empty;
            var command = new DeleteSalaryRangeCommand { Id = salaryRangeId };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync((SalaryRange)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("SalaryRange Not Found.", exception.Message);
            _mockRepository.Verify(r => r.GetByIdAsync(salaryRangeId), Times.Once);
        }

        [Fact]
        public async Task Handle_RepositoryGetThrowsException_ShouldPropagateException()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var command = new DeleteSalaryRangeCommand { Id = salaryRangeId };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Database error", exception.Message);
        }

        [Fact]
        public async Task Handle_RepositoryDeleteThrowsException_ShouldPropagateException()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var command = new DeleteSalaryRangeCommand { Id = salaryRangeId };
            var existingSalaryRange = new SalaryRange { Id = salaryRangeId, Name = "Range to Delete", MinSalary = 50000m, MaxSalary = 70000m };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync(existingSalaryRange);
            _mockRepository.Setup(r => r.DeleteAsync(existingSalaryRange)).ThrowsAsync(new InvalidOperationException("Delete failed"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Delete failed", exception.Message);
        }

        [Fact]
        public async Task Handle_NullRequest_ShouldThrowNullReferenceException()
        {
            // Arrange
            DeleteSalaryRangeCommand command = null;

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_SalaryRangeWithZeroValues_ShouldDeleteSuccessfully()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var command = new DeleteSalaryRangeCommand { Id = salaryRangeId };
            var existingSalaryRange = new SalaryRange { Id = salaryRangeId, Name = "Intern Range", MinSalary = 0m, MaxSalary = 0m };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync(existingSalaryRange);
            _mockRepository.Setup(r => r.DeleteAsync(existingSalaryRange)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(salaryRangeId, result.Data);
            _mockRepository.Verify(r => r.DeleteAsync(existingSalaryRange), Times.Once);
        }

        [Fact]
        public async Task Handle_SalaryRangeWithHighValues_ShouldDeleteSuccessfully()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var command = new DeleteSalaryRangeCommand { Id = salaryRangeId };
            var existingSalaryRange = new SalaryRange { Id = salaryRangeId, Name = "Executive Range", MinSalary = 200000m, MaxSalary = 500000m };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync(existingSalaryRange);
            _mockRepository.Setup(r => r.DeleteAsync(existingSalaryRange)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(salaryRangeId, result.Data);
            _mockRepository.Verify(r => r.DeleteAsync(existingSalaryRange), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidRequestWithComplexSalaryRange_ShouldDeleteSuccessfully()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var command = new DeleteSalaryRangeCommand { Id = salaryRangeId };
            var existingSalaryRange = new SalaryRange 
            { 
                Id = salaryRangeId, 
                Name = "Complex Range",
                MinSalary = 60000m,
                MaxSalary = 90000m,
                Positions = new List<Position>
                {
                    new Position { Id = Guid.NewGuid(), PositionTitle = "Developer" }
                }
            };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync(existingSalaryRange);
            _mockRepository.Setup(r => r.DeleteAsync(existingSalaryRange)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(salaryRangeId, result.Data);
            _mockRepository.Verify(r => r.DeleteAsync(existingSalaryRange), Times.Once);
        }

        [Fact]
        public async Task Handle_SalaryRangeWithPreciseValues_ShouldDeleteSuccessfully()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var command = new DeleteSalaryRangeCommand { Id = salaryRangeId };
            var existingSalaryRange = new SalaryRange 
            { 
                Id = salaryRangeId, 
                Name = "Precise Range",
                MinSalary = 55000.99m,
                MaxSalary = 85000.01m
            };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync(existingSalaryRange);
            _mockRepository.Setup(r => r.DeleteAsync(existingSalaryRange)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(salaryRangeId, result.Data);
            _mockRepository.Verify(r => r.DeleteAsync(existingSalaryRange), Times.Once);
        }
    }
}