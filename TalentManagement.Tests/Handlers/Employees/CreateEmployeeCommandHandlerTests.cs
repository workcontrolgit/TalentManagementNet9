using AutoMapper;
using Moq;
using TalentManagement.Application.Features.Employees.Commands.CreateEmployee;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Application.Wrappers;
using TalentManagement.Domain.Entities;
using TalentManagement.Domain.Enums;

namespace TalentManagement.Tests.Handlers.Employees
{
    public class CreateEmployeeCommandHandlerTests
    {
        private readonly Mock<IEmployeeRepositoryAsync> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CreateEmployeeCommandHandler _handler;

        public CreateEmployeeCommandHandlerTests()
        {
            _mockRepository = new Mock<IEmployeeRepositoryAsync>();
            _mockMapper = new Mock<IMapper>();
            _handler = new CreateEmployeeCommandHandler(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnSuccessResponse()
        {
            var command = new CreateEmployeeCommand
            {
                FirstName = "John",
                MiddleName = "Michael",
                LastName = "Doe",
                PositionId = Guid.NewGuid(),
                Salary = 75000.50m,
                Birthday = DateTime.Now.AddYears(-30),
                Email = "john.doe@company.com",
                Gender = Gender.Male,
                EmployeeNumber = "EMP001",
                Prefix = "Mr.",
                Phone = "+1-555-123-4567"
            };

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = command.FirstName,
                MiddleName = command.MiddleName,
                LastName = command.LastName,
                PositionId = command.PositionId,
                Salary = command.Salary,
                Birthday = command.Birthday,
                Email = command.Email,
                Gender = command.Gender,
                EmployeeNumber = command.EmployeeNumber,
                Prefix = command.Prefix,
                Phone = command.Phone
            };

            _mockMapper.Setup(m => m.Map<Employee>(command)).Returns(employee);
            _mockRepository.Setup(r => r.AddAsync(employee)).Returns(Task.FromResult(employee));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(employee.Id, result.Data);
            _mockRepository.Verify(r => r.AddAsync(employee), Times.Once);
            _mockMapper.Verify(m => m.Map<Employee>(command), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNullMiddleName_ShouldReturnSuccessResponse()
        {
            var command = new CreateEmployeeCommand
            {
                FirstName = "Jane",
                MiddleName = null,
                LastName = "Smith",
                PositionId = Guid.NewGuid(),
                Salary = 65000m,
                Birthday = DateTime.Now.AddYears(-25),
                Email = "jane.smith@company.com",
                Gender = Gender.Female,
                EmployeeNumber = "EMP002",
                Prefix = "Ms.",
                Phone = "+1-555-987-6543"
            };

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = command.FirstName,
                MiddleName = command.MiddleName,
                LastName = command.LastName,
                PositionId = command.PositionId,
                Salary = command.Salary,
                Birthday = command.Birthday,
                Email = command.Email,
                Gender = command.Gender,
                EmployeeNumber = command.EmployeeNumber,
                Prefix = command.Prefix,
                Phone = command.Phone
            };

            _mockMapper.Setup(m => m.Map<Employee>(command)).Returns(employee);
            _mockRepository.Setup(r => r.AddAsync(employee)).Returns(Task.FromResult(employee));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(employee.Id, result.Data);
        }

        [Fact]
        public async Task Handle_WithZeroSalary_ShouldReturnSuccessResponse()
        {
            var command = new CreateEmployeeCommand
            {
                FirstName = "Bob",
                LastName = "Johnson",
                PositionId = Guid.NewGuid(),
                Salary = 0m,
                Birthday = DateTime.Now.AddYears(-35),
                Email = "bob.johnson@company.com",
                Gender = Gender.Male,
                EmployeeNumber = "EMP003",
                Phone = "+1-555-456-7890"
            };

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = command.FirstName,
                LastName = command.LastName,
                PositionId = command.PositionId,
                Salary = command.Salary,
                Birthday = command.Birthday,
                Email = command.Email,
                Gender = command.Gender,
                EmployeeNumber = command.EmployeeNumber,
                Phone = command.Phone
            };

            _mockMapper.Setup(m => m.Map<Employee>(command)).Returns(employee);
            _mockRepository.Setup(r => r.AddAsync(employee)).Returns(Task.FromResult(employee));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(employee.Id, result.Data);
        }

        [Fact]
        public async Task Handle_WithDifferentGenderValues_ShouldReturnSuccessResponse()
        {
            var command = new CreateEmployeeCommand
            {
                FirstName = "Alex",
                LastName = "Taylor",
                PositionId = Guid.NewGuid(),
                Salary = 80000m,
                Birthday = DateTime.Now.AddYears(-28),
                Email = "alex.taylor@company.com",
                Gender = Gender.Female,
                EmployeeNumber = "EMP004",
                Phone = "+1-555-321-6540"
            };

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = command.FirstName,
                LastName = command.LastName,
                PositionId = command.PositionId,
                Salary = command.Salary,
                Birthday = command.Birthday,
                Email = command.Email,
                Gender = command.Gender,
                EmployeeNumber = command.EmployeeNumber,
                Phone = command.Phone
            };

            _mockMapper.Setup(m => m.Map<Employee>(command)).Returns(employee);
            _mockRepository.Setup(r => r.AddAsync(employee)).Returns(Task.FromResult(employee));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(employee.Id, result.Data);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            var command = new CreateEmployeeCommand
            {
                FirstName = "Test",
                LastName = "Employee",
                PositionId = Guid.NewGuid(),
                Salary = 50000m,
                Birthday = DateTime.Now.AddYears(-30),
                Email = "test@company.com",
                Gender = Gender.Male,
                EmployeeNumber = "EMPTEST"
            };

            var employee = new Employee();
            _mockMapper.Setup(m => m.Map<Employee>(command)).Returns(employee);
            _mockRepository.Setup(r => r.AddAsync(employee))
                          .ThrowsAsync(new InvalidOperationException("Database error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_MapperReturnsNull_ShouldThrowException()
        {
            var command = new CreateEmployeeCommand
            {
                FirstName = "Test",
                LastName = "Employee",
                PositionId = Guid.NewGuid()
            };

            _mockMapper.Setup(m => m.Map<Employee>(command)).Returns((Employee)null);

            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}