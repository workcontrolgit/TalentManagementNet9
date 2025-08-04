using Moq;
using TalentManagement.Application.Features.Employees.Queries.GetEmployees;
using TalentManagement.Application.Interfaces.Repositories;
using TalentManagement.Application.Interfaces;
using TalentManagement.Application.Parameters;
using TalentManagement.Application.Wrappers;
using TalentManagement.Domain.Common;
using TalentManagement.Domain.Enums;
using Entity = TalentManagement.Domain.Entities.Entity;

namespace TalentManagement.Tests.Handlers.Employees
{
    public class GetEmployeesQueryHandlerTests
    {
        private readonly Mock<IEmployeeRepositoryAsync> _mockRepository;
        private readonly Mock<IModelHelper> _mockModelHelper;
        private readonly GetAllEmployeesQueryHandler _handler;

        public GetEmployeesQueryHandlerTests()
        {
            _mockRepository = new Mock<IEmployeeRepositoryAsync>();
            _mockModelHelper = new Mock<IModelHelper>();
            _handler = new GetAllEmployeesQueryHandler(_mockRepository.Object, _mockModelHelper.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnPagedResponse()
        {
            var query = new GetEmployeesQuery
            {
                PageNumber = 1,
                PageSize = 10,
                FirstName = "John"
            };

            var entities = new List<Entity>
            {
                new Entity(),
                new Entity()
            };

            var recordsCount = new RecordsCount
            {
                RecordsTotal = 100,
                RecordsFiltered = 2
            };

            _mockModelHelper.Setup(m => m.ValidateModelFields<GetEmployeesViewModel>(It.IsAny<string>()))
                           .Returns("Id,FirstName,LastName");

            _mockModelHelper.Setup(m => m.GetModelFields<GetEmployeesViewModel>())
                           .Returns("Id,FirstName,LastName,Email,EmployeeNumber");

            _mockRepository.Setup(r => r.GetEmployeeResponseAsync(query))
                          .ReturnsAsync((entities, recordsCount));

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Equal(entities, result.Data);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(100, result.RecordsTotal);

            _mockRepository.Verify(r => r.GetEmployeeResponseAsync(query), Times.Once);
        }

        [Fact]
        public async Task Handle_WithValidFields_ShouldValidateFields()
        {
            var query = new GetEmployeesQuery
            {
                PageNumber = 1,
                PageSize = 5,
                Fields = "FirstName,LastName,Email"
            };

            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();

            _mockModelHelper.Setup(m => m.ValidateModelFields<GetEmployeesViewModel>("FirstName,LastName,Email"))
                           .Returns("FirstName,LastName,Email");

            _mockRepository.Setup(r => r.GetEmployeeResponseAsync(query))
                          .ReturnsAsync((entities, recordsCount));

            await _handler.Handle(query, CancellationToken.None);

            _mockModelHelper.Verify(m => m.ValidateModelFields<GetEmployeesViewModel>("FirstName,LastName,Email"), Times.Once);
            Assert.Equal("FirstName,LastName,Email", query.Fields);
        }

        [Fact]
        public async Task Handle_WithInvalidFields_ShouldUseDefaultFields()
        {
            var query = new GetEmployeesQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Fields = "InvalidField"
            };

            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();

            _mockModelHelper.Setup(m => m.ValidateModelFields<GetEmployeesViewModel>("InvalidField"))
                           .Returns("");

            _mockModelHelper.Setup(m => m.GetModelFields<GetEmployeesViewModel>())
                           .Returns("Id,FirstName,LastName,Email");

            _mockRepository.Setup(r => r.GetEmployeeResponseAsync(query))
                          .ReturnsAsync((entities, recordsCount));

            await _handler.Handle(query, CancellationToken.None);

            Assert.Equal("", query.Fields); // ValidateModelFields returned empty, so Fields becomes empty
            _mockModelHelper.Verify(m => m.ValidateModelFields<GetEmployeesViewModel>("InvalidField"), Times.Once);
        }

        [Fact]
        public async Task Handle_WithValidOrderBy_ShouldGetDefaultFields()
        {
            var query = new GetEmployeesQuery
            {
                PageNumber = 1,
                PageSize = 10,
                OrderBy = "FirstName"
                // No Fields property, so should call GetModelFields
            };

            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();

            _mockModelHelper.Setup(m => m.GetModelFields<GetEmployeesViewModel>())
                           .Returns("Id,FirstName,LastName");

            _mockRepository.Setup(r => r.GetEmployeeResponseAsync(query))
                          .ReturnsAsync((entities, recordsCount));

            await _handler.Handle(query, CancellationToken.None);

            _mockModelHelper.Verify(m => m.GetModelFields<GetEmployeesViewModel>(), Times.Once);
            Assert.Equal("Id,FirstName,LastName", query.Fields); // Should be set to default fields
            Assert.Equal("FirstName", query.OrderBy); // OrderBy should remain unchanged
        }

        [Fact]
        public async Task Handle_WithSearchFilters_ShouldPassFiltersToRepository()
        {
            var query = new GetEmployeesQuery
            {
                PageNumber = 1,
                PageSize = 10,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@company.com",
                EmployeeNumber = "EMP001",
                Phone = "555-1234",
                Prefix = "Mr.",
                PositionTitle = "Engineer",
                Gender = Gender.Male,
                MinSalary = 50000,
                MaxSalary = 100000,
                BirthdayFrom = DateTime.Today.AddYears(-40),
                BirthdayTo = DateTime.Today.AddYears(-20)
            };

            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();

            _mockModelHelper.Setup(m => m.GetModelFields<GetEmployeesViewModel>())
                           .Returns("Id,FirstName,LastName");

            _mockRepository.Setup(r => r.GetEmployeeResponseAsync(query))
                          .ReturnsAsync((entities, recordsCount));

            await _handler.Handle(query, CancellationToken.None);

            _mockRepository.Verify(r => r.GetEmployeeResponseAsync(It.Is<GetEmployeesQuery>(q =>
                q.FirstName == "John" &&
                q.LastName == "Doe" &&
                q.Email == "john@company.com" &&
                q.EmployeeNumber == "EMP001" &&
                q.Phone == "555-1234" &&
                q.Prefix == "Mr." &&
                q.PositionTitle == "Engineer" &&
                q.Gender == Gender.Male &&
                q.MinSalary == 50000 &&
                q.MaxSalary == 100000
            )), Times.Once);
        }

        [Fact]
        public async Task Handle_WithEmptyFilters_ShouldStillWork()
        {
            var query = new GetEmployeesQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();

            _mockModelHelper.Setup(m => m.GetModelFields<GetEmployeesViewModel>())
                           .Returns("Id,FirstName,LastName");

            _mockRepository.Setup(r => r.GetEmployeeResponseAsync(query))
                          .ReturnsAsync((entities, recordsCount));

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            _mockRepository.Verify(r => r.GetEmployeeResponseAsync(query), Times.Once);
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            var query = new GetEmployeesQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            _mockModelHelper.Setup(m => m.GetModelFields<GetEmployeesViewModel>())
                           .Returns("Id,FirstName");

            _mockRepository.Setup(r => r.GetEmployeeResponseAsync(query))
                          .ThrowsAsync(new InvalidOperationException("Database error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithNullFields_ShouldUseDefaultFields()
        {
            var query = new GetEmployeesQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Fields = null
            };

            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();

            _mockModelHelper.Setup(m => m.GetModelFields<GetEmployeesViewModel>())
                           .Returns("Id,FirstName,LastName,Email");

            _mockRepository.Setup(r => r.GetEmployeeResponseAsync(query))
                          .ReturnsAsync((entities, recordsCount));

            await _handler.Handle(query, CancellationToken.None);

            Assert.Equal("Id,FirstName,LastName,Email", query.Fields);
            _mockModelHelper.Verify(m => m.GetModelFields<GetEmployeesViewModel>(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithDateRangeFilters_ShouldPassCorrectDates()
        {
            var birthdayFrom = DateTime.Today.AddYears(-50);
            var birthdayTo = DateTime.Today.AddYears(-18);

            var query = new GetEmployeesQuery
            {
                PageNumber = 1,
                PageSize = 10,
                BirthdayFrom = birthdayFrom,
                BirthdayTo = birthdayTo
            };

            var entities = new List<Entity>();
            var recordsCount = new RecordsCount();

            _mockModelHelper.Setup(m => m.GetModelFields<GetEmployeesViewModel>())
                           .Returns("Id,FirstName,LastName");

            _mockRepository.Setup(r => r.GetEmployeeResponseAsync(query))
                          .ReturnsAsync((entities, recordsCount));

            await _handler.Handle(query, CancellationToken.None);

            _mockRepository.Verify(r => r.GetEmployeeResponseAsync(It.Is<GetEmployeesQuery>(q =>
                q.BirthdayFrom == birthdayFrom &&
                q.BirthdayTo == birthdayTo
            )), Times.Once);
        }
    }
}