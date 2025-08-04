using AutoMapper;
using Moq;
using TalentManagement.Application.Features.Departments.Commands.CreateDepartment;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Domain.Entities;

namespace TalentManagement.Tests.Handlers.Departments
{
    public class CreateDepartmentCommandHandlerTests
    {
        private readonly Mock<IDepartmentRepositoryAsync> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CreateDepartmentCommandHandler _handler;

        public CreateDepartmentCommandHandlerTests()
        {
            _mockRepository = new Mock<IDepartmentRepositoryAsync>();
            _mockMapper = new Mock<IMapper>();
            _handler = new CreateDepartmentCommandHandler(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnSuccessResponse()
        {
            // Arrange
            var command = new CreateDepartmentCommand { Name = "Human Resources" };
            var department = new Department { Id = Guid.NewGuid(), Name = command.Name };
            
            _mockMapper.Setup(m => m.Map<Department>(command)).Returns(department);
            _mockRepository.Setup(r => r.AddAsync(department)).Returns(Task.FromResult(department));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(department.Id, result.Data);
            _mockRepository.Verify(r => r.AddAsync(department), Times.Once);
        }

        [Fact]
        public async Task Handle_NullRequest_ShouldThrowNullReferenceException()
        {
            // Arrange
            CreateDepartmentCommand command = null;

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_MapperReturnsNull_ShouldThrowNullReferenceException()
        {
            // Arrange
            var command = new CreateDepartmentCommand { Name = "Test Department" };
            _mockMapper.Setup(m => m.Map<Department>(command)).Returns((Department)null);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var command = new CreateDepartmentCommand { Name = "Test Department" };
            var department = new Department { Id = Guid.NewGuid(), Name = command.Name };
            
            _mockMapper.Setup(m => m.Map<Department>(command)).Returns(department);
            _mockRepository.Setup(r => r.AddAsync(department)).ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Database error", exception.Message);
        }


        [Fact]
        public async Task Handle_WithEmptyName_ShouldReturnSuccessResponse()
        {
            // Arrange
            var command = new CreateDepartmentCommand { Name = "" };
            var department = new Department { Id = Guid.NewGuid(), Name = command.Name };
            
            _mockMapper.Setup(m => m.Map<Department>(command)).Returns(department);
            _mockRepository.Setup(r => r.AddAsync(department)).Returns(Task.FromResult(department));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(department.Id, result.Data);
        }

        [Fact]
        public async Task Handle_WithLongName_ShouldReturnSuccessResponse()
        {
            // Arrange
            var longName = new string('A', 250);
            var command = new CreateDepartmentCommand { Name = longName };
            var department = new Department { Id = Guid.NewGuid(), Name = command.Name };
            
            _mockMapper.Setup(m => m.Map<Department>(command)).Returns(department);
            _mockRepository.Setup(r => r.AddAsync(department)).Returns(Task.FromResult(department));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(department.Id, result.Data);
        }
    }
}