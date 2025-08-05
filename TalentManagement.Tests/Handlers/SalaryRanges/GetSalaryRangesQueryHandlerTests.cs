using Moq;
using TalentManagement.Application.Features.SalaryRanges.Queries.GetSalaryRanges;
using TalentManagement.Application.Interfaces;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Application.Parameters;
using TalentManagement.Domain.Entities;
using Entity = TalentManagement.Domain.Entities.Entity;

namespace TalentManagement.Tests.Handlers.SalaryRanges
{
    public class GetSalaryRangesQueryHandlerTests
    {
        private readonly Mock<ISalaryRangeRepositoryAsync> _mockRepository;
        private readonly Mock<IModelHelper> _mockModelHelper;
        private readonly GetAllSalaryRangesQueryHandler _handler;

        public GetSalaryRangesQueryHandlerTests()
        {
            _mockRepository = new Mock<ISalaryRangeRepositoryAsync>();
            _mockModelHelper = new Mock<IModelHelper>();
            _handler = new GetAllSalaryRangesQueryHandler(_mockRepository.Object, _mockModelHelper.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnPagedResponse()
        {
            // Arrange
            var query = new GetSalaryRangesQuery { PageNumber = 1, PageSize = 10, Name = "Senior" };
            var entities = new List<Entity> { new Entity() };
            var recordsCount = new RecordsCount { RecordsTotal = 10, RecordsFiltered = 5 };
            var repositoryResponse = (entities, recordsCount);

            _mockModelHelper.Setup(m => m.ValidateModelFields<GetSalaryRangesViewModel>(It.IsAny<string>()))
                           .Returns("Id,Name,MinSalary,MaxSalary");
            _mockModelHelper.Setup(m => m.GetModelFields<GetSalaryRangesViewModel>())
                           .Returns("Id,Name,MinSalary,MaxSalary");
            _mockRepository.Setup(r => r.GetSalaryRangeReponseAsync(It.IsAny<GetSalaryRangesQuery>()))
                          .ReturnsAsync(repositoryResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(entities, result.Data);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(10, result.RecordsTotal);
        }

        [Fact]
        public async Task Handle_WithValidFields_ShouldValidateFields()
        {
            // Arrange
            var query = new GetSalaryRangesQuery { PageNumber = 1, PageSize = 10, Fields = "Id,Name,InvalidField" };
            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();
            var repositoryResponse = (entities, recordsCount);

            _mockModelHelper.Setup(m => m.ValidateModelFields<GetSalaryRangesViewModel>("Id,Name,InvalidField"))
                           .Returns("Id,Name");
            _mockRepository.Setup(r => r.GetSalaryRangeReponseAsync(It.IsAny<GetSalaryRangesQuery>()))
                          .ReturnsAsync(repositoryResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            _mockModelHelper.Verify(m => m.ValidateModelFields<GetSalaryRangesViewModel>("Id,Name,InvalidField"), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNullFields_ShouldUseDefaultFields()
        {
            // Arrange
            var query = new GetSalaryRangesQuery { PageNumber = 1, PageSize = 10, Fields = null };
            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();
            var repositoryResponse = (entities, recordsCount);

            _mockModelHelper.Setup(m => m.GetModelFields<GetSalaryRangesViewModel>())
                           .Returns("Id,Name,MinSalary,MaxSalary");
            _mockRepository.Setup(r => r.GetSalaryRangeReponseAsync(It.IsAny<GetSalaryRangesQuery>()))
                          .ReturnsAsync(repositoryResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            _mockModelHelper.Verify(m => m.GetModelFields<GetSalaryRangesViewModel>(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithEmptyFields_ShouldUseDefaultFields()
        {
            // Arrange
            var query = new GetSalaryRangesQuery { PageNumber = 1, PageSize = 10, Fields = "" };
            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();
            var repositoryResponse = (entities, recordsCount);

            _mockModelHelper.Setup(m => m.GetModelFields<GetSalaryRangesViewModel>())
                           .Returns("Id,Name,MinSalary,MaxSalary");
            _mockRepository.Setup(r => r.GetSalaryRangeReponseAsync(It.IsAny<GetSalaryRangesQuery>()))
                          .ReturnsAsync(repositoryResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            _mockModelHelper.Verify(m => m.GetModelFields<GetSalaryRangesViewModel>(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidFields_ShouldUseDefaultFields()
        {
            // Arrange
            var query = new GetSalaryRangesQuery { PageNumber = 1, PageSize = 10, Fields = "InvalidField" };
            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();
            var repositoryResponse = (entities, recordsCount);

            _mockModelHelper.Setup(m => m.ValidateModelFields<GetSalaryRangesViewModel>("InvalidField"))
                           .Returns("");
            _mockModelHelper.Setup(m => m.GetModelFields<GetSalaryRangesViewModel>())
                           .Returns("Id,Name,MinSalary,MaxSalary");
            _mockRepository.Setup(r => r.GetSalaryRangeReponseAsync(It.IsAny<GetSalaryRangesQuery>()))
                          .ReturnsAsync(repositoryResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            _mockModelHelper.Verify(m => m.GetModelFields<GetSalaryRangesViewModel>(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithValidOrderBy_ShouldValidateOrderBy()
        {
            // Arrange
            var query = new GetSalaryRangesQuery { PageNumber = 1, PageSize = 10, OrderBy = "Name desc" };
            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();
            var repositoryResponse = (entities, recordsCount);

            _mockModelHelper.Setup(m => m.ValidateModelFields<GetSalaryRangesViewModel>("Name desc"))
                           .Returns("Name desc");
            _mockModelHelper.Setup(m => m.GetModelFields<GetSalaryRangesViewModel>())
                           .Returns("Id,Name,MinSalary,MaxSalary");
            _mockRepository.Setup(r => r.GetSalaryRangeReponseAsync(It.IsAny<GetSalaryRangesQuery>()))
                          .ReturnsAsync(repositoryResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            _mockModelHelper.Verify(m => m.ValidateModelFields<GetSalaryRangesViewModel>("Name desc"), Times.Once);
        }

        [Fact]
        public async Task Handle_WithFilters_ShouldPassFiltersToRepository()
        {
            // Arrange
            var query = new GetSalaryRangesQuery 
            { 
                PageNumber = 1, 
                PageSize = 10, 
                Name = "Executive"
            };
            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();
            var repositoryResponse = (entities, recordsCount);

            _mockModelHelper.Setup(m => m.GetModelFields<GetSalaryRangesViewModel>())
                           .Returns("Id,Name,MinSalary,MaxSalary");
            _mockRepository.Setup(r => r.GetSalaryRangeReponseAsync(It.IsAny<GetSalaryRangesQuery>()))
                          .ReturnsAsync(repositoryResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.GetSalaryRangeReponseAsync(It.Is<GetSalaryRangesQuery>(q => 
                q.Name == "Executive")), Times.Once);
        }

        [Fact]
        public async Task Handle_WithSalaryFieldsInOrderBy_ShouldValidateOrderBy()
        {
            // Arrange
            var query = new GetSalaryRangesQuery { PageNumber = 1, PageSize = 10, OrderBy = "MinSalary asc" };
            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();
            var repositoryResponse = (entities, recordsCount);

            _mockModelHelper.Setup(m => m.ValidateModelFields<GetSalaryRangesViewModel>("MinSalary asc"))
                           .Returns("MinSalary asc");
            _mockModelHelper.Setup(m => m.GetModelFields<GetSalaryRangesViewModel>())
                           .Returns("Id,Name,MinSalary,MaxSalary");
            _mockRepository.Setup(r => r.GetSalaryRangeReponseAsync(It.IsAny<GetSalaryRangesQuery>()))
                          .ReturnsAsync(repositoryResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            _mockModelHelper.Verify(m => m.ValidateModelFields<GetSalaryRangesViewModel>("MinSalary asc"), Times.Once);
        }

        [Fact]
        public async Task Handle_WithSalaryFieldsInFields_ShouldValidateFields()
        {
            // Arrange
            var query = new GetSalaryRangesQuery { PageNumber = 1, PageSize = 10, Fields = "Id,MinSalary,MaxSalary" };
            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();
            var repositoryResponse = (entities, recordsCount);

            _mockModelHelper.Setup(m => m.ValidateModelFields<GetSalaryRangesViewModel>("Id,MinSalary,MaxSalary"))
                           .Returns("Id,MinSalary,MaxSalary");
            _mockRepository.Setup(r => r.GetSalaryRangeReponseAsync(It.IsAny<GetSalaryRangesQuery>()))
                          .ReturnsAsync(repositoryResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            _mockModelHelper.Verify(m => m.ValidateModelFields<GetSalaryRangesViewModel>("Id,MinSalary,MaxSalary"), Times.Once);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var query = new GetSalaryRangesQuery { PageNumber = 1, PageSize = 10 };

            _mockModelHelper.Setup(m => m.GetModelFields<GetSalaryRangesViewModel>())
                           .Returns("Id,Name,MinSalary,MaxSalary");
            _mockRepository.Setup(r => r.GetSalaryRangeReponseAsync(It.IsAny<GetSalaryRangesQuery>()))
                          .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("Database error", exception.Message);
        }

        [Fact]
        public async Task Handle_ModelHelperThrowsException_ShouldPropagateException()
        {
            // Arrange
            var query = new GetSalaryRangesQuery { PageNumber = 1, PageSize = 10 };

            _mockModelHelper.Setup(m => m.GetModelFields<GetSalaryRangesViewModel>())
                           .Throws(new InvalidOperationException("Model helper error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("Model helper error", exception.Message);
        }

        [Fact]
        public async Task Handle_NullQuery_ShouldThrowNullReferenceException()
        {
            // Arrange
            GetSalaryRangesQuery query = null;

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_EmptyNameFilter_ShouldPassToRepository()
        {
            // Arrange
            var query = new GetSalaryRangesQuery { PageNumber = 1, PageSize = 10, Name = "" };
            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();
            var repositoryResponse = (entities, recordsCount);

            _mockModelHelper.Setup(m => m.GetModelFields<GetSalaryRangesViewModel>())
                           .Returns("Id,Name,MinSalary,MaxSalary");
            _mockRepository.Setup(r => r.GetSalaryRangeReponseAsync(It.IsAny<GetSalaryRangesQuery>()))
                          .ReturnsAsync(repositoryResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.GetSalaryRangeReponseAsync(It.Is<GetSalaryRangesQuery>(q => 
                q.Name == "")), Times.Once);
        }
    }
}