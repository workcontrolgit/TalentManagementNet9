using FluentValidation.TestHelper;
using TalentManagement.Application.Features.Departments.Commands.UpdateDepartment;

namespace TalentManagement.Tests.Handlers.Departments
{
    public class UpdateDepartmentCommandValidatorTests
    {
        private readonly UpdateDepartmentCommandValidator _validator;

        public UpdateDepartmentCommandValidatorTests()
        {
            _validator = new UpdateDepartmentCommandValidator();
        }

        [Fact]
        public void Validate_ValidCommand_ShouldPassValidation()
        {
            // Arrange
            var command = new UpdateDepartmentCommand
            {
                Id = Guid.NewGuid(),
                Name = "Human Resources"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_EmptyId_ShouldFailValidation()
        {
            // Arrange
            var command = new UpdateDepartmentCommand
            {
                Id = Guid.Empty,
                Name = "Human Resources"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id)
                  .WithErrorMessage("Id is required.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Validate_EmptyName_ShouldFailValidation(string name)
        {
            // Arrange
            var command = new UpdateDepartmentCommand
            {
                Id = Guid.NewGuid(),
                Name = name
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("Name is required.");
        }

        [Fact]
        public void Validate_NameMaxLength_ShouldPassValidation()
        {
            // Arrange
            var command = new UpdateDepartmentCommand
            {
                Id = Guid.NewGuid(),
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
            var command = new UpdateDepartmentCommand
            {
                Id = Guid.NewGuid(),
                Name = new string('A', 101) // 101 characters - exceeds maximum
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("Name must not exceed 100 characters.");
        }

        [Fact]
        public void Validate_ValidMinimalName_ShouldPassValidation()
        {
            // Arrange
            var command = new UpdateDepartmentCommand
            {
                Id = Guid.NewGuid(),
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
            var command = new UpdateDepartmentCommand
            {
                Id = Guid.NewGuid(),
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
            var command = new UpdateDepartmentCommand
            {
                Id = Guid.NewGuid(),
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
            var command = new UpdateDepartmentCommand
            {
                Id = Guid.NewGuid(),
                Name = "部门测试"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_BothIdAndNameEmpty_ShouldFailValidationForBoth()
        {
            // Arrange
            var command = new UpdateDepartmentCommand
            {
                Id = Guid.Empty,
                Name = ""
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id)
                  .WithErrorMessage("Id is required.");
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("Name is required.");
        }

        [Fact]
        public void Validate_ValidGuidWithValidName_ShouldPassValidation()
        {
            // Arrange
            var command = new UpdateDepartmentCommand
            {
                Id = new Guid("12345678-1234-1234-1234-123456789012"),
                Name = "Updated Department Name"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}