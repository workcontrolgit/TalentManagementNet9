using Moq;
using TalentManagement.Application.Exceptions;
using TalentManagement.Application.Features.Departments.Commands.UpdateDepartment;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Domain.Entities;

namespace TalentManagement.Tests.Handlers.Departments
{
    public class UpdateDepartmentCommandHandlerTests
    {
        private readonly Mock<IDepartmentRepositoryAsync> _mockRepository;
        private readonly UpdateDepartmentCommandHandler _handler;

        public UpdateDepartmentCommandHandlerTests()
        {
            _mockRepository = new Mock<IDepartmentRepositoryAsync>();
            _handler = new UpdateDepartmentCommandHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnSuccessResponse()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var command = new UpdateDepartmentCommand { Id = departmentId, Name = "Updated Department" };
            var existingDepartment = new Department { Id = departmentId, Name = "Original Department" };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(existingDepartment);
            _mockRepository.Setup(r => r.UpdateAsync(existingDepartment)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(departmentId, result.Data);
            Assert.Equal("Updated Department", existingDepartment.Name);
            _mockRepository.Verify(r => r.UpdateAsync(existingDepartment), Times.Once);
        }

        [Fact]
        public async Task Handle_DepartmentNotFound_ShouldThrowApiException()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var command = new UpdateDepartmentCommand { Id = departmentId, Name = "Updated Department" };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync((Department)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Department Not Found.", exception.Message);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Department>()), Times.Never);
        }

        [Fact]
        public async Task Handle_EmptyGuidId_ShouldStillCallRepository()
        {
            // Arrange
            var departmentId = Guid.Empty;
            var command = new UpdateDepartmentCommand { Id = departmentId, Name = "Updated Department" };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync((Department)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Department Not Found.", exception.Message);
            _mockRepository.Verify(r => r.GetByIdAsync(departmentId), Times.Once);
        }

        [Fact]
        public async Task Handle_RepositoryGetThrowsException_ShouldPropagateException()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var command = new UpdateDepartmentCommand { Id = departmentId, Name = "Updated Department" };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Database error", exception.Message);
        }

        [Fact]
        public async Task Handle_RepositoryUpdateThrowsException_ShouldPropagateException()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var command = new UpdateDepartmentCommand { Id = departmentId, Name = "Updated Department" };
            var existingDepartment = new Department { Id = departmentId, Name = "Original Department" };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(existingDepartment);
            _mockRepository.Setup(r => r.UpdateAsync(existingDepartment)).ThrowsAsync(new InvalidOperationException("Update failed"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Update failed", exception.Message);
        }

        [Fact]
        public async Task Handle_NullRequest_ShouldThrowNullReferenceException()
        {
            // Arrange
            UpdateDepartmentCommand command = null;

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithEmptyName_ShouldUpdateSuccessfully()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var command = new UpdateDepartmentCommand { Id = departmentId, Name = "" };
            var existingDepartment = new Department { Id = departmentId, Name = "Original Department" };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(existingDepartment);
            _mockRepository.Setup(r => r.UpdateAsync(existingDepartment)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal("", existingDepartment.Name);
        }

        [Fact]
        public async Task Handle_WithNullName_ShouldUpdateSuccessfully()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var command = new UpdateDepartmentCommand { Id = departmentId, Name = null };
            var existingDepartment = new Department { Id = departmentId, Name = "Original Department" };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(existingDepartment);
            _mockRepository.Setup(r => r.UpdateAsync(existingDepartment)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Null(existingDepartment.Name);
        }

        [Fact]
        public async Task Handle_WithLongName_ShouldUpdateSuccessfully()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var longName = new string('A', 250);
            var command = new UpdateDepartmentCommand { Id = departmentId, Name = longName };
            var existingDepartment = new Department { Id = departmentId, Name = "Original Department" };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(existingDepartment);
            _mockRepository.Setup(r => r.UpdateAsync(existingDepartment)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(longName, existingDepartment.Name);
        }
    }
}