using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TalentManagement.Application.Features.Employees.Queries.GetEmployees;
using TalentManagement.Application.Interfaces;
using TalentManagement.Application.Parameters;
using TalentManagement.Domain.Entities;
using TalentManagement.Domain.Enums;
using TalentManagement.Infrastructure.Persistence.Contexts;
using TalentManagement.Infrastructure.Persistence.Repositories;
using Entity = TalentManagement.Domain.Entities.Entity;

namespace TalentManagement.Tests.Repositories
{
    public class EmployeeRepositoryAsyncTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IDataShapeHelper<Employee>> _mockDataShaper;
        private readonly EmployeeRepositoryAsync _repository;
        private readonly List<Employee> _testEmployees;
        private readonly List<Position> _testPositions;
        private readonly List<Department> _testDepartments;

        public EmployeeRepositoryAsyncTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var mockDateTimeService = new Mock<IDateTimeService>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            
            _context = new ApplicationDbContext(options, mockDateTimeService.Object, mockLoggerFactory.Object);
            _mockDataShaper = new Mock<IDataShapeHelper<Employee>>();
            _repository = new EmployeeRepositoryAsync(_context, _mockDataShaper.Object);

            _testDepartments = new List<Department>();
            _testPositions = new List<Position>();
            _testEmployees = new List<Employee>();

            SeedTestData();
        }

        private void SeedTestData()
        {
            var departments = new List<Department>
            {
                new Department { Id = Guid.NewGuid(), Name = "Engineering" },
                new Department { Id = Guid.NewGuid(), Name = "Marketing" },
                new Department { Id = Guid.NewGuid(), Name = "HR" }
            };

            var salaryRanges = new List<SalaryRange>
            {
                new SalaryRange { Id = Guid.NewGuid(), Name = "Junior", MinSalary = 50000, MaxSalary = 70000 },
                new SalaryRange { Id = Guid.NewGuid(), Name = "Senior", MinSalary = 80000, MaxSalary = 120000 }
            };

            var positions = new List<Position>
            {
                new Position 
                { 
                    Id = Guid.NewGuid(), 
                    PositionTitle = "Software Engineer", 
                    PositionNumber = "SE001",
                    PositionDescription = "Develops software applications",
                    DepartmentId = departments[0].Id,
                    SalaryRangeId = salaryRanges[0].Id
                },
                new Position 
                { 
                    Id = Guid.NewGuid(), 
                    PositionTitle = "Marketing Manager", 
                    PositionNumber = "MM001",
                    PositionDescription = "Manages marketing campaigns",
                    DepartmentId = departments[1].Id,
                    SalaryRangeId = salaryRanges[1].Id
                },
                new Position 
                { 
                    Id = Guid.NewGuid(), 
                    PositionTitle = "HR Specialist", 
                    PositionNumber = "HR001",
                    PositionDescription = "Handles human resources tasks",
                    DepartmentId = departments[2].Id,
                    SalaryRangeId = salaryRanges[0].Id
                }
            };

            var employees = new List<Employee>
            {
                new Employee
                {
                    Id = Guid.NewGuid(),
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
                    PositionId = positions[0].Id
                },
                new Employee
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@company.com",
                    EmployeeNumber = "EMP002",
                    Phone = "+1-555-987-6543",
                    Prefix = "Ms.",
                    Salary = 85000m,
                    Birthday = DateTime.Now.AddYears(-28),
                    Gender = Gender.Female,
                    PositionId = positions[1].Id
                },
                new Employee
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Robert",
                    MiddleName = "James",
                    LastName = "Wilson",
                    Email = "robert.wilson@company.com",
                    EmployeeNumber = "EMP003",
                    Phone = "+1-555-456-7890",
                    Prefix = "Dr.",
                    Salary = 95000m,
                    Birthday = DateTime.Now.AddYears(-35),
                    Gender = Gender.Male,
                    PositionId = positions[0].Id
                },
                new Employee
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Alice",
                    LastName = "Johnson",
                    Email = "alice.johnson@company.com",
                    EmployeeNumber = "EMP004",
                    Salary = 65000m,
                    Birthday = DateTime.Now.AddYears(-26),
                    Gender = Gender.Female,
                    PositionId = positions[2].Id
                }
            };

            _context.Departments.AddRange(departments);
            _context.SalaryRanges.AddRange(salaryRanges);
            _context.Positions.AddRange(positions);
            _context.Employees.AddRange(employees);
            _context.SaveChanges();

            _testDepartments.AddRange(departments);
            _testPositions.AddRange(positions);
            _testEmployees.AddRange(employees);
        }

        [Fact]
        public async Task AddAsync_ShouldAddEmployeeToDatabase()
        {
            var employeeId = Guid.NewGuid();
            var newEmployee = new Employee
            {
                Id = employeeId,
                FirstName = "Test",
                LastName = "Employee",
                Email = "test@company.com",
                EmployeeNumber = "EMPTEST",
                Salary = 50000m,
                Birthday = DateTime.Now.AddYears(-25),
                Gender = Gender.Female,
                PositionId = _testPositions[0].Id
            };

            var result = await _repository.AddAsync(newEmployee);

            Assert.NotNull(result);
            Assert.Equal(employeeId, newEmployee.Id);

            var savedEmployee = await _context.Employees.FindAsync(employeeId);
            Assert.NotNull(savedEmployee);
            Assert.Equal("Test", savedEmployee.FirstName);
            Assert.Equal("Employee", savedEmployee.LastName);
            Assert.Equal("test@company.com", savedEmployee.Email);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnEmployee()
        {
            var employee = _testEmployees[0];

            var result = await _repository.GetByIdAsync(employee.Id);

            Assert.NotNull(result);
            Assert.Equal(employee.Id, result.Id);
            Assert.Equal(employee.FirstName, result.FirstName);
            Assert.Equal(employee.LastName, result.LastName);
            Assert.Equal(employee.Email, result.Email);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            var invalidId = Guid.NewGuid();

            var result = await _repository.GetByIdAsync(invalidId);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEmployeeInDatabase()
        {
            var employee = _testEmployees[0];
            employee.FirstName = "Updated";
            employee.LastName = "Name";
            employee.Salary = 90000m;

            await _repository.UpdateAsync(employee);

            var updatedEmployee = await _context.Employees.FindAsync(employee.Id);
            Assert.NotNull(updatedEmployee);
            Assert.Equal("Updated", updatedEmployee.FirstName);
            Assert.Equal("Name", updatedEmployee.LastName);
            Assert.Equal(90000m, updatedEmployee.Salary);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveEmployeeFromDatabase()
        {
            var employee = _testEmployees[0];

            await _repository.DeleteAsync(employee);

            var deletedEmployee = await _context.Employees.FindAsync(employee.Id);
            Assert.Null(deletedEmployee);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEmployees()
        {
            var result = await _repository.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(_testEmployees.Count, result.Count());
        }

        [Fact]
        public async Task IsUniqueEmployeeNumberAsync_WithUniqueNumber_ShouldReturnTrue()
        {
            var uniqueNumber = "UNIQUE001";

            var isUnique = await _repository.IsUniqueEmployeeNumberAsync(uniqueNumber);

            Assert.True(isUnique);
        }

        [Fact]
        public async Task IsUniqueEmployeeNumberAsync_WithExistingNumber_ShouldReturnFalse()
        {
            var existingNumber = _testEmployees[0].EmployeeNumber;

            var isUnique = await _repository.IsUniqueEmployeeNumberAsync(existingNumber);

            Assert.False(isUnique);
        }

        [Fact]
        public async Task GetEmployeeResponseAsync_WithNoFilters_ShouldReturnAllEmployees()
        {
            var query = new GetEmployeesQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Fields = "Id,FirstName,LastName"
            };

            var shapedData = new List<Entity>
            {
                new Entity(),
                new Entity(),
                new Entity(),
                new Entity()
            };

            _mockDataShaper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<Employee>>(), It.IsAny<string>()))
                          .Returns(shapedData);

            var result = await _repository.GetEmployeeResponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(_testEmployees.Count, result.recordsCount.RecordsTotal);
            Assert.Equal(_testEmployees.Count, result.recordsCount.RecordsFiltered);
        }

        [Fact]
        public async Task GetEmployeeResponseAsync_WithFirstNameFilter_ShouldReturnFilteredEmployees()
        {
            var query = new GetEmployeesQuery
            {
                PageNumber = 1,
                PageSize = 10,
                FirstName = "John",
                Fields = "Id,FirstName,LastName"
            };

            var expectedEmployees = _testEmployees.Where(e => e.FirstName.Contains("John")).ToList();

            _mockDataShaper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<Employee>>(), It.IsAny<string>()))
                          .Returns(new List<Entity> { new Entity() });

            var result = await _repository.GetEmployeeResponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(_testEmployees.Count, result.recordsCount.RecordsTotal);
            Assert.Equal(expectedEmployees.Count, result.recordsCount.RecordsFiltered);
        }

        [Fact]
        public async Task GetEmployeeResponseAsync_WithEmailFilter_ShouldReturnFilteredEmployees()
        {
            var query = new GetEmployeesQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Email = "jane.smith",
                Fields = "Id,FirstName,LastName,Email"
            };

            var expectedEmployees = _testEmployees.Where(e => e.Email.Contains("jane.smith")).ToList();

            _mockDataShaper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<Employee>>(), It.IsAny<string>()))
                          .Returns(new List<Entity> { new Entity() });

            var result = await _repository.GetEmployeeResponseAsync(query);

            Assert.Equal(expectedEmployees.Count, result.recordsCount.RecordsFiltered);
        }

        [Fact]
        public async Task GetEmployeeResponseAsync_WithGenderFilter_ShouldReturnFilteredEmployees()
        {
            var query = new GetEmployeesQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Gender = Gender.Male,
                Fields = "Id,FirstName,LastName,Gender"
            };

            var expectedEmployees = _testEmployees.Where(e => e.Gender == Gender.Male).ToList();

            _mockDataShaper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<Employee>>(), It.IsAny<string>()))
                          .Returns(new List<Entity> { new Entity() });

            var result = await _repository.GetEmployeeResponseAsync(query);

            Assert.Equal(expectedEmployees.Count, result.recordsCount.RecordsFiltered);
        }

        [Fact]
        public async Task GetEmployeeResponseAsync_WithSalaryRangeFilter_ShouldReturnFilteredEmployees()
        {
            var query = new GetEmployeesQuery
            {
                PageNumber = 1,
                PageSize = 10,
                MinSalary = 70000,
                MaxSalary = 90000,
                Fields = "Id,FirstName,LastName,Salary"
            };

            var expectedEmployees = _testEmployees.Where(e => e.Salary >= 70000 && e.Salary <= 90000).ToList();

            _mockDataShaper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<Employee>>(), It.IsAny<string>()))
                          .Returns(new List<Entity> { new Entity() });

            var result = await _repository.GetEmployeeResponseAsync(query);

            Assert.Equal(expectedEmployees.Count, result.recordsCount.RecordsFiltered);
        }

        [Fact]
        public async Task GetEmployeeResponseAsync_WithPaging_ShouldReturnCorrectPage()
        {
            var query = new GetEmployeesQuery
            {
                PageNumber = 1,
                PageSize = 2,
                Fields = "Id,FirstName,LastName"
            };

            _mockDataShaper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<Employee>>(), It.IsAny<string>()))
                          .Returns((IEnumerable<Employee> employees, string fields) => 
                              new List<Entity> { new Entity(), new Entity() });

            var result = await _repository.GetEmployeeResponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(2, result.data.Count());
            Assert.Equal(_testEmployees.Count, result.recordsCount.RecordsTotal);
        }

        [Fact]
        public async Task GetEmployeeResponseAsync_WithOrderBy_ShouldReturnOrderedResults()
        {
            var query = new GetEmployeesQuery
            {
                PageNumber = 1,
                PageSize = 10,
                OrderBy = "FirstName",
                Fields = "Id,FirstName,LastName"
            };

            _mockDataShaper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<Employee>>(), It.IsAny<string>()))
                          .Returns(new List<Entity> { new Entity(), new Entity(), new Entity(), new Entity() });

            var result = await _repository.GetEmployeeResponseAsync(query);

            Assert.NotNull(result.data);
            _mockDataShaper.Verify(x => x.ShapeData(It.IsAny<IEnumerable<Employee>>(), "Id,FirstName,LastName"), 
                                 Times.Once);
        }

        [Fact]
        public async Task GetEmployeeResponseAsync_WithBirthdayDateRange_ShouldReturnFilteredEmployees()
        {
            var birthdayFrom = DateTime.Now.AddYears(-32);
            var birthdayTo = DateTime.Now.AddYears(-27);

            var query = new GetEmployeesQuery
            {
                PageNumber = 1,
                PageSize = 10,
                BirthdayFrom = birthdayFrom,
                BirthdayTo = birthdayTo,
                Fields = "Id,FirstName,LastName,Birthday"
            };

            var expectedEmployees = _testEmployees.Where(e => e.Birthday >= birthdayFrom && e.Birthday <= birthdayTo).ToList();

            _mockDataShaper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<Employee>>(), It.IsAny<string>()))
                          .Returns(new List<Entity> { new Entity() });

            var result = await _repository.GetEmployeeResponseAsync(query);

            Assert.Equal(expectedEmployees.Count, result.recordsCount.RecordsFiltered);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}