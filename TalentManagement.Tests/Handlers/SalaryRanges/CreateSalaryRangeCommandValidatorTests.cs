using FluentValidation.TestHelper;
using TalentManagement.Application.Features.SalaryRanges.Commands.CreateSalaryRange;

namespace TalentManagement.Tests.Handlers.SalaryRanges
{
    public class CreateSalaryRangeCommandValidatorTests
    {
        private readonly CreateSalaryRangeCommandValidator _validator;

        public CreateSalaryRangeCommandValidatorTests()
        {
            _validator = new CreateSalaryRangeCommandValidator();
        }

        [Fact]
        public void Validate_ValidCommand_ShouldPassValidation()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand
            {
                Name = "Senior Level",
                MinSalary = 80000m,
                MaxSalary = 120000m
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
            var command = new CreateSalaryRangeCommand
            {
                Name = new string('A', 100), // Exactly 100 characters
                MinSalary = 50000m,
                MaxSalary = 70000m
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
            var command = new CreateSalaryRangeCommand
            {
                Name = new string('A', 101), // 101 characters - exceeds maximum
                MinSalary = 50000m,
                MaxSalary = 70000m
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
            var command = new CreateSalaryRangeCommand
            {
                Name = name,
                MinSalary = 50000m,
                MaxSalary = 70000m
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("Name is required.");
        }

        [Fact]
        public void Validate_ZeroMinSalary_ShouldFailValidation()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand
            {
                Name = "Test Range",
                MinSalary = 0,
                MaxSalary = 70000m
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.MinSalary)
                  .WithErrorMessage("Min Salary is required.");
        }

        [Theory]
        [InlineData(-1000)]
        [InlineData(-0.01)]
        public void Validate_NegativeMinSalary_ShouldFailValidation(decimal minSalary)
        {
            // Arrange
            var command = new CreateSalaryRangeCommand
            {
                Name = "Test Range",
                MinSalary = minSalary,
                MaxSalary = 70000m
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.MinSalary)
                  .WithErrorMessage("Min Salary must be greater than 0.");
        }

        [Fact]
        public void Validate_ZeroMaxSalary_ShouldFailValidation()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand
            {
                Name = "Test Range",
                MinSalary = 50000m,
                MaxSalary = 0
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.MaxSalary)
                  .WithErrorMessage("Max Salary is required.");
        }

        [Theory]
        [InlineData(-1000)]
        [InlineData(-0.01)]
        public void Validate_NegativeMaxSalary_ShouldFailValidation(decimal maxSalary)
        {
            // Arrange
            var command = new CreateSalaryRangeCommand
            {
                Name = "Test Range",
                MinSalary = 50000m,
                MaxSalary = maxSalary
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.MaxSalary)
                  .WithErrorMessage("Max Salary must be greater than 0.");
        }

        [Fact]
        public void Validate_MaxSalaryLessThanMinSalary_ShouldFailValidation()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand
            {
                Name = "Test Range",
                MinSalary = 80000m,
                MaxSalary = 60000m // Less than MinSalary
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.MaxSalary)
                  .WithErrorMessage("Max Salary must be greater than MinSalary.");
        }

        [Fact]
        public void Validate_MaxSalaryEqualToMinSalary_ShouldFailValidation()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand
            {
                Name = "Test Range",
                MinSalary = 70000m,
                MaxSalary = 70000m // Equal to MinSalary
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.MaxSalary)
                  .WithErrorMessage("Max Salary must be greater than MinSalary.");
        }

        [Fact]
        public void Validate_ValidSalaryRange_ShouldPassValidation()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand
            {
                Name = "Executive Level",
                MinSalary = 150000m,
                MaxSalary = 300000m
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_SmallSalaryDifference_ShouldPassValidation()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand
            {
                Name = "Narrow Range",
                MinSalary = 50000.00m,
                MaxSalary = 50000.01m // Very small difference
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_HighSalaryValues_ShouldPassValidation()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand
            {
                Name = "CEO Level",
                MinSalary = 500000m,
                MaxSalary = 1000000m
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_AllFieldsValidMinimum_ShouldPassValidation()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand
            {
                Name = "A", // Minimum valid name
                MinSalary = 0.01m, // Minimum valid MinSalary
                MaxSalary = 0.02m // Minimum valid MaxSalary (greater than MinSalary)
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_MultipleErrors_ShouldReturnAllErrors()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand
            {
                Name = "", // Invalid name
                MinSalary = -1000m, // Invalid MinSalary
                MaxSalary = -500m // Invalid MaxSalary
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
            result.ShouldHaveValidationErrorFor(x => x.MinSalary);
            result.ShouldHaveValidationErrorFor(x => x.MaxSalary);
        }

        [Fact]
        public void Validate_SpecialCharactersInName_ShouldPassValidation()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand
            {
                Name = "Level 1 - Entry (New Grad)",
                MinSalary = 40000m,
                MaxSalary = 55000m
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_UnicodeCharactersInName_ShouldPassValidation()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand
            {
                Name = "薪资范围",
                MinSalary = 60000m,
                MaxSalary = 80000m
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}