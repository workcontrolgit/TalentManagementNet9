using Moq;
using TalentManagement.Application.Features.Positions.Commands.CreatePosition;
using TalentManagement.Application.Interfaces.Repositories;

namespace TalentManagement.Tests.Handlers.Positions
{
    public class CreatePositionCommandValidatorTests
    {
        private readonly Mock<IPositionRepositoryAsync> _mockRepository;
        private readonly CreatePositionCommandValidator _validator;

        public CreatePositionCommandValidatorTests()
        {
            _mockRepository = new Mock<IPositionRepositoryAsync>();
            _validator = new CreatePositionCommandValidator(_mockRepository.Object);
        }

        [Fact]
        public async Task Validate_ValidCommand_ShouldPassValidation()
        {
            var command = new CreatePositionCommand
            {
                PositionTitle = "Software Engineer",
                PositionNumber = "SE001",
                PositionDescription = "Develops software applications",
                DepartmentId = Guid.NewGuid(),
                SalaryRangeId = Guid.NewGuid()
            };

            _mockRepository.Setup(r => r.IsUniquePositionNumberAsync(command.PositionNumber))
                .ReturnsAsync(true);

            var result = await _validator.ValidateAsync(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task Validate_EmptyPositionNumber_ShouldFailValidation(string positionNumber)
        {
            var command = new CreatePositionCommand
            {
                PositionTitle = "Software Engineer",
                PositionNumber = positionNumber,
                DepartmentId = Guid.NewGuid(),
                SalaryRangeId = Guid.NewGuid()
            };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "PositionNumber");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("required"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task Validate_EmptyPositionTitle_ShouldFailValidation(string positionTitle)
        {
            var command = new CreatePositionCommand
            {
                PositionTitle = positionTitle,
                PositionNumber = "SE001",
                DepartmentId = Guid.NewGuid(),
                SalaryRangeId = Guid.NewGuid()
            };

            _mockRepository.Setup(r => r.IsUniquePositionNumberAsync(command.PositionNumber))
                .ReturnsAsync(true);

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "PositionTitle");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("required"));
        }

        [Fact]
        public async Task Validate_PositionNumberTooLong_ShouldFailValidation()
        {
            var command = new CreatePositionCommand
            {
                PositionTitle = "Software Engineer",
                PositionNumber = new string('A', 51), // 51 characters
                DepartmentId = Guid.NewGuid(),
                SalaryRangeId = Guid.NewGuid()
            };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "PositionNumber");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("must not exceed 50 characters"));
        }

        [Fact]
        public async Task Validate_PositionTitleTooLong_ShouldFailValidation()
        {
            var command = new CreatePositionCommand
            {
                PositionTitle = new string('A', 51), // 51 characters
                PositionNumber = "SE001",
                DepartmentId = Guid.NewGuid(),
                SalaryRangeId = Guid.NewGuid()
            };

            _mockRepository.Setup(r => r.IsUniquePositionNumberAsync(command.PositionNumber))
                .ReturnsAsync(true);

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "PositionTitle");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("must not exceed 50 characters"));
        }

        [Fact]
        public async Task Validate_DuplicatePositionNumber_ShouldFailValidation()
        {
            var command = new CreatePositionCommand
            {
                PositionTitle = "Software Engineer",
                PositionNumber = "SE001",
                DepartmentId = Guid.NewGuid(),
                SalaryRangeId = Guid.NewGuid()
            };

            _mockRepository.Setup(r => r.IsUniquePositionNumberAsync(command.PositionNumber))
                .ReturnsAsync(false);

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "PositionNumber");
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("already exists"));
        }

        [Fact]
        public async Task Validate_PositionNumberMaxLength_ShouldPassValidation()
        {
            var command = new CreatePositionCommand
            {
                PositionTitle = "Software Engineer",
                PositionNumber = new string('A', 50), // Exactly 50 characters
                DepartmentId = Guid.NewGuid(),
                SalaryRangeId = Guid.NewGuid()
            };

            _mockRepository.Setup(r => r.IsUniquePositionNumberAsync(command.PositionNumber))
                .ReturnsAsync(true);

            var result = await _validator.ValidateAsync(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task Validate_PositionTitleMaxLength_ShouldPassValidation()
        {
            var command = new CreatePositionCommand
            {
                PositionTitle = new string('A', 50), // Exactly 50 characters
                PositionNumber = "SE001",
                DepartmentId = Guid.NewGuid(),
                SalaryRangeId = Guid.NewGuid()
            };

            _mockRepository.Setup(r => r.IsUniquePositionNumberAsync(command.PositionNumber))
                .ReturnsAsync(true);

            var result = await _validator.ValidateAsync(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task Validate_RepositoryThrowsException_ShouldPropagateException()
        {
            var command = new CreatePositionCommand
            {
                PositionTitle = "Software Engineer",
                PositionNumber = "SE001",
                DepartmentId = Guid.NewGuid(),
                SalaryRangeId = Guid.NewGuid()
            };

            _mockRepository.Setup(r => r.IsUniquePositionNumberAsync(command.PositionNumber))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _validator.ValidateAsync(command));
        }
    }
}