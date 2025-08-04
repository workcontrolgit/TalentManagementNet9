using Moq;
using TalentManagement.Application.Features.Employees.Queries.GetEmployeeById;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Application.Exceptions;
using TalentManagement.Domain.Entities;
using TalentManagement.Domain.Enums;

namespace TalentManagement.Tests.Handlers.Employees
{
    public class GetEmployeeByIdQueryHandlerTests
    {
        private readonly Mock<IEmployeeRepositoryAsync> _mockRepository;
        private readonly GetEmployeeByIdQuery.GetEmployeeByIdQueryHandler _handler;

        public GetEmployeeByIdQueryHandlerTests()
        {
            _mockRepository = new Mock<IEmployeeRepositoryAsync>();
            _handler = new GetEmployeeByIdQuery.GetEmployeeByIdQueryHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_ValidEmployeeId_ShouldReturnEmployee()
        {
            var employeeId = Guid.NewGuid();
            var employee = new Employee
            {
                Id = employeeId,
                FirstName = "John",
                MiddleName = "Michael",
                LastName = "Doe",
                Email = "john.doe@company.com",
                EmployeeNumber = "EMP001",
                Phone = "+1-555-123-4567",
                Prefix = "Mr.",
                Salary = 75000m,
                Birthday = DateTime.Now.AddYears(-30),
                Gender = Gender.Male,
                PositionId = Guid.NewGuid()
            };

            var query = new GetEmployeeByIdQuery { Id = employeeId };

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync(employee);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(employee, result.Data);
            Assert.Equal(employeeId, result.Data.Id);
            Assert.Equal("John", result.Data.FirstName);
            Assert.Equal("Doe", result.Data.LastName);

            _mockRepository.Verify(r => r.GetByIdAsync(employeeId), Times.Once);
        }

        [Fact]
        public async Task Handle_EmployeeNotFound_ShouldThrowApiException()
        {
            var employeeId = Guid.NewGuid();
            var query = new GetEmployeeByIdQuery { Id = employeeId };

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync((Employee)null);

            var exception = await Assert.ThrowsAsync<ApiException>(() =>
                _handler.Handle(query, CancellationToken.None));

            Assert.Equal("Employee Not Found.", exception.Message);
            _mockRepository.Verify(r => r.GetByIdAsync(employeeId), Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyGuid_ShouldThrowApiException()
        {
            var query = new GetEmployeeByIdQuery { Id = Guid.Empty };

            _mockRepository.Setup(r => r.GetByIdAsync(Guid.Empty)).ReturnsAsync((Employee)null);

            var exception = await Assert.ThrowsAsync<ApiException>(() =>
                _handler.Handle(query, CancellationToken.None));

            Assert.Equal("Employee Not Found.", exception.Message);
        }

        [Fact]
        public async Task Handle_EmployeeWithMinimalData_ShouldReturnEmployee()
        {
            var employeeId = Guid.NewGuid();
            var employee = new Employee
            {
                Id = employeeId,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@company.com",
                EmployeeNumber = "EMP002",
                Salary = 0m,
                Birthday = DateTime.Now.AddYears(-25),
                Gender = Gender.Female,
                PositionId = Guid.NewGuid()
            };

            var query = new GetEmployeeByIdQuery { Id = employeeId };

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync(employee);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(employee, result.Data);
            Assert.Equal("Jane", result.Data.FirstName);
            Assert.Equal("Smith", result.Data.LastName);
            Assert.Equal(0m, result.Data.Salary);
        }

        [Fact]
        public async Task Handle_EmployeeWithAllFields_ShouldReturnCompleteEmployee()
        {
            var employeeId = Guid.NewGuid();
            var positionId = Guid.NewGuid();
            var birthday = DateTime.Now.AddYears(-35);
            var employee = new Employee
            {
                Id = employeeId,
                FirstName = "Robert",
                MiddleName = "James",
                LastName = "Wilson",
                Email = "robert.wilson@company.com",
                EmployeeNumber = "EMP003",
                Phone = "+1-555-987-1234",
                Prefix = "Dr.",
                Salary = 95000.75m,
                Birthday = birthday,
                Gender = Gender.Male,
                PositionId = positionId
            };

            var query = new GetEmployeeByIdQuery { Id = employeeId };

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync(employee);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(employeeId, result.Data.Id);
            Assert.Equal("Robert", result.Data.FirstName);
            Assert.Equal("James", result.Data.MiddleName);
            Assert.Equal("Wilson", result.Data.LastName);
            Assert.Equal("robert.wilson@company.com", result.Data.Email);
            Assert.Equal("EMP003", result.Data.EmployeeNumber);
            Assert.Equal("+1-555-987-1234", result.Data.Phone);
            Assert.Equal("Dr.", result.Data.Prefix);
            Assert.Equal(95000.75m, result.Data.Salary);
            Assert.Equal(birthday, result.Data.Birthday);
            Assert.Equal(Gender.Male, result.Data.Gender);
            Assert.Equal(positionId, result.Data.PositionId);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            var employeeId = Guid.NewGuid();
            var query = new GetEmployeeByIdQuery { Id = employeeId };

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId))
                          .ThrowsAsync(new InvalidOperationException("Database error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_EmployeeWithNullOptionalFields_ShouldReturnEmployee()
        {
            var employeeId = Guid.NewGuid();
            var employee = new Employee
            {
                Id = employeeId,
                FirstName = "Alex",
                MiddleName = null,
                LastName = "Taylor",
                Email = "alex.taylor@company.com",
                EmployeeNumber = "EMP004",
                Phone = null,
                Prefix = null,
                Salary = 68000m,
                Birthday = DateTime.Now.AddYears(-27),
                Gender = Gender.Female,
                PositionId = Guid.NewGuid()
            };

            var query = new GetEmployeeByIdQuery { Id = employeeId };

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync(employee);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(employee, result.Data);
            Assert.Null(result.Data.MiddleName);
            Assert.Null(result.Data.Phone);
            Assert.Null(result.Data.Prefix);
            Assert.Equal(Gender.Female, result.Data.Gender);
        }
    }
}