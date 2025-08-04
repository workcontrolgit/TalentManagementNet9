using Moq;
using TalentManagement.Application.Exceptions;
using TalentManagement.Application.Features.Departments.Commands.DeleteDepartment;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Domain.Entities;

namespace TalentManagement.Tests.Handlers.Departments
{
    public class DeleteDepartmentCommandHandlerTests
    {
        private readonly Mock<IDepartmentRepositoryAsync> _mockRepository;
        private readonly DeleteDepartmentCommandHandler _handler;

        public DeleteDepartmentCommandHandlerTests()
        {
            _mockRepository = new Mock<IDepartmentRepositoryAsync>();
            _handler = new DeleteDepartmentCommandHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldDeleteDepartmentAndReturnSuccess()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var command = new DeleteDepartmentCommand { Id = departmentId };
            var existingDepartment = new Department { Id = departmentId, Name = "Department to Delete" };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(existingDepartment);
            _mockRepository.Setup(r => r.DeleteAsync(existingDepartment)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(departmentId, result.Data);
            _mockRepository.Verify(r => r.DeleteAsync(existingDepartment), Times.Once);
        }

        [Fact]
        public async Task Handle_DepartmentNotFound_ShouldThrowApiException()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var command = new DeleteDepartmentCommand { Id = departmentId };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync((Department)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Department Not Found.", exception.Message);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Department>()), Times.Never);
        }

        [Fact]
        public async Task Handle_EmptyGuidId_ShouldStillCallRepository()
        {
            // Arrange
            var departmentId = Guid.Empty;
            var command = new DeleteDepartmentCommand { Id = departmentId };

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
            var command = new DeleteDepartmentCommand { Id = departmentId };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Database error", exception.Message);
        }

        [Fact]
        public async Task Handle_RepositoryDeleteThrowsException_ShouldPropagateException()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var command = new DeleteDepartmentCommand { Id = departmentId };
            var existingDepartment = new Department { Id = departmentId, Name = "Department to Delete" };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(existingDepartment);
            _mockRepository.Setup(r => r.DeleteAsync(existingDepartment)).ThrowsAsync(new InvalidOperationException("Delete failed"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Delete failed", exception.Message);
        }

        [Fact]
        public async Task Handle_NullRequest_ShouldThrowNullReferenceException()
        {
            // Arrange
            DeleteDepartmentCommand command = null;

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(command, CancellationToken.None));
        }


        [Fact]
        public async Task Handle_ValidRequestWithComplexDepartment_ShouldDeleteSuccessfully()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var command = new DeleteDepartmentCommand { Id = departmentId };
            var existingDepartment = new Department 
            { 
                Id = departmentId, 
                Name = "Complex Department",
                Positions = new List<Position>
                {
                    new Position { Id = Guid.NewGuid(), PositionTitle = "Manager" }
                }
            };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(existingDepartment);
            _mockRepository.Setup(r => r.DeleteAsync(existingDepartment)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(departmentId, result.Data);
            _mockRepository.Verify(r => r.DeleteAsync(existingDepartment), Times.Once);
        }
    }
}