using Moq;
using TalentManagement.Application.Features.Employees.Commands.UpdateEmployee;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Application.Exceptions;
using TalentManagement.Domain.Entities;
using TalentManagement.Domain.Enums;

namespace TalentManagement.Tests.Handlers.Employees
{
    public class UpdateEmployeeCommandHandlerTests
    {
        private readonly Mock<IEmployeeRepositoryAsync> _mockRepository;
        private readonly UpdateEmployeeCommand.UpdateEmployeeCommandHandler _handler;

        public UpdateEmployeeCommandHandlerTests()
        {
            _mockRepository = new Mock<IEmployeeRepositoryAsync>();
            _handler = new UpdateEmployeeCommand.UpdateEmployeeCommandHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnSuccessResponse()
        {
            var employeeId = Guid.NewGuid();
            var existingEmployee = new Employee
            {
                Id = employeeId,
                FirstName = "Old First",
                LastName = "Old Last",
                Email = "old@company.com",
                EmployeeNumber = "EMP001"
            };

            var command = new UpdateEmployeeCommand
            {
                Id = employeeId,
                FirstName = "John",
                MiddleName = "Michael",
                LastName = "Doe",
                PositionId = Guid.NewGuid(),
                Salary = 85000m,
                Birthday = DateTime.Now.AddYears(-32),
                Email = "john.doe@company.com",
                Gender = Gender.Male,
                EmployeeNumber = "EMP001",
                Prefix = "Mr.",
                Phone = "+1-555-123-4567"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync(existingEmployee);
            _mockRepository.Setup(r => r.UpdateAsync(existingEmployee)).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(employeeId, result.Data);

            Assert.Equal(command.FirstName, existingEmployee.FirstName);
            Assert.Equal(command.MiddleName, existingEmployee.MiddleName);
            Assert.Equal(command.LastName, existingEmployee.LastName);
            Assert.Equal(command.PositionId, existingEmployee.PositionId);
            Assert.Equal(command.Salary, existingEmployee.Salary);
            Assert.Equal(command.Birthday, existingEmployee.Birthday);
            Assert.Equal(command.Email, existingEmployee.Email);
            Assert.Equal(command.Gender, existingEmployee.Gender);
            Assert.Equal(command.EmployeeNumber, existingEmployee.EmployeeNumber);
            Assert.Equal(command.Prefix, existingEmployee.Prefix);
            Assert.Equal(command.Phone, existingEmployee.Phone);

            _mockRepository.Verify(r => r.GetByIdAsync(employeeId), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(existingEmployee), Times.Once);
        }

        [Fact]
        public async Task Handle_EmployeeNotFound_ShouldThrowApiException()
        {
            var employeeId = Guid.NewGuid();
            var command = new UpdateEmployeeCommand
            {
                Id = employeeId,
                FirstName = "John",
                LastName = "Doe"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync((Employee)null);

            var exception = await Assert.ThrowsAsync<ApiException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Employee Not Found.", exception.Message);
            _mockRepository.Verify(r => r.GetByIdAsync(employeeId), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Employee>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithNullOptionalFields_ShouldUpdateSuccessfully()
        {
            var employeeId = Guid.NewGuid();
            var existingEmployee = new Employee
            {
                Id = employeeId,
                FirstName = "Jane",
                LastName = "Smith",
                MiddleName = "Old Middle",
                Prefix = "Ms.",
                Phone = "Old Phone"
            };

            var command = new UpdateEmployeeCommand
            {
                Id = employeeId,
                FirstName = "Jane",
                LastName = "Smith",
                MiddleName = null,
                PositionId = Guid.NewGuid(),
                Salary = 75000m,
                Birthday = DateTime.Now.AddYears(-28),
                Email = "jane.smith@company.com",
                Gender = Gender.Female,
                EmployeeNumber = "EMP002",
                Prefix = null,
                Phone = null
            };

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync(existingEmployee);
            _mockRepository.Setup(r => r.UpdateAsync(existingEmployee)).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Null(existingEmployee.MiddleName);
            Assert.Null(existingEmployee.Prefix);
            Assert.Null(existingEmployee.Phone);
        }

        [Fact]
        public async Task Handle_UpdateWithDifferentGender_ShouldUpdateSuccessfully()
        {
            var employeeId = Guid.NewGuid();
            var existingEmployee = new Employee
            {
                Id = employeeId,
                Gender = Gender.Male
            };

            var command = new UpdateEmployeeCommand
            {
                Id = employeeId,
                FirstName = "Alex",
                LastName = "Taylor",
                PositionId = Guid.NewGuid(),
                Salary = 70000m,
                Birthday = DateTime.Now.AddYears(-25),
                Email = "alex@company.com",
                Gender = Gender.Female,
                EmployeeNumber = "EMP003"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync(existingEmployee);
            _mockRepository.Setup(r => r.UpdateAsync(existingEmployee)).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(Gender.Female, existingEmployee.Gender);
        }

        [Fact]
        public async Task Handle_UpdateWithZeroSalary_ShouldUpdateSuccessfully()
        {
            var employeeId = Guid.NewGuid();
            var existingEmployee = new Employee
            {
                Id = employeeId,
                Salary = 50000m
            };

            var command = new UpdateEmployeeCommand
            {
                Id = employeeId,
                FirstName = "Test",
                LastName = "Employee",
                PositionId = Guid.NewGuid(),
                Salary = 0m,
                Birthday = DateTime.Now.AddYears(-30),
                Email = "test@company.com",
                Gender = Gender.Male,
                EmployeeNumber = "EMPTEST"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync(existingEmployee);
            _mockRepository.Setup(r => r.UpdateAsync(existingEmployee)).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(0m, existingEmployee.Salary);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            var employeeId = Guid.NewGuid();
            var existingEmployee = new Employee { Id = employeeId };
            var command = new UpdateEmployeeCommand { Id = employeeId };

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync(existingEmployee);
            _mockRepository.Setup(r => r.UpdateAsync(existingEmployee))
                          .ThrowsAsync(new InvalidOperationException("Database error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}