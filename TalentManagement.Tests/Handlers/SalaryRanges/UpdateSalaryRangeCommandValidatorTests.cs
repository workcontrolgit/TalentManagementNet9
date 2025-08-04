using FluentValidation.TestHelper;
using TalentManagement.Application.Features.SalaryRanges.Commands.UpdateSalaryRange;

namespace TalentManagement.Tests.Handlers.SalaryRanges
{
    public class UpdateSalaryRangeCommandValidatorTests
    {
        private readonly UpdateSalaryRangeCommandValidator _validator;

        public UpdateSalaryRangeCommandValidatorTests()
        {
            _validator = new UpdateSalaryRangeCommandValidator();
        }

        [Fact]
        public void Validate_ValidCommand_ShouldPassValidation()
        {
            // Arrange
            var command = new UpdateSalaryRangeCommand
            {
                Id = Guid.NewGuid(),
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
        public void Validate_EmptyId_ShouldFailValidation()
        {
            // Arrange
            var command = new UpdateSalaryRangeCommand
            {
                Id = Guid.Empty,
                Name = "Senior Level",
                MinSalary = 80000m,
                MaxSalary = 120000m
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
            var command = new UpdateSalaryRangeCommand
            {
                Id = Guid.NewGuid(),
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
        public void Validate_NameMaxLength_ShouldPassValidation()
        {
            // Arrange
            var command = new UpdateSalaryRangeCommand
            {
                Id = Guid.NewGuid(),
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
            var command = new UpdateSalaryRangeCommand
            {
                Id = Guid.NewGuid(),
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

        [Fact]
        public void Validate_ZeroMinSalary_ShouldFailValidation()
        {
            // Arrange
            var command = new UpdateSalaryRangeCommand
            {
                Id = Guid.NewGuid(),
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
            var command = new UpdateSalaryRangeCommand
            {
                Id = Guid.NewGuid(),
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
            var command = new UpdateSalaryRangeCommand
            {
                Id = Guid.NewGuid(),
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
            var command = new UpdateSalaryRangeCommand
            {
                Id = Guid.NewGuid(),
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
            var command = new UpdateSalaryRangeCommand
            {
                Id = Guid.NewGuid(),
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
            var command = new UpdateSalaryRangeCommand
            {
                Id = Guid.NewGuid(),
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
            var command = new UpdateSalaryRangeCommand
            {
                Id = Guid.NewGuid(),
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
            var command = new UpdateSalaryRangeCommand
            {
                Id = Guid.NewGuid(),
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
            var command = new UpdateSalaryRangeCommand
            {
                Id = Guid.NewGuid(),
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
            var command = new UpdateSalaryRangeCommand
            {
                Id = Guid.NewGuid(),
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
        public void Validate_BothIdAndNameEmpty_ShouldFailValidationForBoth()
        {
            // Arrange
            var command = new UpdateSalaryRangeCommand
            {
                Id = Guid.Empty,
                Name = "",
                MinSalary = 50000m,
                MaxSalary = 70000m
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
        public void Validate_MultipleErrors_ShouldReturnAllErrors()
        {
            // Arrange
            var command = new UpdateSalaryRangeCommand
            {
                Id = Guid.Empty, // Invalid Id
                Name = "", // Invalid name
                MinSalary = -1000m, // Invalid MinSalary
                MaxSalary = -500m // Invalid MaxSalary
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id);
            result.ShouldHaveValidationErrorFor(x => x.Name);
            result.ShouldHaveValidationErrorFor(x => x.MinSalary);
            result.ShouldHaveValidationErrorFor(x => x.MaxSalary);
        }

        [Fact]
        public void Validate_ValidGuidWithValidData_ShouldPassValidation()
        {
            // Arrange
            var command = new UpdateSalaryRangeCommand
            {
                Id = new Guid("12345678-1234-1234-1234-123456789012"),
                Name = "Updated Salary Range",
                MinSalary = 60000m,
                MaxSalary = 90000m
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_SpecialCharactersInName_ShouldPassValidation()
        {
            // Arrange
            var command = new UpdateSalaryRangeCommand
            {
                Id = Guid.NewGuid(),
                Name = "Level 2 - Mid (Experienced)",
                MinSalary = 65000m,
                MaxSalary = 85000m
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
            var command = new UpdateSalaryRangeCommand
            {
                Id = Guid.NewGuid(),
                Name = "更新薪资范围",
                MinSalary = 70000m,
                MaxSalary = 95000m
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}