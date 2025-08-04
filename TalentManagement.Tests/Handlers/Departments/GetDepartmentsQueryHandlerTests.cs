using Moq;
using TalentManagement.Application.Features.Departments.Queries.GetDepartments;
using TalentManagement.Application.Interfaces;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Application.Parameters;
using TalentManagement.Domain.Entities;
using Entity = TalentManagement.Domain.Entities.Entity;

namespace TalentManagement.Tests.Handlers.Departments
{
    public class GetDepartmentsQueryHandlerTests
    {
        private readonly Mock<IDepartmentRepositoryAsync> _mockRepository;
        private readonly Mock<IModelHelper> _mockModelHelper;
        private readonly GetAllDepartmentsQueryHandler _handler;

        public GetDepartmentsQueryHandlerTests()
        {
            _mockRepository = new Mock<IDepartmentRepositoryAsync>();
            _mockModelHelper = new Mock<IModelHelper>();
            _handler = new GetAllDepartmentsQueryHandler(_mockRepository.Object, _mockModelHelper.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnPagedResponse()
        {
            // Arrange
            var query = new GetDepartmentsQuery { PageNumber = 1, PageSize = 10, Name = "Engineering" };
            var entities = new List<Entity> { new Entity() };
            var recordsCount = new RecordsCount { RecordsTotal = 10, RecordsFiltered = 5 };
            var repositoryResponse = (entities, recordsCount);

            _mockModelHelper.Setup(m => m.ValidateModelFields<GetDepartmentsViewModel>(It.IsAny<string>()))
                           .Returns("Id,Name");
            _mockModelHelper.Setup(m => m.GetModelFields<GetDepartmentsViewModel>())
                           .Returns("Id,Name");
            _mockRepository.Setup(r => r.GetDepartmentReponseAsync(It.IsAny<GetDepartmentsQuery>()))
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
            var query = new GetDepartmentsQuery { PageNumber = 1, PageSize = 10, Fields = "Id,Name,InvalidField" };
            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();
            var repositoryResponse = (entities, recordsCount);

            _mockModelHelper.Setup(m => m.ValidateModelFields<GetDepartmentsViewModel>("Id,Name,InvalidField"))
                           .Returns("Id,Name");
            _mockRepository.Setup(r => r.GetDepartmentReponseAsync(It.IsAny<GetDepartmentsQuery>()))
                          .ReturnsAsync(repositoryResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            _mockModelHelper.Verify(m => m.ValidateModelFields<GetDepartmentsViewModel>("Id,Name,InvalidField"), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNullFields_ShouldUseDefaultFields()
        {
            // Arrange
            var query = new GetDepartmentsQuery { PageNumber = 1, PageSize = 10, Fields = null };
            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();
            var repositoryResponse = (entities, recordsCount);

            _mockModelHelper.Setup(m => m.GetModelFields<GetDepartmentsViewModel>())
                           .Returns("Id,Name");
            _mockRepository.Setup(r => r.GetDepartmentReponseAsync(It.IsAny<GetDepartmentsQuery>()))
                          .ReturnsAsync(repositoryResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            _mockModelHelper.Verify(m => m.GetModelFields<GetDepartmentsViewModel>(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithEmptyFields_ShouldUseDefaultFields()
        {
            // Arrange
            var query = new GetDepartmentsQuery { PageNumber = 1, PageSize = 10, Fields = "" };
            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();
            var repositoryResponse = (entities, recordsCount);

            _mockModelHelper.Setup(m => m.GetModelFields<GetDepartmentsViewModel>())
                           .Returns("Id,Name");
            _mockRepository.Setup(r => r.GetDepartmentReponseAsync(It.IsAny<GetDepartmentsQuery>()))
                          .ReturnsAsync(repositoryResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            _mockModelHelper.Verify(m => m.GetModelFields<GetDepartmentsViewModel>(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidFields_ShouldUseDefaultFields()
        {
            // Arrange
            var query = new GetDepartmentsQuery { PageNumber = 1, PageSize = 10, Fields = "InvalidField" };
            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();
            var repositoryResponse = (entities, recordsCount);

            _mockModelHelper.Setup(m => m.ValidateModelFields<GetDepartmentsViewModel>("InvalidField"))
                           .Returns("");
            _mockModelHelper.Setup(m => m.GetModelFields<GetDepartmentsViewModel>())
                           .Returns("Id,Name");
            _mockRepository.Setup(r => r.GetDepartmentReponseAsync(It.IsAny<GetDepartmentsQuery>()))
                          .ReturnsAsync(repositoryResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            _mockModelHelper.Verify(m => m.GetModelFields<GetDepartmentsViewModel>(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithValidOrderBy_ShouldValidateOrderBy()
        {
            // Arrange
            var query = new GetDepartmentsQuery { PageNumber = 1, PageSize = 10, OrderBy = "Name desc" };
            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();
            var repositoryResponse = (entities, recordsCount);

            _mockModelHelper.Setup(m => m.ValidateModelFields<GetDepartmentsViewModel>("Name desc"))
                           .Returns("Name desc");
            _mockModelHelper.Setup(m => m.GetModelFields<GetDepartmentsViewModel>())
                           .Returns("Id,Name");
            _mockRepository.Setup(r => r.GetDepartmentReponseAsync(It.IsAny<GetDepartmentsQuery>()))
                          .ReturnsAsync(repositoryResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            _mockModelHelper.Verify(m => m.ValidateModelFields<GetDepartmentsViewModel>("Name desc"), Times.Once);
        }

        [Fact]
        public async Task Handle_WithFilters_ShouldPassFiltersToRepository()
        {
            // Arrange
            var query = new GetDepartmentsQuery 
            { 
                PageNumber = 1, 
                PageSize = 10, 
                Name = "Engineering"
            };
            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();
            var repositoryResponse = (entities, recordsCount);

            _mockModelHelper.Setup(m => m.GetModelFields<GetDepartmentsViewModel>())
                           .Returns("Id,Name");
            _mockRepository.Setup(r => r.GetDepartmentReponseAsync(It.IsAny<GetDepartmentsQuery>()))
                          .ReturnsAsync(repositoryResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.GetDepartmentReponseAsync(It.Is<GetDepartmentsQuery>(q => 
                q.Name == "Engineering")), Times.Once);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var query = new GetDepartmentsQuery { PageNumber = 1, PageSize = 10 };

            _mockModelHelper.Setup(m => m.GetModelFields<GetDepartmentsViewModel>())
                           .Returns("Id,Name");
            _mockRepository.Setup(r => r.GetDepartmentReponseAsync(It.IsAny<GetDepartmentsQuery>()))
                          .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("Database error", exception.Message);
        }

        [Fact]
        public async Task Handle_ModelHelperThrowsException_ShouldPropagateException()
        {
            // Arrange
            var query = new GetDepartmentsQuery { PageNumber = 1, PageSize = 10 };

            _mockModelHelper.Setup(m => m.GetModelFields<GetDepartmentsViewModel>())
                           .Throws(new InvalidOperationException("Model helper error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("Model helper error", exception.Message);
        }

        [Fact]
        public async Task Handle_NullQuery_ShouldThrowNullReferenceException()
        {
            // Arrange
            GetDepartmentsQuery query = null;

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(query, CancellationToken.None));
        }

    }
}