using Moq;
using TalentManagement.Application.Exceptions;
using TalentManagement.Application.Features.Departments.Queries.GetDepartmentById;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Domain.Entities;

namespace TalentManagement.Tests.Handlers.Departments
{
    public class GetDepartmentByIdQueryHandlerTests
    {
        private readonly Mock<IDepartmentRepositoryAsync> _mockRepository;
        private readonly GetDepartmentByIdQuery.GetDepartmentByIdQueryHandler _handler;

        public GetDepartmentByIdQueryHandlerTests()
        {
            _mockRepository = new Mock<IDepartmentRepositoryAsync>();
            _handler = new GetDepartmentByIdQuery.GetDepartmentByIdQueryHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_ValidId_ShouldReturnDepartmentSuccessfully()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var department = new Department { Id = departmentId, Name = "Engineering" };
            var query = new GetDepartmentByIdQuery { Id = departmentId };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(department);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(department, result.Data);
            Assert.Equal(departmentId, result.Data.Id);
            Assert.Equal("Engineering", result.Data.Name);
        }

        [Fact]
        public async Task Handle_ValidId_ResponseShouldHaveCorrectStructure()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var department = new Department { Id = departmentId, Name = "Marketing" };
            var query = new GetDepartmentByIdQuery { Id = departmentId };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(department);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.IsType<Department>(result.Data);
            Assert.True(result.Succeeded);
            Assert.Null(result.Errors);
            Assert.Null(result.Message);
        }

        [Fact]
        public async Task Handle_DepartmentNotFound_ShouldThrowApiException()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var query = new GetDepartmentByIdQuery { Id = departmentId };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync((Department)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("Department Not Found.", exception.Message);
        }

        [Fact]
        public async Task Handle_EmptyGuid_ShouldStillCallRepository()
        {
            // Arrange
            var departmentId = Guid.Empty;
            var query = new GetDepartmentByIdQuery { Id = departmentId };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync((Department)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("Department Not Found.", exception.Message);
            _mockRepository.Verify(r => r.GetByIdAsync(departmentId), Times.Once);
        }

        [Fact]
        public async Task Handle_NullQuery_ShouldThrowNullReferenceException()
        {
            // Arrange
            GetDepartmentByIdQuery query = null;

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var query = new GetDepartmentByIdQuery { Id = departmentId };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("Database error", exception.Message);
        }


        [Fact]
        public async Task Handle_DepartmentWithNavigationProperties_ShouldReturnCompleteDepartment()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var department = new Department 
            { 
                Id = departmentId, 
                Name = "IT Department",
                Positions = new List<Position>
                {
                    new Position { Id = Guid.NewGuid(), PositionTitle = "Developer" },
                    new Position { Id = Guid.NewGuid(), PositionTitle = "Manager" }
                }
            };
            var query = new GetDepartmentByIdQuery { Id = departmentId };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(department);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(department, result.Data);
            Assert.Equal(2, result.Data.Positions.Count);
        }
    }
}