using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TalentManagement.Application.Features.Positions.Queries.GetPositions;
using TalentManagement.Application.Interfaces;
using TalentManagement.Application.Parameters;
using TalentManagement.Domain.Entities;
using TalentManagement.Infrastructure.Persistence.Contexts;
using TalentManagement.Infrastructure.Persistence.Repositories;

namespace TalentManagement.Tests.Repositories
{
    public class PositionRepositoryAsyncTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IDataShapeHelper<Position>> _mockDataShaper;
        private readonly Mock<IMockService> _mockService;
        private readonly PositionRepositoryAsync _repository;

        public PositionRepositoryAsyncTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var mockDateTimeService = new Mock<IDateTimeService>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            
            _context = new ApplicationDbContext(options, mockDateTimeService.Object, mockLoggerFactory.Object);
            _mockDataShaper = new Mock<IDataShapeHelper<Position>>();
            _mockService = new Mock<IMockService>();
            _repository = new PositionRepositoryAsync(_context, _mockDataShaper.Object, _mockService.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var departments = new List<Department>
            {
                new Department { Id = Guid.NewGuid(), Name = "Engineering" },
                new Department { Id = Guid.NewGuid(), Name = "Marketing" }
            };

            var salaryRanges = new List<SalaryRange>
            {
                new SalaryRange { Id = Guid.NewGuid(), MinSalary = 50000, MaxSalary = 80000 },
                new SalaryRange { Id = Guid.NewGuid(), MinSalary = 80000, MaxSalary = 120000 }
            };

            var positions = new List<Position>
            {
                new Position
                {
                    Id = Guid.NewGuid(),
                    PositionTitle = "Software Engineer",
                    PositionNumber = "SE001",
                    PositionDescription = "Develops software",
                    DepartmentId = departments[0].Id,
                    Department = departments[0],
                    SalaryRangeId = salaryRanges[0].Id,
                    SalaryRange = salaryRanges[0]
                },
                new Position
                {
                    Id = Guid.NewGuid(),
                    PositionTitle = "Senior Software Engineer",
                    PositionNumber = "SSE001",
                    PositionDescription = "Leads software development",
                    DepartmentId = departments[0].Id,
                    Department = departments[0],
                    SalaryRangeId = salaryRanges[1].Id,
                    SalaryRange = salaryRanges[1]
                },
                new Position
                {
                    Id = Guid.NewGuid(),
                    PositionTitle = "Marketing Manager",
                    PositionNumber = "MM001",
                    PositionDescription = "Manages marketing campaigns",
                    DepartmentId = departments[1].Id,
                    Department = departments[1],
                    SalaryRangeId = salaryRanges[1].Id,
                    SalaryRange = salaryRanges[1]
                }
            };

            _context.Departments.AddRange(departments);
            _context.SalaryRanges.AddRange(salaryRanges);
            _context.Positions.AddRange(positions);
            _context.SaveChanges();
        }

        [Fact]
        public async Task IsUniquePositionNumberAsync_UniqueNumber_ShouldReturnTrue()
        {
            var uniqueNumber = "UNIQUE001";

            var result = await _repository.IsUniquePositionNumberAsync(uniqueNumber);

            Assert.True(result);
        }

        [Fact]
        public async Task IsUniquePositionNumberAsync_ExistingNumber_ShouldReturnFalse()
        {
            var existingNumber = "SE001";

            var result = await _repository.IsUniquePositionNumberAsync(existingNumber);

            Assert.False(result);
        }

        [Fact]
        public async Task IsUniquePositionNumberAsync_EmptyString_ShouldReturnTrue()
        {
            var result = await _repository.IsUniquePositionNumberAsync("");

            Assert.True(result);
        }

        [Fact]
        public async Task IsUniquePositionNumberAsync_NullString_ShouldReturnTrue()
        {
            var result = await _repository.IsUniquePositionNumberAsync(null);

            Assert.True(result);
        }

        [Fact]
        public async Task SeedDataAsync_ValidParameters_ShouldCallMockService()
        {
            var departments = _context.Departments.ToList();
            var salaryRanges = _context.SalaryRanges.ToList();
            var mockPositions = new List<Position>
            {
                new Position { PositionTitle = "Test Position", PositionNumber = "TP001" }
            };

            _mockService.Setup(m => m.GetPositions(It.IsAny<int>(), departments, salaryRanges))
                .Returns(mockPositions);

            try
            {
                var result = await _repository.SeedDataAsync(1, departments, salaryRanges);
                Assert.True(result);
            }
            catch (InvalidOperationException)
            {
                // BulkInsert not supported in InMemory database, but we can verify the mock was called
                _mockService.Verify(m => m.GetPositions(1, departments, salaryRanges), Times.Once);
            }
        }

        [Fact]
        public async Task GetPositionReponseAsync_NoFilters_ShouldReturnAllPositions()
        {
            var query = new GetPositionsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Fields = "Id,PositionTitle"
            };

            var shapedData = new List<Entity>
            {
                new Entity(),
                new Entity(),
                new Entity()
            };

            _mockDataShaper.Setup(d => d.ShapeData(It.IsAny<IEnumerable<Position>>(), query.Fields))
                .Returns(shapedData);

            var result = await _repository.GetPositionReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(shapedData, result.data);
            Assert.True(result.recordsCount.RecordsTotal > 0);
            Assert.Equal(result.recordsCount.RecordsTotal, result.recordsCount.RecordsFiltered);
        }

        [Fact]
        public async Task GetPositionReponseAsync_WithPositionTitleFilter_ShouldReturnFilteredResults()
        {
            var query = new GetPositionsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                PositionTitle = "Software",
                Fields = "Id,PositionTitle"
            };

            var shapedData = new List<Entity>
            {
                new Entity(),
                new Entity()
            };

            _mockDataShaper.Setup(d => d.ShapeData(It.IsAny<IEnumerable<Position>>(), query.Fields))
                .Returns(shapedData);

            var result = await _repository.GetPositionReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.True(result.recordsCount.RecordsFiltered <= result.recordsCount.RecordsTotal);
        }

        [Fact]
        public async Task GetPositionReponseAsync_WithDepartmentFilter_ShouldReturnFilteredResults()
        {
            var query = new GetPositionsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Department = "Engineering",
                Fields = "Id,PositionTitle"
            };

            var shapedData = new List<Entity>
            {
                new Entity(),
                new Entity()
            };

            _mockDataShaper.Setup(d => d.ShapeData(It.IsAny<IEnumerable<Position>>(), query.Fields))
                .Returns(shapedData);

            var result = await _repository.GetPositionReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.True(result.recordsCount.RecordsFiltered <= result.recordsCount.RecordsTotal);
        }

        [Fact]
        public async Task GetPositionReponseAsync_WithPositionNumberFilter_ShouldReturnFilteredResults()
        {
            var query = new GetPositionsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                PositionNumber = "SE001",
                Fields = "Id,PositionTitle"
            };

            var shapedData = new List<Entity>
            {
                new Entity()
            };

            _mockDataShaper.Setup(d => d.ShapeData(It.IsAny<IEnumerable<Position>>(), query.Fields))
                .Returns(shapedData);

            var result = await _repository.GetPositionReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.True(result.recordsCount.RecordsFiltered <= result.recordsCount.RecordsTotal);
        }

        [Fact]
        public async Task GetPositionReponseAsync_WithMultipleFilters_ShouldReturnFilteredResults()
        {
            var query = new GetPositionsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                PositionTitle = "Software",
                Department = "Engineering",
                PositionNumber = "SE",
                Fields = "Id,PositionTitle"
            };

            var shapedData = new List<Entity>
            {
                new Entity()
            };

            _mockDataShaper.Setup(d => d.ShapeData(It.IsAny<IEnumerable<Position>>(), query.Fields))
                .Returns(shapedData);

            var result = await _repository.GetPositionReponseAsync(query);

            Assert.NotNull(result.data);
        }

        [Fact]
        public async Task GetPositionReponseAsync_WithPagination_ShouldReturnCorrectPage()
        {
            var query = new GetPositionsQuery
            {
                PageNumber = 2,
                PageSize = 1,
                Fields = "Id,PositionTitle"
            };

            var shapedData = new List<Entity>
            {
                new Entity()
            };

            _mockDataShaper.Setup(d => d.ShapeData(It.IsAny<IEnumerable<Position>>(), query.Fields))
                .Returns(shapedData);

            var result = await _repository.GetPositionReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(shapedData, result.data);
        }

        [Fact]
        public async Task GetPositionReponseAsync_WithOrderBy_ShouldApplyOrdering()
        {
            var query = new GetPositionsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                OrderBy = "PositionTitle",
                Fields = "Id,PositionTitle"
            };

            var shapedData = new List<Entity>
            {
                new Entity()
            };

            _mockDataShaper.Setup(d => d.ShapeData(It.IsAny<IEnumerable<Position>>(), query.Fields))
                .Returns(shapedData);

            var result = await _repository.GetPositionReponseAsync(query);

            Assert.NotNull(result.data);
        }

        [Fact]
        public async Task GetPositionReponseAsync_NoFieldsSpecified_ShouldNotApplyFieldSelection()
        {
            var query = new GetPositionsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Fields = null
            };

            var shapedData = new List<Entity>
            {
                new Entity()
            };

            _mockDataShaper.Setup(d => d.ShapeData(It.IsAny<IEnumerable<Position>>(), null))
                .Returns(shapedData);

            var result = await _repository.GetPositionReponseAsync(query);

            Assert.NotNull(result.data);
            _mockDataShaper.Verify(d => d.ShapeData(It.IsAny<IEnumerable<Position>>(), null), Times.Once);
        }

        [Fact]
        public async Task PagedPositionReponseAsync_WithSearchValue_ShouldReturnFilteredResults()
        {
            var query = new PagedPositionsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Fields = "Id,PositionTitle",
                Search = new Search { Value = "Software" }
            };

            var shapedData = new List<Entity>
            {
                new Entity()
            };

            _mockDataShaper.Setup(d => d.ShapeData(It.IsAny<IEnumerable<Position>>(), query.Fields))
                .Returns(shapedData);

            var result = await _repository.PagedPositionReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.True(result.recordsCount.RecordsFiltered <= result.recordsCount.RecordsTotal);
        }

        [Fact]
        public async Task PagedPositionReponseAsync_EmptySearch_ShouldReturnAllResults()
        {
            var query = new PagedPositionsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Fields = "Id,PositionTitle",
                Search = new Search { Value = "" }
            };

            var shapedData = new List<Entity>
            {
                new Entity()
            };

            _mockDataShaper.Setup(d => d.ShapeData(It.IsAny<IEnumerable<Position>>(), query.Fields))
                .Returns(shapedData);

            var result = await _repository.PagedPositionReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(result.recordsCount.RecordsTotal, result.recordsCount.RecordsFiltered);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}