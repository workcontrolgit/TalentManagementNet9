using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TalentManagement.Application.Features.Departments.Commands.CreateDepartment;
using TalentManagement.Application.Features.Departments.Commands.DeleteDepartment;
using TalentManagement.Application.Features.Departments.Commands.UpdateDepartment;
using TalentManagement.Application.Features.Departments.Queries.GetDepartmentById;
using TalentManagement.Application.Features.Departments.Queries.GetDepartments;
using TalentManagement.Application.Parameters;
using TalentManagement.Application.Wrappers;
using TalentManagement.Domain.Entities;
using TalentManagement.WebApi.Controllers.v1;
using Entity = TalentManagement.Domain.Entities.Entity;

namespace TalentManagement.Tests.Controllers
{
    public class DepartmentsControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly DepartmentsController _controller;

        public DepartmentsControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new DepartmentsController();
            
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
            var filter = new GetDepartmentsQuery { Name = "HR" };
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
            GetDepartmentsQuery filter = null;
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
            var departmentId = Guid.NewGuid();
            var department = new Department { Id = departmentId, Name = "Engineering" };
            var response = new Response<Department>(department);
            _mockMediator.Setup(m => m.Send(It.IsAny<GetDepartmentByIdQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(response);

            // Act
            var result = await _controller.Get(departmentId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockMediator.Verify(m => m.Send(It.Is<GetDepartmentByIdQuery>(q => q.Id == departmentId), 
                                           It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Get_EmptyGuidId_ShouldStillCallMediator()
        {
            // Arrange
            var departmentId = Guid.Empty;
            var department = new Department { Id = departmentId, Name = "Test" };
            var response = new Response<Department>(department);
            _mockMediator.Setup(m => m.Send(It.IsAny<GetDepartmentByIdQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(response);

            // Act
            var result = await _controller.Get(departmentId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockMediator.Verify(m => m.Send(It.Is<GetDepartmentByIdQuery>(q => q.Id == departmentId), 
                                           It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Post_ValidCommand_ShouldReturnCreatedAtActionResult()
        {
            // Arrange
            var command = new CreateDepartmentCommand { Name = "Marketing" };
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
            CreateDepartmentCommand command = null;
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
            var query = new PagedDepartmentsQuery 
            { 
                PageNumber = 1, 
                PageSize = 10,
                Search = new Search { Value = "test" }
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
            var departmentId = Guid.NewGuid();
            var command = new UpdateDepartmentCommand { Id = departmentId, Name = "Updated Department" };
            var response = new Response<Guid>(departmentId);
            _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(response);

            // Act
            var result = await _controller.Put(departmentId, command);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockMediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Put_MismatchedIds_ShouldReturnBadRequest()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var differentId = Guid.NewGuid();
            var command = new UpdateDepartmentCommand { Id = differentId, Name = "Updated Department" };

            // Act
            var result = await _controller.Put(departmentId, command);

            // Assert
            Assert.IsType<BadRequestResult>(result);
            _mockMediator.Verify(m => m.Send(It.IsAny<UpdateDepartmentCommand>(), It.IsAny<CancellationToken>()), 
                               Times.Never);
        }

        [Fact]
        public async Task Put_ValidCommand_ShouldReturnOkResult()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var command = new UpdateDepartmentCommand { Id = departmentId, Name = "Updated Department" };
            var response = new Response<Guid>(departmentId);
            _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(response);

            // Act
            var result = await _controller.Put(departmentId, command);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task Delete_ValidId_ShouldReturnOkResult()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var response = new Response<Guid>(departmentId);
            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteDepartmentCommand>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(response);

            // Act
            var result = await _controller.Delete(departmentId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockMediator.Verify(m => m.Send(It.Is<DeleteDepartmentCommand>(c => c.Id == departmentId), 
                                           It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_EmptyGuid_ShouldStillCallMediator()
        {
            // Arrange
            var departmentId = Guid.Empty;
            var response = new Response<Guid>(departmentId);
            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteDepartmentCommand>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(response);

            // Act
            var result = await _controller.Delete(departmentId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockMediator.Verify(m => m.Send(It.Is<DeleteDepartmentCommand>(c => c.Id == departmentId), 
                                           It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}