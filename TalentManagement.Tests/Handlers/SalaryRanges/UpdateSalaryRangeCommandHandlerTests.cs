using Moq;
using TalentManagement.Application.Exceptions;
using TalentManagement.Application.Features.SalaryRanges.Commands.UpdateSalaryRange;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Domain.Entities;

namespace TalentManagement.Tests.Handlers.SalaryRanges
{
    public class UpdateSalaryRangeCommandHandlerTests
    {
        private readonly Mock<ISalaryRangeRepositoryAsync> _mockRepository;
        private readonly UpdateSalaryRangeCommandHandler _handler;

        public UpdateSalaryRangeCommandHandlerTests()
        {
            _mockRepository = new Mock<ISalaryRangeRepositoryAsync>();
            _handler = new UpdateSalaryRangeCommandHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnSuccessResponse()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var command = new UpdateSalaryRangeCommand { Id = salaryRangeId, Name = "Updated Range", MinSalary = 60000m, MaxSalary = 90000m };
            var existingSalaryRange = new SalaryRange { Id = salaryRangeId, Name = "Original Range", MinSalary = 50000m, MaxSalary = 80000m };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync(existingSalaryRange);
            _mockRepository.Setup(r => r.UpdateAsync(existingSalaryRange)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(salaryRangeId, result.Data);
            Assert.Equal("Updated Range", existingSalaryRange.Name);
            Assert.Equal(60000m, existingSalaryRange.MinSalary);
            Assert.Equal(90000m, existingSalaryRange.MaxSalary);
            _mockRepository.Verify(r => r.UpdateAsync(existingSalaryRange), Times.Once);
        }

        [Fact]
        public async Task Handle_SalaryRangeNotFound_ShouldThrowApiException()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var command = new UpdateSalaryRangeCommand { Id = salaryRangeId, Name = "Updated Range", MinSalary = 60000m, MaxSalary = 90000m };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync((SalaryRange)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("SalaryRange Not Found.", exception.Message);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<SalaryRange>()), Times.Never);
        }

        [Fact]
        public async Task Handle_EmptyGuidId_ShouldStillCallRepository()
        {
            // Arrange
            var salaryRangeId = Guid.Empty;
            var command = new UpdateSalaryRangeCommand { Id = salaryRangeId, Name = "Updated Range", MinSalary = 60000m, MaxSalary = 90000m };

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
            var command = new UpdateSalaryRangeCommand { Id = salaryRangeId, Name = "Updated Range", MinSalary = 60000m, MaxSalary = 90000m };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Database error", exception.Message);
        }

        [Fact]
        public async Task Handle_RepositoryUpdateThrowsException_ShouldPropagateException()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var command = new UpdateSalaryRangeCommand { Id = salaryRangeId, Name = "Updated Range", MinSalary = 60000m, MaxSalary = 90000m };
            var existingSalaryRange = new SalaryRange { Id = salaryRangeId, Name = "Original Range", MinSalary = 50000m, MaxSalary = 80000m };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync(existingSalaryRange);
            _mockRepository.Setup(r => r.UpdateAsync(existingSalaryRange)).ThrowsAsync(new InvalidOperationException("Update failed"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Update failed", exception.Message);
        }

        [Fact]
        public async Task Handle_NullRequest_ShouldThrowNullReferenceException()
        {
            // Arrange
            UpdateSalaryRangeCommand command = null;

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithZeroSalaries_ShouldUpdateSuccessfully()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var command = new UpdateSalaryRangeCommand { Id = salaryRangeId, Name = "Intern Range", MinSalary = 0m, MaxSalary = 0m };
            var existingSalaryRange = new SalaryRange { Id = salaryRangeId, Name = "Original Range", MinSalary = 50000m, MaxSalary = 80000m };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync(existingSalaryRange);
            _mockRepository.Setup(r => r.UpdateAsync(existingSalaryRange)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(0m, existingSalaryRange.MinSalary);
            Assert.Equal(0m, existingSalaryRange.MaxSalary);
        }

        [Fact]
        public async Task Handle_WithNullName_ShouldUpdateSuccessfully()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var command = new UpdateSalaryRangeCommand { Id = salaryRangeId, Name = null, MinSalary = 60000m, MaxSalary = 90000m };
            var existingSalaryRange = new SalaryRange { Id = salaryRangeId, Name = "Original Range", MinSalary = 50000m, MaxSalary = 80000m };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync(existingSalaryRange);
            _mockRepository.Setup(r => r.UpdateAsync(existingSalaryRange)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Null(existingSalaryRange.Name);
        }

        [Fact]
        public async Task Handle_WithHighSalaries_ShouldUpdateSuccessfully()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var command = new UpdateSalaryRangeCommand { Id = salaryRangeId, Name = "Executive Range", MinSalary = 200000m, MaxSalary = 500000m };
            var existingSalaryRange = new SalaryRange { Id = salaryRangeId, Name = "Original Range", MinSalary = 50000m, MaxSalary = 80000m };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync(existingSalaryRange);
            _mockRepository.Setup(r => r.UpdateAsync(existingSalaryRange)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(200000m, existingSalaryRange.MinSalary);
            Assert.Equal(500000m, existingSalaryRange.MaxSalary);
        }

        [Fact]
        public async Task Handle_WithNegativeSalaries_ShouldUpdateSuccessfully()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var command = new UpdateSalaryRangeCommand { Id = salaryRangeId, Name = "Test Range", MinSalary = -1000m, MaxSalary = -500m };
            var existingSalaryRange = new SalaryRange { Id = salaryRangeId, Name = "Original Range", MinSalary = 50000m, MaxSalary = 80000m };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync(existingSalaryRange);
            _mockRepository.Setup(r => r.UpdateAsync(existingSalaryRange)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(-1000m, existingSalaryRange.MinSalary);
            Assert.Equal(-500m, existingSalaryRange.MaxSalary);
        }

        [Fact]
        public async Task Handle_WithPreciseSalaries_ShouldUpdateSuccessfully()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var command = new UpdateSalaryRangeCommand { Id = salaryRangeId, Name = "Precise Range", MinSalary = 55000.99m, MaxSalary = 85000.01m };
            var existingSalaryRange = new SalaryRange { Id = salaryRangeId, Name = "Original Range", MinSalary = 50000m, MaxSalary = 80000m };

            _mockRepository.Setup(r => r.GetByIdAsync(salaryRangeId)).ReturnsAsync(existingSalaryRange);
            _mockRepository.Setup(r => r.UpdateAsync(existingSalaryRange)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(55000.99m, existingSalaryRange.MinSalary);
            Assert.Equal(85000.01m, existingSalaryRange.MaxSalary);
        }
    }
}