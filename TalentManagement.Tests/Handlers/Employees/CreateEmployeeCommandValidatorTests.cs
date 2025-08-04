using Moq;
using TalentManagement.Application.Features.Employees.Commands.CreateEmployee;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Domain.Enums;

namespace TalentManagement.Tests.Handlers.Employees
{
    public class CreateEmployeeCommandValidatorTests
    {
        private readonly CreateEmployeeCommandValidator _validator;
        private readonly Mock<IEmployeeRepositoryAsync> _mockRepository;

        public CreateEmployeeCommandValidatorTests()
        {
            _mockRepository = new Mock<IEmployeeRepositoryAsync>();
            _validator = new CreateEmployeeCommandValidator(_mockRepository.Object);
        }

        [Fact]
        public async Task Validate_ValidCommand_ShouldPassValidation()
        {
            var command = new CreateEmployeeCommand
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@company.com",
                EmployeeNumber = "EMP001",
                PositionId = Guid.NewGuid(),
                Salary = 50000m,
                Birthday = DateTime.Now.AddYears(-30),
                Gender = Gender.Male
            };

            _mockRepository.Setup(r => r.IsUniqueEmployeeNumberAsync(command.EmployeeNumber))
                          .ReturnsAsync(true);

            var result = await _validator.ValidateAsync(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task Validate_EmptyFirstName_ShouldHaveValidationError(string firstName)
        {
            var command = new CreateEmployeeCommand { FirstName = firstName };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("required"));
        }

        [Fact]
        public async Task Validate_FirstNameTooLong_ShouldHaveValidationError()
        {
            var command = new CreateEmployeeCommand 
            { 
                FirstName = new string('A', 51) // 51 characters
            };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("must not exceed 50 characters"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task Validate_EmptyLastName_ShouldHaveValidationError(string lastName)
        {
            var command = new CreateEmployeeCommand { LastName = lastName };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "LastName");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("required"));
        }

        [Fact]
        public async Task Validate_LastNameTooLong_ShouldHaveValidationError()
        {
            var command = new CreateEmployeeCommand 
            { 
                LastName = new string('B', 51) // 51 characters
            };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "LastName");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("must not exceed 50 characters"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task Validate_EmptyEmail_ShouldHaveValidationError(string email)
        {
            var command = new CreateEmployeeCommand { Email = email };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Email");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("required"));
        }

        [Fact]
        public async Task Validate_InvalidEmail_ShouldHaveValidationError()
        {
            var command = new CreateEmployeeCommand { Email = "invalid-email" };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Email");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("valid email"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task Validate_EmptyEmployeeNumber_ShouldHaveValidationError(string employeeNumber)
        {
            var command = new CreateEmployeeCommand { EmployeeNumber = employeeNumber };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "EmployeeNumber");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("required"));
        }

        [Fact]
        public async Task Validate_EmployeeNumberTooLong_ShouldHaveValidationError()
        {
            var command = new CreateEmployeeCommand 
            { 
                EmployeeNumber = new string('1', 21) // 21 characters
            };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "EmployeeNumber");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("must not exceed 20 characters"));
        }

        [Fact]
        public async Task Validate_DuplicateEmployeeNumber_ShouldHaveValidationError()
        {
            _mockRepository.Setup(r => r.IsUniqueEmployeeNumberAsync("EMP001"))
                          .ReturnsAsync(false);

            var command = new CreateEmployeeCommand { EmployeeNumber = "EMP001" };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "EmployeeNumber");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("already exists"));
        }

        [Fact]
        public async Task Validate_EmptyPositionId_ShouldHaveValidationError()
        {
            var command = new CreateEmployeeCommand { PositionId = Guid.Empty };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "PositionId");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("required"));
        }

        [Fact]
        public async Task Validate_ZeroSalary_ShouldHaveValidationError()
        {
            var command = new CreateEmployeeCommand { Salary = 0m };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Salary");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("greater than 0"));
        }

        [Fact]
        public async Task Validate_NegativeSalary_ShouldHaveValidationError()
        {
            var command = new CreateEmployeeCommand { Salary = -1000m };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Salary");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("greater than 0"));
        }

        [Fact]
        public async Task Validate_FutureBirthday_ShouldHaveValidationError()
        {
            var command = new CreateEmployeeCommand { Birthday = DateTime.Today.AddDays(1) };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Birthday");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("must be in the past"));
        }

        [Fact]
        public async Task Validate_PhoneTooLong_ShouldHaveValidationError()
        {
            var command = new CreateEmployeeCommand 
            { 
                Phone = new string('1', 21) // 21 characters
            };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Phone");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("must not exceed 20 characters"));
        }

        [Fact]
        public async Task Validate_PrefixTooLong_ShouldHaveValidationError()
        {
            var command = new CreateEmployeeCommand 
            { 
                Prefix = new string('A', 11) // 11 characters
            };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Prefix");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("must not exceed 10 characters"));
        }

        [Fact]
        public async Task Validate_MiddleNameTooLong_ShouldHaveValidationError()
        {
            var command = new CreateEmployeeCommand 
            { 
                MiddleName = new string('M', 51) // 51 characters
            };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "MiddleName");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("must not exceed 50 characters"));
        }

        [Fact]
        public async Task Validate_OptionalFieldsEmpty_ShouldNotHaveValidationErrors()
        {
            var command = new CreateEmployeeCommand
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@company.com",
                EmployeeNumber = "EMP001",
                PositionId = Guid.NewGuid(),
                Salary = 50000m,
                Birthday = DateTime.Now.AddYears(-30),
                Gender = Gender.Male,
                MiddleName = null,
                Phone = null,
                Prefix = null
            };

            _mockRepository.Setup(r => r.IsUniqueEmployeeNumberAsync(command.EmployeeNumber))
                          .ReturnsAsync(true);

            var result = await _validator.ValidateAsync(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
    }
}