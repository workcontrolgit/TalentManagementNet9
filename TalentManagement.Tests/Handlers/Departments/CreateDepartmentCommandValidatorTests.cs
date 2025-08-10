using FluentValidation.TestHelper;
using TalentManagement.Application.Features.Departments.Commands.CreateDepartment;

namespace TalentManagement.Tests.Handlers.Departments
{
    public class CreateDepartmentCommandValidatorTests
    {
        private readonly CreateDepartmentCommandValidator _validator;

        public CreateDepartmentCommandValidatorTests()
        {
            _validator = new CreateDepartmentCommandValidator();
        }

        [Fact]
        public void Validate_ValidCommand_ShouldPassValidation()
        {
            // Arrange
            var command = new CreateDepartmentCommand
            {
                Name = "Human Resources"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_NameMaxLength_ShouldPassValidation()
        {
            // Arrange
            var command = new CreateDepartmentCommand
            {
                Name = new string('A', 100) // Exactly 100 characters
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_NameTooLong_ShouldFailValidation()
        {
            // Arrange
            var command = new CreateDepartmentCommand
            {
                Name = new string('A', 101) // 101 characters - exceeds maximum
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("Name must not exceed 100 characters.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Validate_EmptyName_ShouldFailValidation(string name)
        {
            // Arrange
            var command = new CreateDepartmentCommand
            {
                Name = name
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("Name is required.");
        }

        [Fact]
        public void Validate_ValidMinimalName_ShouldPassValidation()
        {
            // Arrange
            var command = new CreateDepartmentCommand
            {
                Name = "IT"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_NameWithSpecialCharacters_ShouldPassValidation()
        {
            // Arrange
            var command = new CreateDepartmentCommand
            {
                Name = "R&D Department - Software"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_NameWithNumbers_ShouldPassValidation()
        {
            // Arrange
            var command = new CreateDepartmentCommand
            {
                Name = "Department 123"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_NameWithUnicodeCharacters_ShouldPassValidation()
        {
            // Arrange
            var command = new CreateDepartmentCommand
            {
                Name = "部门测试"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}