using Moq;
using TalentManagement.Application.Features.Positions.Queries.GetPositions;
using TalentManagement.Application.Interfaces;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Application.Parameters;
using TalentManagement.Application.Wrappers;
using TalentManagement.Domain.Entities;

namespace TalentManagement.Tests.Handlers.Positions
{
    public class GetPositionsQueryHandlerTests
    {
        private readonly Mock<IPositionRepositoryAsync> _mockRepository;
        private readonly Mock<IModelHelper> _mockModelHelper;
        private readonly GetAllPositionsQueryHandler _handler;

        public GetPositionsQueryHandlerTests()
        {
            _mockRepository = new Mock<IPositionRepositoryAsync>();
            _mockModelHelper = new Mock<IModelHelper>();
            _handler = new GetAllPositionsQueryHandler(_mockRepository.Object, _mockModelHelper.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnPagedResponse()
        {
            var query = new GetPositionsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                PositionTitle = "Engineer",
                Department = "IT"
            };

            var positions = new List<Entity>
            {
                new Entity(),
                new Entity()
            };

            var recordsCount = new RecordsCount { RecordsFiltered = 2, RecordsTotal = 2 };
            var repositoryResult = (data: positions, recordsCount: recordsCount);

            _mockModelHelper.Setup(m => m.GetModelFields<GetPositionsViewModel>())
                .Returns("Id,PositionTitle,PositionNumber");
            
            _mockRepository.Setup(r => r.GetPositionReponseAsync(It.IsAny<GetPositionsQuery>()))
                .ReturnsAsync(repositoryResult);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsType<PagedResponse<IEnumerable<Entity>>>(result);
            Assert.True(result.Succeeded);
            Assert.Equal(positions, result.Data);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(2, result.RecordsTotal);

            _mockRepository.Verify(r => r.GetPositionReponseAsync(It.IsAny<GetPositionsQuery>()), Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyFields_ShouldUseDefaultFields()
        {
            var query = new GetPositionsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Fields = ""
            };

            var positions = new List<Entity>();
            var recordsCount = new RecordsCount { RecordsFiltered = 0, RecordsTotal = 0 };
            var repositoryResult = (data: positions, recordsCount: recordsCount);

            _mockModelHelper.Setup(m => m.GetModelFields<GetPositionsViewModel>())
                .Returns("Id,PositionTitle,PositionNumber");
            
            _mockRepository.Setup(r => r.GetPositionReponseAsync(It.IsAny<GetPositionsQuery>()))
                .ReturnsAsync(repositoryResult);

            await _handler.Handle(query, CancellationToken.None);

            _mockModelHelper.Verify(m => m.GetModelFields<GetPositionsViewModel>(), Times.Once);
            Assert.Equal("Id,PositionTitle,PositionNumber", query.Fields);
        }

        [Fact]
        public async Task Handle_InvalidFields_ShouldValidateAndUseValidFields()
        {
            var query = new GetPositionsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Fields = "Id,InvalidField,PositionTitle"
            };

            var positions = new List<Entity>();
            var recordsCount = new RecordsCount { RecordsFiltered = 0, RecordsTotal = 0 };
            var repositoryResult = (data: positions, recordsCount: recordsCount);

            _mockModelHelper.Setup(m => m.ValidateModelFields<GetPositionsViewModel>("Id,InvalidField,PositionTitle"))
                .Returns("Id,PositionTitle");
            
            _mockRepository.Setup(r => r.GetPositionReponseAsync(It.IsAny<GetPositionsQuery>()))
                .ReturnsAsync(repositoryResult);

            await _handler.Handle(query, CancellationToken.None);

            _mockModelHelper.Verify(m => m.ValidateModelFields<GetPositionsViewModel>("Id,InvalidField,PositionTitle"), Times.Once);
            Assert.Equal("Id,PositionTitle", query.Fields);
        }

        [Fact]
        public async Task Handle_InvalidOrderBy_ShouldValidateOrderBy()
        {
            var query = new GetPositionsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                OrderBy = "InvalidField"
            };

            var positions = new List<Entity>();
            var recordsCount = new RecordsCount { RecordsFiltered = 0, RecordsTotal = 0 };
            var repositoryResult = (data: positions, recordsCount: recordsCount);

            _mockModelHelper.Setup(m => m.GetModelFields<GetPositionsViewModel>())
                .Returns("Id,PositionTitle");
            
            _mockModelHelper.Setup(m => m.ValidateModelFields<GetPositionsViewModel>("InvalidField"))
                .Returns("PositionTitle");
            
            _mockRepository.Setup(r => r.GetPositionReponseAsync(It.IsAny<GetPositionsQuery>()))
                .ReturnsAsync(repositoryResult);

            await _handler.Handle(query, CancellationToken.None);

            _mockModelHelper.Verify(m => m.ValidateModelFields<GetPositionsViewModel>("InvalidField"), Times.Once);
            Assert.Equal("PositionTitle", query.OrderBy);
        }

        [Fact]
        public async Task Handle_NullQuery_ShouldThrowNullReferenceException()
        {
            await Assert.ThrowsAsync<NullReferenceException>(() => 
                _handler.Handle(null, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            var query = new GetPositionsQuery { PageNumber = 1, PageSize = 10 };

            _mockModelHelper.Setup(m => m.GetModelFields<GetPositionsViewModel>())
                .Returns("Id,PositionTitle");
            
            _mockRepository.Setup(r => r.GetPositionReponseAsync(It.IsAny<GetPositionsQuery>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ModelHelperThrowsException_ShouldPropagateException()
        {
            var query = new GetPositionsQuery 
            { 
                PageNumber = 1, 
                PageSize = 10,
                Fields = "SomeField"
            };

            _mockModelHelper.Setup(m => m.ValidateModelFields<GetPositionsViewModel>(It.IsAny<string>()))
                .Throws(new InvalidOperationException("Model validation error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithFilters_ShouldPassFiltersToRepository()
        {
            var query = new GetPositionsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                PositionTitle = "Software Engineer",
                PositionNumber = "SE001",
                Department = "Engineering"
            };

            var positions = new List<Entity>();
            var recordsCount = new RecordsCount { RecordsFiltered = 0, RecordsTotal = 0 };
            var repositoryResult = (data: positions, recordsCount: recordsCount);

            _mockModelHelper.Setup(m => m.GetModelFields<GetPositionsViewModel>())
                .Returns("Id,PositionTitle");
            
            _mockRepository.Setup(r => r.GetPositionReponseAsync(It.IsAny<GetPositionsQuery>()))
                .ReturnsAsync(repositoryResult);

            await _handler.Handle(query, CancellationToken.None);

            _mockRepository.Verify(r => r.GetPositionReponseAsync(
                It.Is<GetPositionsQuery>(q => 
                    q.PositionTitle == "Software Engineer" &&
                    q.PositionNumber == "SE001" &&
                    q.Department == "Engineering")), Times.Once);
        }

        [Fact]
        public async Task Handle_CancellationRequested_ShouldHonorCancellation()
        {
            var query = new GetPositionsQuery { PageNumber = 1, PageSize = 10 };

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            _mockModelHelper.Setup(m => m.GetModelFields<GetPositionsViewModel>())
                .Returns("Id,PositionTitle");
            
            _mockRepository.Setup(r => r.GetPositionReponseAsync(It.IsAny<GetPositionsQuery>()))
                .Returns(Task.FromCanceled<(IEnumerable<Entity> data, RecordsCount recordsCount)>(cancellationTokenSource.Token));

            await Assert.ThrowsAsync<TaskCanceledException>(() => 
                _handler.Handle(query, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task Handle_ValidFieldsProvided_ShouldNotUseDefaultFields()
        {
            var query = new GetPositionsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Fields = "Id,PositionTitle"
            };

            var positions = new List<Entity>();
            var recordsCount = new RecordsCount { RecordsFiltered = 0, RecordsTotal = 0 };
            var repositoryResult = (data: positions, recordsCount: recordsCount);

            _mockModelHelper.Setup(m => m.ValidateModelFields<GetPositionsViewModel>("Id,PositionTitle"))
                .Returns("Id,PositionTitle");
            
            _mockRepository.Setup(r => r.GetPositionReponseAsync(It.IsAny<GetPositionsQuery>()))
                .ReturnsAsync(repositoryResult);

            await _handler.Handle(query, CancellationToken.None);

            _mockModelHelper.Verify(m => m.ValidateModelFields<GetPositionsViewModel>("Id,PositionTitle"), Times.Once);
            _mockModelHelper.Verify(m => m.GetModelFields<GetPositionsViewModel>(), Times.Never);
        }
    }
}