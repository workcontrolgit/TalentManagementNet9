using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TalentManagement.Application.Features.Employees.Commands.CreateEmployee;
using TalentManagement.Application.Features.Employees.Commands.DeleteEmployee;
using TalentManagement.Application.Features.Employees.Commands.UpdateEmployee;
using TalentManagement.Application.Features.Employees.Queries.GetEmployeeById;
using TalentManagement.Application.Features.Employees.Queries.GetEmployees;
using TalentManagement.Application.Parameters;
using TalentManagement.Application.Wrappers;
using TalentManagement.Domain.Common;
using TalentManagement.Domain.Entities;
using TalentManagement.Domain.Enums;
using TalentManagement.WebApi.Controllers.v1;

namespace TalentManagement.Tests.Controllers
{
    public class EmployeesControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly EmployeesController _controller;

        public EmployeesControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new EmployeesController();
            
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
            var filter = new GetEmployeesQuery { PageNumber = 1, PageSize = 10 };
            var expectedResponse = new PagedResponse<IEnumerable<Entity>>(
                new List<Entity>(), 1, 10, new RecordsCount());

            _mockMediator.Setup(m => m.Send(filter, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expectedResponse);

            var result = await _controller.Get(filter);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
            _mockMediator.Verify(m => m.Send(filter, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Get_WithSearchFilters_ShouldPassFiltersToMediator()
        {
            var filter = new GetEmployeesQuery 
            { 
                PageNumber = 1, 
                PageSize = 5,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@company.com",
                Gender = Gender.Male
            };

            var expectedResponse = new PagedResponse<IEnumerable<Entity>>(
                new List<Entity>(), 1, 5, new RecordsCount());

            _mockMediator.Setup(m => m.Send(It.IsAny<GetEmployeesQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expectedResponse);

            var result = await _controller.Get(filter);

            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockMediator.Verify(m => m.Send(It.Is<GetEmployeesQuery>(q => 
                q.FirstName == "John" && 
                q.LastName == "Doe" &&
                q.Email == "john@company.com" &&
                q.Gender == Gender.Male), 
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnOkResult()
        {
            var employeeId = Guid.NewGuid();
            var employee = new Employee 
            { 
                Id = employeeId, 
                FirstName = "John", 
                LastName = "Doe" 
            };
            var expectedResponse = new Response<Employee>(employee);

            _mockMediator.Setup(m => m.Send(It.Is<GetEmployeeByIdQuery>(q => q.Id == employeeId), 
                                          It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expectedResponse);

            var result = await _controller.Get(employeeId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
        }

        [Fact]
        public async Task Post_WithValidCommand_ShouldReturnCreatedResult()
        {
            var command = new CreateEmployeeCommand
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@company.com",
                EmployeeNumber = "EMP001",
                PositionId = Guid.NewGuid(),
                Salary = 75000m,
                Birthday = DateTime.Now.AddYears(-30),
                Gender = Gender.Female
            };

            var employeeId = Guid.NewGuid();
            var expectedResponse = new Response<Guid>(employeeId);

            _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expectedResponse);

            var result = await _controller.Post(command);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(expectedResponse, createdResult.Value);
            Assert.Equal(nameof(_controller.Post), createdResult.ActionName);
        }

        [Fact]
        public async Task Paged_WithQuery_ShouldReturnOkResult()
        {
            var query = new PagedEmployeesQuery 
            { 
                Draw = 1, 
                Start = 0, 
                Length = 10 
            };

            var expectedResponse = new PagedDataTableResponse<IEnumerable<Entity>>(
                new List<Entity>(), 1, new RecordsCount());

            _mockMediator.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expectedResponse);

            var result = await _controller.Paged(query);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
        }

        [Fact]
        public async Task Put_WithValidIdAndCommand_ShouldReturnOkResult()
        {
            var employeeId = Guid.NewGuid();
            var command = new UpdateEmployeeCommand
            {
                Id = employeeId,
                FirstName = "Updated",
                LastName = "Employee",
                Email = "updated@company.com",
                EmployeeNumber = "EMP001",
                PositionId = Guid.NewGuid(),
                Salary = 85000m,
                Birthday = DateTime.Now.AddYears(-35),
                Gender = Gender.Male
            };

            var expectedResponse = new Response<Guid>(employeeId);

            _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expectedResponse);

            var result = await _controller.Put(employeeId, command);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
        }

        [Fact]
        public async Task Put_WithMismatchedIds_ShouldReturnBadRequest()
        {
            var employeeId = Guid.NewGuid();
            var differentId = Guid.NewGuid();
            var command = new UpdateEmployeeCommand
            {
                Id = differentId,
                FirstName = "Test",
                LastName = "Employee"
            };

            var result = await _controller.Put(employeeId, command);

            Assert.IsType<BadRequestResult>(result);
            _mockMediator.Verify(m => m.Send(It.IsAny<UpdateEmployeeCommand>(), 
                                          It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WithValidId_ShouldReturnOkResult()
        {
            var employeeId = Guid.NewGuid();
            var expectedResponse = new Response<Guid>(employeeId);

            _mockMediator.Setup(m => m.Send(It.Is<DeleteEmployeeCommand>(c => c.Id == employeeId), 
                                          It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expectedResponse);

            var result = await _controller.Delete(employeeId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
        }

        [Fact]
        public async Task Get_WithEmptyFilter_ShouldReturnOkResult()
        {
            var filter = new GetEmployeesQuery();
            var expectedResponse = new PagedResponse<IEnumerable<Entity>>(
                new List<Entity>(), 1, 20, new RecordsCount());

            _mockMediator.Setup(m => m.Send(filter, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expectedResponse);

            var result = await _controller.Get(filter);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Post_WithCompleteEmployeeData_ShouldReturnCreatedResult()
        {
            var command = new CreateEmployeeCommand
            {
                FirstName = "Robert",
                MiddleName = "James",
                LastName = "Wilson",
                Email = "robert.wilson@company.com",
                EmployeeNumber = "EMP003",
                Phone = "+1-555-987-1234",
                Prefix = "Dr.",
                PositionId = Guid.NewGuid(),
                Salary = 95000.75m,
                Birthday = DateTime.Now.AddYears(-40),
                Gender = Gender.Male
            };

            var employeeId = Guid.NewGuid();
            var expectedResponse = new Response<Guid>(employeeId);

            _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expectedResponse);

            var result = await _controller.Post(command);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(expectedResponse, createdResult.Value);
        }

        [Fact]
        public async Task GetById_WithEmptyGuid_ShouldStillCallMediator()
        {
            var employeeId = Guid.Empty;
            var expectedResponse = new Response<Employee>(new Employee());

            _mockMediator.Setup(m => m.Send(It.Is<GetEmployeeByIdQuery>(q => q.Id == employeeId), 
                                          It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expectedResponse);

            var result = await _controller.Get(employeeId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockMediator.Verify(m => m.Send(It.Is<GetEmployeeByIdQuery>(q => q.Id == employeeId), 
                                          It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Put_WithMinimalData_ShouldReturnOkResult()
        {
            var employeeId = Guid.NewGuid();
            var command = new UpdateEmployeeCommand
            {
                Id = employeeId,
                FirstName = "Min",
                LastName = "Data",
                Email = "min@company.com",
                EmployeeNumber = "EMPMIN",
                PositionId = Guid.NewGuid(),
                Salary = 1m,
                Birthday = DateTime.Now.AddYears(-18),
                Gender = Gender.Female
            };

            var expectedResponse = new Response<Guid>(employeeId);

            _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expectedResponse);

            var result = await _controller.Put(employeeId, command);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
        }

        [Fact]
        public async Task Delete_WithEmptyGuid_ShouldStillCallMediator()
        {
            var employeeId = Guid.Empty;
            var expectedResponse = new Response<Guid>(employeeId);

            _mockMediator.Setup(m => m.Send(It.Is<DeleteEmployeeCommand>(c => c.Id == employeeId), 
                                          It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expectedResponse);

            var result = await _controller.Delete(employeeId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockMediator.Verify(m => m.Send(It.Is<DeleteEmployeeCommand>(c => c.Id == employeeId), 
                                          It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}