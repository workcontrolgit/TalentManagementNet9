using AutoMapper;
using Moq;
using TalentManagement.Application.Features.SalaryRanges.Commands.CreateSalaryRange;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Domain.Entities;

namespace TalentManagement.Tests.Handlers.SalaryRanges
{
    public class CreateSalaryRangeCommandHandlerTests
    {
        private readonly Mock<ISalaryRangeRepositoryAsync> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CreateSalaryRangeCommandHandler _handler;

        public CreateSalaryRangeCommandHandlerTests()
        {
            _mockRepository = new Mock<ISalaryRangeRepositoryAsync>();
            _mockMapper = new Mock<IMapper>();
            _handler = new CreateSalaryRangeCommandHandler(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnSuccessResponse()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand { Name = "Junior Level", MinSalary = 40000m, MaxSalary = 60000m };
            var salaryRange = new SalaryRange { Id = Guid.NewGuid(), Name = command.Name, MinSalary = command.MinSalary, MaxSalary = command.MaxSalary };
            
            _mockMapper.Setup(m => m.Map<SalaryRange>(command)).Returns(salaryRange);
            _mockRepository.Setup(r => r.AddAsync(salaryRange)).Returns(Task.FromResult(salaryRange));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(salaryRange.Id, result.Data);
            _mockRepository.Verify(r => r.AddAsync(salaryRange), Times.Once);
        }

        [Fact]
        public async Task Handle_NullRequest_ShouldThrowNullReferenceException()
        {
            // Arrange
            CreateSalaryRangeCommand command = null;

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_MapperReturnsNull_ShouldThrowNullReferenceException()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand { Name = "Test Range", MinSalary = 50000m, MaxSalary = 70000m };
            _mockMapper.Setup(m => m.Map<SalaryRange>(command)).Returns((SalaryRange)null);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand { Name = "Test Range", MinSalary = 50000m, MaxSalary = 70000m };
            var salaryRange = new SalaryRange { Id = Guid.NewGuid(), Name = command.Name, MinSalary = command.MinSalary, MaxSalary = command.MaxSalary };
            
            _mockMapper.Setup(m => m.Map<SalaryRange>(command)).Returns(salaryRange);
            _mockRepository.Setup(r => r.AddAsync(salaryRange)).ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Database error", exception.Message);
        }

        [Fact]
        public async Task Handle_WithZeroSalaries_ShouldReturnSuccessResponse()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand { Name = "Unpaid Intern", MinSalary = 0m, MaxSalary = 0m };
            var salaryRange = new SalaryRange { Id = Guid.NewGuid(), Name = command.Name, MinSalary = command.MinSalary, MaxSalary = command.MaxSalary };
            
            _mockMapper.Setup(m => m.Map<SalaryRange>(command)).Returns(salaryRange);
            _mockRepository.Setup(r => r.AddAsync(salaryRange)).Returns(Task.FromResult(salaryRange));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(salaryRange.Id, result.Data);
        }

        [Fact]
        public async Task Handle_WithHighSalaries_ShouldReturnSuccessResponse()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand { Name = "Executive Level", MinSalary = 200000m, MaxSalary = 500000m };
            var salaryRange = new SalaryRange { Id = Guid.NewGuid(), Name = command.Name, MinSalary = command.MinSalary, MaxSalary = command.MaxSalary };
            
            _mockMapper.Setup(m => m.Map<SalaryRange>(command)).Returns(salaryRange);
            _mockRepository.Setup(r => r.AddAsync(salaryRange)).Returns(Task.FromResult(salaryRange));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(salaryRange.Id, result.Data);
        }

        [Fact]
        public async Task Handle_WithNegativeSalaries_ShouldReturnSuccessResponse()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand { Name = "Test Range", MinSalary = -1000m, MaxSalary = -500m };
            var salaryRange = new SalaryRange { Id = Guid.NewGuid(), Name = command.Name, MinSalary = command.MinSalary, MaxSalary = command.MaxSalary };
            
            _mockMapper.Setup(m => m.Map<SalaryRange>(command)).Returns(salaryRange);
            _mockRepository.Setup(r => r.AddAsync(salaryRange)).Returns(Task.FromResult(salaryRange));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(salaryRange.Id, result.Data);
        }

        [Fact]
        public async Task Handle_WithEmptyName_ShouldReturnSuccessResponse()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand { Name = "", MinSalary = 50000m, MaxSalary = 70000m };
            var salaryRange = new SalaryRange { Id = Guid.NewGuid(), Name = command.Name, MinSalary = command.MinSalary, MaxSalary = command.MaxSalary };
            
            _mockMapper.Setup(m => m.Map<SalaryRange>(command)).Returns(salaryRange);
            _mockRepository.Setup(r => r.AddAsync(salaryRange)).Returns(Task.FromResult(salaryRange));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(salaryRange.Id, result.Data);
        }

        [Fact]
        public async Task Handle_WithLongName_ShouldReturnSuccessResponse()
        {
            // Arrange
            var longName = new string('A', 250);
            var command = new CreateSalaryRangeCommand { Name = longName, MinSalary = 50000m, MaxSalary = 70000m };
            var salaryRange = new SalaryRange { Id = Guid.NewGuid(), Name = command.Name, MinSalary = command.MinSalary, MaxSalary = command.MaxSalary };
            
            _mockMapper.Setup(m => m.Map<SalaryRange>(command)).Returns(salaryRange);
            _mockRepository.Setup(r => r.AddAsync(salaryRange)).Returns(Task.FromResult(salaryRange));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(salaryRange.Id, result.Data);
        }

        [Fact]
        public async Task Handle_WithPreciseSalaries_ShouldReturnSuccessResponse()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand { Name = "Precise Range", MinSalary = 50000.99m, MaxSalary = 70000.01m };
            var salaryRange = new SalaryRange { Id = Guid.NewGuid(), Name = command.Name, MinSalary = command.MinSalary, MaxSalary = command.MaxSalary };
            
            _mockMapper.Setup(m => m.Map<SalaryRange>(command)).Returns(salaryRange);
            _mockRepository.Setup(r => r.AddAsync(salaryRange)).Returns(Task.FromResult(salaryRange));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(salaryRange.Id, result.Data);
        }
    }
}