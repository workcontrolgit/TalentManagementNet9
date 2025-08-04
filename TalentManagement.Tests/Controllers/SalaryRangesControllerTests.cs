using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TalentManagement.Application.Features.SalaryRanges.Commands.CreateSalaryRange;
using TalentManagement.Application.Features.SalaryRanges.Commands.DeleteSalaryRange;
using TalentManagement.Application.Features.SalaryRanges.Commands.UpdateSalaryRange;
using TalentManagement.Application.Features.SalaryRanges.Queries.GetSalaryRangeById;
using TalentManagement.Application.Features.SalaryRanges.Queries.GetSalaryRanges;
using TalentManagement.Application.Parameters;
using TalentManagement.Application.Wrappers;
using TalentManagement.Domain.Entities;
using TalentManagement.WebApi.Controllers.v1;
using Entity = TalentManagement.Domain.Entities.Entity;

namespace TalentManagement.Tests.Controllers
{
    public class SalaryRangesControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly SalaryRangesController _controller;

        public SalaryRangesControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new SalaryRangesController();
            
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
            // Arrange
            var filter = new GetSalaryRangesQuery { Name = "Junior" };
            var response = new PagedResponse<IEnumerable<Entity>>(new List<Entity>(), 1, 10, new RecordsCount());
            _mockMediator.Setup(m => m.Send(filter, It.IsAny<CancellationToken>())).ReturnsAsync(response);

            // Act
            var result = await _controller.Get(filter);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockMediator.Verify(m => m.Send(filter, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Get_NullFilter_ShouldStillCallMediator()
        {
            // Arrange
            GetSalaryRangesQuery filter = null;
            var response = new PagedResponse<IEnumerable<Entity>>(new List<Entity>(), 1, 10, new RecordsCount());
            _mockMediator.Setup(m => m.Send(filter, It.IsAny<CancellationToken>())).ReturnsAsync(response);

            // Act
            var result = await _controller.Get(filter);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockMediator.Verify(m => m.Send(filter, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Get_WithId_ShouldReturnOkResult()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var salaryRange = new SalaryRange { Id = salaryRangeId, Name = "Senior Level", MinSalary = 80000m, MaxSalary = 120000m };
            var response = new Response<SalaryRange>(salaryRange);
            _mockMediator.Setup(m => m.Send(It.IsAny<GetSalaryRangeByIdQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(response);

            // Act
            var result = await _controller.Get(salaryRangeId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockMediator.Verify(m => m.Send(It.Is<GetSalaryRangeByIdQuery>(q => q.Id == salaryRangeId), 
                                           It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Get_EmptyGuidId_ShouldStillCallMediator()
        {
            // Arrange
            var salaryRangeId = Guid.Empty;
            var salaryRange = new SalaryRange { Id = salaryRangeId, Name = "Test", MinSalary = 0m, MaxSalary = 0m };
            var response = new Response<SalaryRange>(salaryRange);
            _mockMediator.Setup(m => m.Send(It.IsAny<GetSalaryRangeByIdQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(response);

            // Act
            var result = await _controller.Get(salaryRangeId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockMediator.Verify(m => m.Send(It.Is<GetSalaryRangeByIdQuery>(q => q.Id == salaryRangeId), 
                                           It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Post_ValidCommand_ShouldReturnCreatedAtActionResult()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand { Name = "Entry Level", MinSalary = 40000m, MaxSalary = 60000m };
            var response = new Response<Guid>(Guid.NewGuid());
            _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(response);

            // Act
            var result = await _controller.Post(command);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
            _mockMediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Post_NullCommand_ShouldStillCallMediator()
        {
            // Arrange
            CreateSalaryRangeCommand command = null;
            var response = new Response<Guid>(Guid.NewGuid());
            _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(response);

            // Act
            var result = await _controller.Post(command);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
            _mockMediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Paged_ValidQuery_ShouldReturnOkResult()
        {
            // Arrange
            var query = new PagedSalaryRangesQuery 
            { 
                PageNumber = 1, 
                PageSize = 10,
                Search = new Search { Value = "senior" }
            };
            var response = new PagedDataTableResponse<IEnumerable<Entity>>(new List<Entity>(), 1, new RecordsCount());
            _mockMediator.Setup(m => m.Send(query, It.IsAny<CancellationToken>())).ReturnsAsync(response);

            // Act
            var result = await _controller.Paged(query);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockMediator.Verify(m => m.Send(query, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Put_SameIds_ShouldCallMediator()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var command = new UpdateSalaryRangeCommand { Id = salaryRangeId, Name = "Updated Range", MinSalary = 50000m, MaxSalary = 80000m };
            var response = new Response<Guid>(salaryRangeId);
            _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(response);

            // Act
            var result = await _controller.Put(salaryRangeId, command);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockMediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Put_MismatchedIds_ShouldReturnBadRequest()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var differentId = Guid.NewGuid();
            var command = new UpdateSalaryRangeCommand { Id = differentId, Name = "Updated Range", MinSalary = 50000m, MaxSalary = 80000m };

            // Act
            var result = await _controller.Put(salaryRangeId, command);

            // Assert
            Assert.IsType<BadRequestResult>(result);
            _mockMediator.Verify(m => m.Send(It.IsAny<UpdateSalaryRangeCommand>(), It.IsAny<CancellationToken>()), 
                               Times.Never);
        }

        [Fact]
        public async Task Put_ValidCommand_ShouldReturnOkResult()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var command = new UpdateSalaryRangeCommand { Id = salaryRangeId, Name = "Updated Range", MinSalary = 55000m, MaxSalary = 85000m };
            var response = new Response<Guid>(salaryRangeId);
            _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(response);

            // Act
            var result = await _controller.Put(salaryRangeId, command);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task Delete_ValidId_ShouldReturnOkResult()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var response = new Response<Guid>(salaryRangeId);
            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteSalaryRangeCommand>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(response);

            // Act
            var result = await _controller.Delete(salaryRangeId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockMediator.Verify(m => m.Send(It.Is<DeleteSalaryRangeCommand>(c => c.Id == salaryRangeId), 
                                           It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_EmptyGuid_ShouldStillCallMediator()
        {
            // Arrange
            var salaryRangeId = Guid.Empty;
            var response = new Response<Guid>(salaryRangeId);
            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteSalaryRangeCommand>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(response);

            // Act
            var result = await _controller.Delete(salaryRangeId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockMediator.Verify(m => m.Send(It.Is<DeleteSalaryRangeCommand>(c => c.Id == salaryRangeId), 
                                           It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Post_CommandWithZeroSalaries_ShouldStillCallMediator()
        {
            // Arrange
            var command = new CreateSalaryRangeCommand { Name = "Intern", MinSalary = 0m, MaxSalary = 0m };
            var response = new Response<Guid>(Guid.NewGuid());
            _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(response);

            // Act
            var result = await _controller.Post(command);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
            _mockMediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Put_CommandWithHighSalaries_ShouldCallMediator()
        {
            // Arrange
            var salaryRangeId = Guid.NewGuid();
            var command = new UpdateSalaryRangeCommand 
            { 
                Id = salaryRangeId, 
                Name = "Executive", 
                MinSalary = 200000m, 
                MaxSalary = 500000m 
            };
            var response = new Response<Guid>(salaryRangeId);
            _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(response);

            // Act
            var result = await _controller.Put(salaryRangeId, command);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockMediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}