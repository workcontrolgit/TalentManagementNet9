using Moq;
using TalentManagement.Application.Features.Employees.Commands.DeleteEmployee;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Application.Exceptions;
using TalentManagement.Domain.Entities;

namespace TalentManagement.Tests.Handlers.Employees
{
    public class DeleteEmployeeCommandHandlerTests
    {
        private readonly Mock<IEmployeeRepositoryAsync> _mockRepository;
        private readonly DeleteEmployeeCommand.DeleteEmployeeCommandHandler _handler;

        public DeleteEmployeeCommandHandlerTests()
        {
            _mockRepository = new Mock<IEmployeeRepositoryAsync>();
            _handler = new DeleteEmployeeCommand.DeleteEmployeeCommandHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_ValidEmployeeId_ShouldReturnSuccessResponse()
        {
            var employeeId = Guid.NewGuid();
            var existingEmployee = new Employee
            {
                Id = employeeId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@company.com",
                EmployeeNumber = "EMP001"
            };

            var command = new DeleteEmployeeCommand { Id = employeeId };

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync(existingEmployee);
            _mockRepository.Setup(r => r.DeleteAsync(existingEmployee)).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(employeeId, result.Data);

            _mockRepository.Verify(r => r.GetByIdAsync(employeeId), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync(existingEmployee), Times.Once);
        }

        [Fact]
        public async Task Handle_EmployeeNotFound_ShouldThrowApiException()
        {
            var employeeId = Guid.NewGuid();
            var command = new DeleteEmployeeCommand { Id = employeeId };

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync((Employee)null);

            var exception = await Assert.ThrowsAsync<ApiException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Employee Not Found.", exception.Message);
            _mockRepository.Verify(r => r.GetByIdAsync(employeeId), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Employee>()), Times.Never);
        }

        [Fact]
        public async Task Handle_EmptyGuid_ShouldThrowApiException()
        {
            var command = new DeleteEmployeeCommand { Id = Guid.Empty };

            _mockRepository.Setup(r => r.GetByIdAsync(Guid.Empty)).ReturnsAsync((Employee)null);

            var exception = await Assert.ThrowsAsync<ApiException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Employee Not Found.", exception.Message);
        }

        [Fact]
        public async Task Handle_RepositoryGetThrowsException_ShouldPropagateException()
        {
            var employeeId = Guid.NewGuid();
            var command = new DeleteEmployeeCommand { Id = employeeId };

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId))
                          .ThrowsAsync(new InvalidOperationException("Database connection error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));

            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Employee>()), Times.Never);
        }

        [Fact]
        public async Task Handle_RepositoryDeleteThrowsException_ShouldPropagateException()
        {
            var employeeId = Guid.NewGuid();
            var existingEmployee = new Employee { Id = employeeId };
            var command = new DeleteEmployeeCommand { Id = employeeId };

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync(existingEmployee);
            _mockRepository.Setup(r => r.DeleteAsync(existingEmployee))
                          .ThrowsAsync(new InvalidOperationException("Delete constraint violation"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));

            _mockRepository.Verify(r => r.GetByIdAsync(employeeId), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync(existingEmployee), Times.Once);
        }

        [Fact]
        public async Task Handle_EmployeeWithAllFields_ShouldDeleteSuccessfully()
        {
            var employeeId = Guid.NewGuid();
            var existingEmployee = new Employee
            {
                Id = employeeId,
                FirstName = "Jane",
                MiddleName = "Marie",
                LastName = "Smith",
                Email = "jane.smith@company.com",
                EmployeeNumber = "EMP002",
                Phone = "+1-555-987-6543",
                Prefix = "Ms.",
                Salary = 75000m,
                Birthday = DateTime.Now.AddYears(-28),
                PositionId = Guid.NewGuid()
            };

            var command = new DeleteEmployeeCommand { Id = employeeId };

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync(existingEmployee);
            _mockRepository.Setup(r => r.DeleteAsync(existingEmployee)).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(employeeId, result.Data);
        }
    }
}