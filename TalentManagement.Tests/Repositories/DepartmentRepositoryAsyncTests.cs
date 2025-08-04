using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TalentManagement.Application.Features.Departments.Queries.GetDepartments;
using TalentManagement.Application.Interfaces;
using TalentManagement.Application.Parameters;
using TalentManagement.Domain.Entities;
using TalentManagement.Infrastructure.Persistence.Contexts;
using TalentManagement.Infrastructure.Persistence.Repositories;
using Entity = TalentManagement.Domain.Entities.Entity;

namespace TalentManagement.Tests.Repositories
{
    public class DepartmentRepositoryAsyncTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IDataShapeHelper<Department>> _mockDataShapeHelper;
        private readonly DepartmentRepositoryAsync _repository;
        private readonly List<Department> _testDepartments;

        public DepartmentRepositoryAsyncTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var mockDateTimeService = new Mock<IDateTimeService>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();

            _context = new ApplicationDbContext(options, mockDateTimeService.Object, mockLoggerFactory.Object);
            _mockDataShapeHelper = new Mock<IDataShapeHelper<Department>>();
            _repository = new DepartmentRepositoryAsync(_context, _mockDataShapeHelper.Object);

            _testDepartments = new List<Department>();

            SeedTestData();
        }

        private void SeedTestData()
        {
            var departments = new List<Department>
            {
                new Department { Id = Guid.NewGuid(), Name = "Human Resources" },
                new Department { Id = Guid.NewGuid(), Name = "Engineering" },
                new Department { Id = Guid.NewGuid(), Name = "Marketing" },
                new Department { Id = Guid.NewGuid(), Name = "Finance" },
                new Department { Id = Guid.NewGuid(), Name = "Operations" }
            };

            _context.Departments.AddRange(departments);
            _context.SaveChanges();

            _testDepartments.AddRange(departments);
        }

        [Fact]
        public async Task AddAsync_ShouldAddDepartmentToDatabase()
        {
            var departmentId = Guid.NewGuid();
            var newDepartment = new Department
            {
                Id = departmentId,
                Name = "Test Department"
            };

            var result = await _repository.AddAsync(newDepartment);

            Assert.NotNull(result);
            Assert.Equal(departmentId, newDepartment.Id);

            var savedDepartment = await _context.Departments.FindAsync(departmentId);
            Assert.NotNull(savedDepartment);
            Assert.Equal("Test Department", savedDepartment.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnDepartment()
        {
            var departmentId = _testDepartments[0].Id;

            var result = await _repository.GetByIdAsync(departmentId);

            Assert.NotNull(result);
            Assert.Equal(departmentId, result.Id);
            Assert.Equal(_testDepartments[0].Name, result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            var invalidId = Guid.NewGuid();

            var result = await _repository.GetByIdAsync(invalidId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllDepartments()
        {
            var result = await _repository.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(_testDepartments.Count, result.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateDepartmentInDatabase()
        {
            var departmentToUpdate = _testDepartments[0];
            departmentToUpdate.Name = "Updated Department Name";

            await _repository.UpdateAsync(departmentToUpdate);

            var updatedDepartment = await _context.Departments.FindAsync(departmentToUpdate.Id);
            Assert.NotNull(updatedDepartment);
            Assert.Equal("Updated Department Name", updatedDepartment.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveDepartmentFromDatabase()
        {
            var departmentToDelete = _testDepartments[0];

            await _repository.DeleteAsync(departmentToDelete);

            var deletedDepartment = await _context.Departments.FindAsync(departmentToDelete.Id);
            Assert.Null(deletedDepartment);
        }

        [Fact]
        public async Task GetDepartmentReponseAsync_WithNoFilters_ShouldReturnAllDepartments()
        {
            _mockDataShapeHelper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<Department>>(), It.IsAny<string>()))
                              .Returns((IEnumerable<Department> source, string fields) => 
                                  source.Select(d => new Entity()).ToList());

            var query = new GetDepartmentsQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            var result = await _repository.GetDepartmentReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(_testDepartments.Count, result.recordsCount.RecordsTotal);
            Assert.Equal(_testDepartments.Count, result.recordsCount.RecordsFiltered);
        }

        [Fact]
        public async Task GetDepartmentReponseAsync_WithNameFilter_ShouldReturnFilteredDepartments()
        {
            _mockDataShapeHelper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<Department>>(), It.IsAny<string>()))
                              .Returns((IEnumerable<Department> source, string fields) => 
                                  source.Select(d => new Entity()).ToList());

            var query = new GetDepartmentsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Name = "Engineering"
            };

            var result = await _repository.GetDepartmentReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(_testDepartments.Count, result.recordsCount.RecordsTotal);
            Assert.Equal(1, result.recordsCount.RecordsFiltered);
        }

        [Fact]
        public async Task GetDepartmentReponseAsync_WithPaging_ShouldReturnCorrectPage()
        {
            _mockDataShapeHelper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<Department>>(), It.IsAny<string>()))
                              .Returns((IEnumerable<Department> source, string fields) => 
                                  source.Select(d => new Entity()).ToList());

            var query = new GetDepartmentsQuery
            {
                PageNumber = 2,
                PageSize = 2
            };

            var result = await _repository.GetDepartmentReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(_testDepartments.Count, result.recordsCount.RecordsTotal);
            Assert.Equal(_testDepartments.Count, result.recordsCount.RecordsFiltered);
            Assert.Equal(2, result.data.Count());
        }

        [Fact]
        public async Task GetDepartmentReponseAsync_WithOrderBy_ShouldReturnOrderedResults()
        {
            _mockDataShapeHelper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<Department>>(), It.IsAny<string>()))
                              .Returns((IEnumerable<Department> source, string fields) => 
                                  source.Select(d => new Entity()).ToList());

            var query = new GetDepartmentsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                OrderBy = "Name desc"
            };

            var result = await _repository.GetDepartmentReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(_testDepartments.Count, result.recordsCount.RecordsTotal);
        }

        [Fact]
        public async Task GetDepartmentReponseAsync_WithFields_ShouldApplyFieldSelection()
        {
            _mockDataShapeHelper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<Department>>(), It.IsAny<string>()))
                              .Returns((IEnumerable<Department> source, string fields) => 
                                  source.Select(d => new Entity()).ToList());

            var query = new GetDepartmentsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Fields = "Id,Name"
            };

            var result = await _repository.GetDepartmentReponseAsync(query);

            Assert.NotNull(result.data);
            _mockDataShapeHelper.Verify(x => x.ShapeData(It.IsAny<IEnumerable<Department>>(), "Id,Name"), Times.Once);
        }

        [Fact]
        public async Task GetDepartmentReponseAsync_WithEmptyNameFilter_ShouldReturnAllDepartments()
        {
            _mockDataShapeHelper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<Department>>(), It.IsAny<string>()))
                              .Returns((IEnumerable<Department> source, string fields) => 
                                  source.Select(d => new Entity()).ToList());

            var query = new GetDepartmentsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Name = ""
            };

            var result = await _repository.GetDepartmentReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(_testDepartments.Count, result.recordsCount.RecordsTotal);
            Assert.Equal(_testDepartments.Count, result.recordsCount.RecordsFiltered);
        }

        [Fact]
        public async Task PagedDepartmentReponseAsync_WithEmptySearch_ShouldReturnAllResults()
        {
            _mockDataShapeHelper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<Department>>(), It.IsAny<string>()))
                              .Returns((IEnumerable<Department> source, string fields) => 
                                  source.Select(d => new Entity()).ToList());

            var query = new PagedDepartmentsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Search = new Search { Value = "" }
            };

            var result = await _repository.PagedDepartmentReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(_testDepartments.Count, result.recordsCount.RecordsTotal);
            Assert.Equal(_testDepartments.Count, result.recordsCount.RecordsFiltered);
        }

        [Fact]
        public async Task PagedDepartmentReponseAsync_WithSearchValue_ShouldReturnFilteredResults()
        {
            _mockDataShapeHelper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<Department>>(), It.IsAny<string>()))
                              .Returns((IEnumerable<Department> source, string fields) => 
                                  source.Select(d => new Entity()).ToList());

            var query = new PagedDepartmentsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Search = new Search { Value = "Human" }
            };

            var result = await _repository.PagedDepartmentReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(_testDepartments.Count, result.recordsCount.RecordsTotal);
            Assert.Equal(1, result.recordsCount.RecordsFiltered);
        }

        [Fact]
        public async Task PagedDepartmentReponseAsync_WithPagination_ShouldReturnCorrectPage()
        {
            _mockDataShapeHelper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<Department>>(), It.IsAny<string>()))
                              .Returns((IEnumerable<Department> source, string fields) => 
                                  source.Select(d => new Entity()).ToList());

            var query = new PagedDepartmentsQuery
            {
                PageNumber = 2,
                PageSize = 2,
                Search = new Search { Value = "" }
            };

            var result = await _repository.PagedDepartmentReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(_testDepartments.Count, result.recordsCount.RecordsTotal);
            Assert.Equal(2, result.data.Count());
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}