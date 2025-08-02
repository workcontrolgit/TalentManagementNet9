using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TalentManagement.Application.Features.Positions.Commands.CreatePosition;
using TalentManagement.Application.Features.Positions.Commands.DeletePosition;
using TalentManagement.Application.Features.Positions.Commands.UpdatePosition;
using TalentManagement.Application.Features.Positions.Queries.GetPositionById;
using TalentManagement.Application.Features.Positions.Queries.GetPositions;
using TalentManagement.Application.Wrappers;
using TalentManagement.Domain.Entities;
using TalentManagement.WebApi.Controllers.v1;

namespace TalentManagement.Tests.Controllers
{
    public class PositionsControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly PositionsController _controller;

        public PositionsControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new PositionsController();
            
            // Setup HttpContext with service provider containing our mock mediator
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_mockMediator.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task Get_WithFilter_ShouldReturnOkResult()
        {
            var filter = new GetPositionsQuery { PageNumber = 1, PageSize = 10 };
            var expectedResponse = new PagedResponse<IEnumerable<Entity>>(
                new List<Entity>(), 1, 10, new Application.Parameters.RecordsCount());

            _mockMediator.Setup(m => m.Send(filter, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var result = await _controller.Get(filter);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
            _mockMediator.Verify(m => m.Send(filter, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Get_WithId_ShouldReturnOkResult()
        {
            var positionId = Guid.NewGuid();
            var expectedPosition = new Position { Id = positionId, PositionTitle = "Software Engineer" };
            var expectedResponse = new Response<Position>(expectedPosition);

            _mockMediator.Setup(m => m.Send(It.Is<GetPositionByIdQuery>(q => q.Id == positionId), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var result = await _controller.Get(positionId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
            _mockMediator.Verify(m => m.Send(It.Is<GetPositionByIdQuery>(q => q.Id == positionId), 
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Post_ValidCommand_ShouldReturnCreatedAtActionResult()
        {
            var command = new CreatePositionCommand
            {
                PositionTitle = "Software Engineer",
                PositionNumber = "SE001",
                PositionDescription = "Develops software",
                DepartmentId = Guid.NewGuid(),
                SalaryRangeId = Guid.NewGuid()
            };

            var expectedResponse = new Response<Guid>(Guid.NewGuid());

            _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var result = await _controller.Post(command);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.Post), createdResult.ActionName);
            Assert.Equal(expectedResponse, createdResult.Value);
            _mockMediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Put_ValidCommand_ShouldReturnOkResult()
        {
            var positionId = Guid.NewGuid();
            var command = new UpdatePositionCommand
            {
                Id = positionId,
                PositionTitle = "Senior Software Engineer",
                PositionDescription = "Updated description"
            };

            var expectedResponse = new Response<Guid>(positionId);

            _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var result = await _controller.Put(positionId, command);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
            _mockMediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Put_MismatchedIds_ShouldReturnBadRequest()
        {
            var positionId = Guid.NewGuid();
            var differentId = Guid.NewGuid();
            var command = new UpdatePositionCommand { Id = differentId };

            var result = await _controller.Put(positionId, command);

            Assert.IsType<BadRequestResult>(result);
            _mockMediator.Verify(m => m.Send(It.IsAny<UpdatePositionCommand>(), It.IsAny<CancellationToken>()), 
                Times.Never);
        }

        [Fact]
        public async Task Delete_ValidId_ShouldReturnOkResult()
        {
            var positionId = Guid.NewGuid();
            var expectedResponse = new Response<Guid>(positionId);

            _mockMediator.Setup(m => m.Send(It.Is<DeletePositionCommand>(c => c.Id == positionId), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var result = await _controller.Delete(positionId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
            _mockMediator.Verify(m => m.Send(It.Is<DeletePositionCommand>(c => c.Id == positionId), 
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddMock_ValidCommand_ShouldReturnOkResult()
        {
            var command = new InsertMockPositionCommand();
            var expectedResponse = new Response<int>(1);

            _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var result = await _controller.AddMock(command);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
            _mockMediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Paged_ValidQuery_ShouldReturnOkResult()
        {
            var query = new PagedPositionsQuery { Draw = 1, Start = 0, Length = 10 };
            var expectedResponse = new PagedDataTableResponse<IEnumerable<Entity>>(
                new List<Entity>(), 1, new Application.Parameters.RecordsCount());

            _mockMediator.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var result = await _controller.Paged(query);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
            _mockMediator.Verify(m => m.Send(query, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Get_NullFilter_ShouldStillCallMediator()
        {
            GetPositionsQuery filter = null;
            var expectedResponse = new PagedResponse<IEnumerable<Entity>>(
                new List<Entity>(), 1, 10, new Application.Parameters.RecordsCount());

            _mockMediator.Setup(m => m.Send(filter, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var result = await _controller.Get(filter);

            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockMediator.Verify(m => m.Send(filter, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Get_EmptyGuidId_ShouldStillCallMediator()
        {
            var positionId = Guid.Empty;
            var expectedResponse = new Response<Position>(new Position());

            _mockMediator.Setup(m => m.Send(It.Is<GetPositionByIdQuery>(q => q.Id == Guid.Empty), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var result = await _controller.Get(positionId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockMediator.Verify(m => m.Send(It.Is<GetPositionByIdQuery>(q => q.Id == Guid.Empty), 
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Post_NullCommand_ShouldStillCallMediator()
        {
            CreatePositionCommand command = null;
            var expectedResponse = new Response<Guid>(Guid.NewGuid());

            _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var result = await _controller.Post(command);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            _mockMediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Put_SameIds_ShouldCallMediator()
        {
            var positionId = Guid.NewGuid();
            var command = new UpdatePositionCommand { Id = positionId };
            var expectedResponse = new Response<Guid>(positionId);

            _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var result = await _controller.Put(positionId, command);

            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockMediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_EmptyGuid_ShouldStillCallMediator()
        {
            var positionId = Guid.Empty;
            var expectedResponse = new Response<Guid>(Guid.Empty);

            _mockMediator.Setup(m => m.Send(It.Is<DeletePositionCommand>(c => c.Id == Guid.Empty), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var result = await _controller.Delete(positionId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockMediator.Verify(m => m.Send(It.Is<DeletePositionCommand>(c => c.Id == Guid.Empty), 
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}