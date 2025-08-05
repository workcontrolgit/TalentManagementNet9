using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TalentManagement.Application.Features.SalaryRanges.Queries.GetSalaryRanges;
using TalentManagement.Application.Interfaces;
using TalentManagement.Application.Parameters;
using TalentManagement.Domain.Entities;
using TalentManagement.Infrastructure.Persistence.Contexts;
using TalentManagement.Infrastructure.Persistence.Repositories;
using Entity = TalentManagement.Domain.Entities.Entity;

namespace TalentManagement.Tests.Repositories
{
    public class SalaryRangeRepositoryAsyncTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IDataShapeHelper<SalaryRange>> _mockDataShapeHelper;
        private readonly SalaryRangeRepositoryAsync _repository;
        private readonly List<SalaryRange> _testSalaryRanges;

        public SalaryRangeRepositoryAsyncTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var mockDateTimeService = new Mock<IDateTimeService>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();

            _context = new ApplicationDbContext(options, mockDateTimeService.Object, mockLoggerFactory.Object);
            _mockDataShapeHelper = new Mock<IDataShapeHelper<SalaryRange>>();
            _repository = new SalaryRangeRepositoryAsync(_context, _mockDataShapeHelper.Object);

            _testSalaryRanges = new List<SalaryRange>();

            SeedTestData();
        }

        private void SeedTestData()
        {
            var salaryRanges = new List<SalaryRange>
            {
                new SalaryRange { Id = Guid.NewGuid(), Name = "Entry Level", MinSalary = 40000m, MaxSalary = 60000m },
                new SalaryRange { Id = Guid.NewGuid(), Name = "Junior Level", MinSalary = 50000m, MaxSalary = 70000m },
                new SalaryRange { Id = Guid.NewGuid(), Name = "Mid Level", MinSalary = 65000m, MaxSalary = 85000m },
                new SalaryRange { Id = Guid.NewGuid(), Name = "Senior Level", MinSalary = 80000m, MaxSalary = 120000m },
                new SalaryRange { Id = Guid.NewGuid(), Name = "Executive Level", MinSalary = 150000m, MaxSalary = 300000m }
            };

            _context.SalaryRanges.AddRange(salaryRanges);
            _context.SaveChanges();

            _testSalaryRanges.AddRange(salaryRanges);
        }

        [Fact]
        public async Task AddAsync_ShouldAddSalaryRangeToDatabase()
        {
            var salaryRangeId = Guid.NewGuid();
            var newSalaryRange = new SalaryRange
            {
                Id = salaryRangeId,
                Name = "Test Range",
                MinSalary = 55000m,
                MaxSalary = 75000m
            };

            var result = await _repository.AddAsync(newSalaryRange);

            Assert.NotNull(result);
            Assert.Equal(salaryRangeId, newSalaryRange.Id);

            var savedSalaryRange = await _context.SalaryRanges.FindAsync(salaryRangeId);
            Assert.NotNull(savedSalaryRange);
            Assert.Equal("Test Range", savedSalaryRange.Name);
            Assert.Equal(55000m, savedSalaryRange.MinSalary);
            Assert.Equal(75000m, savedSalaryRange.MaxSalary);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnSalaryRange()
        {
            var salaryRangeId = _testSalaryRanges[0].Id;

            var result = await _repository.GetByIdAsync(salaryRangeId);

            Assert.NotNull(result);
            Assert.Equal(salaryRangeId, result.Id);
            Assert.Equal(_testSalaryRanges[0].Name, result.Name);
            Assert.Equal(_testSalaryRanges[0].MinSalary, result.MinSalary);
            Assert.Equal(_testSalaryRanges[0].MaxSalary, result.MaxSalary);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            var invalidId = Guid.NewGuid();

            var result = await _repository.GetByIdAsync(invalidId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllSalaryRanges()
        {
            var result = await _repository.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(_testSalaryRanges.Count, result.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateSalaryRangeInDatabase()
        {
            var salaryRangeToUpdate = _testSalaryRanges[0];
            salaryRangeToUpdate.Name = "Updated Range Name";
            salaryRangeToUpdate.MinSalary = 45000m;
            salaryRangeToUpdate.MaxSalary = 65000m;

            await _repository.UpdateAsync(salaryRangeToUpdate);

            var updatedSalaryRange = await _context.SalaryRanges.FindAsync(salaryRangeToUpdate.Id);
            Assert.NotNull(updatedSalaryRange);
            Assert.Equal("Updated Range Name", updatedSalaryRange.Name);
            Assert.Equal(45000m, updatedSalaryRange.MinSalary);
            Assert.Equal(65000m, updatedSalaryRange.MaxSalary);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveSalaryRangeFromDatabase()
        {
            var salaryRangeToDelete = _testSalaryRanges[0];

            await _repository.DeleteAsync(salaryRangeToDelete);

            var deletedSalaryRange = await _context.SalaryRanges.FindAsync(salaryRangeToDelete.Id);
            Assert.Null(deletedSalaryRange);
        }

        [Fact]
        public async Task GetSalaryRangeReponseAsync_WithNoFilters_ShouldReturnAllSalaryRanges()
        {
            _mockDataShapeHelper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<SalaryRange>>(), It.IsAny<string>()))
                              .Returns((IEnumerable<SalaryRange> source, string fields) => 
                                  source.Select(s => new Entity()).ToList());

            var query = new GetSalaryRangesQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            var result = await _repository.GetSalaryRangeReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(_testSalaryRanges.Count, result.recordsCount.RecordsTotal);
            Assert.Equal(_testSalaryRanges.Count, result.recordsCount.RecordsFiltered);
        }

        [Fact]
        public async Task GetSalaryRangeReponseAsync_WithNameFilter_ShouldReturnFilteredSalaryRanges()
        {
            _mockDataShapeHelper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<SalaryRange>>(), It.IsAny<string>()))
                              .Returns((IEnumerable<SalaryRange> source, string fields) => 
                                  source.Select(s => new Entity()).ToList());

            var query = new GetSalaryRangesQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Name = "Senior"
            };

            var result = await _repository.GetSalaryRangeReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(_testSalaryRanges.Count, result.recordsCount.RecordsTotal);
            Assert.Equal(1, result.recordsCount.RecordsFiltered);
        }

        [Fact]
        public async Task GetSalaryRangeReponseAsync_WithPaging_ShouldReturnCorrectPage()
        {
            _mockDataShapeHelper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<SalaryRange>>(), It.IsAny<string>()))
                              .Returns((IEnumerable<SalaryRange> source, string fields) => 
                                  source.Select(s => new Entity()).ToList());

            var query = new GetSalaryRangesQuery
            {
                PageNumber = 2,
                PageSize = 2
            };

            var result = await _repository.GetSalaryRangeReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(_testSalaryRanges.Count, result.recordsCount.RecordsTotal);
            Assert.Equal(_testSalaryRanges.Count, result.recordsCount.RecordsFiltered);
            Assert.Equal(2, result.data.Count());
        }

        [Fact]
        public async Task GetSalaryRangeReponseAsync_WithOrderBy_ShouldReturnOrderedResults()
        {
            _mockDataShapeHelper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<SalaryRange>>(), It.IsAny<string>()))
                              .Returns((IEnumerable<SalaryRange> source, string fields) => 
                                  source.Select(s => new Entity()).ToList());

            var query = new GetSalaryRangesQuery
            {
                PageNumber = 1,
                PageSize = 10,
                OrderBy = "Name desc"
            };

            var result = await _repository.GetSalaryRangeReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(_testSalaryRanges.Count, result.recordsCount.RecordsTotal);
        }

        [Fact]
        public async Task GetSalaryRangeReponseAsync_WithFields_ShouldApplyFieldSelection()
        {
            _mockDataShapeHelper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<SalaryRange>>(), It.IsAny<string>()))
                              .Returns((IEnumerable<SalaryRange> source, string fields) => 
                                  source.Select(s => new Entity()).ToList());

            var query = new GetSalaryRangesQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Fields = "Id,Name,MinSalary,MaxSalary"
            };

            var result = await _repository.GetSalaryRangeReponseAsync(query);

            Assert.NotNull(result.data);
            _mockDataShapeHelper.Verify(x => x.ShapeData(It.IsAny<IEnumerable<SalaryRange>>(), "Id,Name,MinSalary,MaxSalary"), Times.Once);
        }

        [Fact]
        public async Task GetSalaryRangeReponseAsync_WithEmptyNameFilter_ShouldReturnAllSalaryRanges()
        {
            _mockDataShapeHelper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<SalaryRange>>(), It.IsAny<string>()))
                              .Returns((IEnumerable<SalaryRange> source, string fields) => 
                                  source.Select(s => new Entity()).ToList());

            var query = new GetSalaryRangesQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Name = ""
            };

            var result = await _repository.GetSalaryRangeReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(_testSalaryRanges.Count, result.recordsCount.RecordsTotal);
            Assert.Equal(_testSalaryRanges.Count, result.recordsCount.RecordsFiltered);
        }

        [Fact]
        public async Task PagedSalaryRangeReponseAsync_WithEmptySearch_ShouldReturnAllResults()
        {
            _mockDataShapeHelper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<SalaryRange>>(), It.IsAny<string>()))
                              .Returns((IEnumerable<SalaryRange> source, string fields) => 
                                  source.Select(s => new Entity()).ToList());

            var query = new PagedSalaryRangesQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Search = new Search { Value = "" }
            };

            var result = await _repository.PagedSalaryRangeReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(_testSalaryRanges.Count, result.recordsCount.RecordsTotal);
            Assert.Equal(_testSalaryRanges.Count, result.recordsCount.RecordsFiltered);
        }

        [Fact]
        public async Task PagedSalaryRangeReponseAsync_WithSearchValue_ShouldReturnFilteredResults()
        {
            _mockDataShapeHelper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<SalaryRange>>(), It.IsAny<string>()))
                              .Returns((IEnumerable<SalaryRange> source, string fields) => 
                                  source.Select(s => new Entity()).ToList());

            var query = new PagedSalaryRangesQuery
            {
                PageNumber = 1,
                PageSize = 10,
                Search = new Search { Value = "Level" }
            };

            var result = await _repository.PagedSalaryRangeReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(_testSalaryRanges.Count, result.recordsCount.RecordsTotal);
            Assert.Equal(5, result.recordsCount.RecordsFiltered); // All test ranges contain "Level"
        }

        [Fact]
        public async Task PagedSalaryRangeReponseAsync_WithPagination_ShouldReturnCorrectPage()
        {
            _mockDataShapeHelper.Setup(x => x.ShapeData(It.IsAny<IEnumerable<SalaryRange>>(), It.IsAny<string>()))
                              .Returns((IEnumerable<SalaryRange> source, string fields) => 
                                  source.Select(s => new Entity()).ToList());

            var query = new PagedSalaryRangesQuery
            {
                PageNumber = 2,
                PageSize = 2,
                Search = new Search { Value = "" }
            };

            var result = await _repository.PagedSalaryRangeReponseAsync(query);

            Assert.NotNull(result.data);
            Assert.Equal(_testSalaryRanges.Count, result.recordsCount.RecordsTotal);
            Assert.Equal(2, result.data.Count());
        }

        [Fact]
        public async Task AddAsync_WithZeroSalaries_ShouldAddSuccessfully()
        {
            var salaryRangeId = Guid.NewGuid();
            var newSalaryRange = new SalaryRange
            {
                Id = salaryRangeId,
                Name = "Intern Range",
                MinSalary = 0m,
                MaxSalary = 0m
            };

            var result = await _repository.AddAsync(newSalaryRange);

            Assert.NotNull(result);
            var savedSalaryRange = await _context.SalaryRanges.FindAsync(salaryRangeId);
            Assert.NotNull(savedSalaryRange);
            Assert.Equal(0m, savedSalaryRange.MinSalary);
            Assert.Equal(0m, savedSalaryRange.MaxSalary);
        }

        [Fact]
        public async Task AddAsync_WithHighSalaries_ShouldAddSuccessfully()
        {
            var salaryRangeId = Guid.NewGuid();
            var newSalaryRange = new SalaryRange
            {
                Id = salaryRangeId,
                Name = "CEO Range",
                MinSalary = 500000m,
                MaxSalary = 1000000m
            };

            var result = await _repository.AddAsync(newSalaryRange);

            Assert.NotNull(result);
            var savedSalaryRange = await _context.SalaryRanges.FindAsync(salaryRangeId);
            Assert.NotNull(savedSalaryRange);
            Assert.Equal(500000m, savedSalaryRange.MinSalary);
            Assert.Equal(1000000m, savedSalaryRange.MaxSalary);
        }

        [Fact]
        public async Task UpdateAsync_WithPreciseSalaries_ShouldUpdateSuccessfully()
        {
            var salaryRangeToUpdate = _testSalaryRanges[0];
            salaryRangeToUpdate.MinSalary = 55000.99m;
            salaryRangeToUpdate.MaxSalary = 75000.01m;

            await _repository.UpdateAsync(salaryRangeToUpdate);

            var updatedSalaryRange = await _context.SalaryRanges.FindAsync(salaryRangeToUpdate.Id);
            Assert.NotNull(updatedSalaryRange);
            Assert.Equal(55000.99m, updatedSalaryRange.MinSalary);
            Assert.Equal(75000.01m, updatedSalaryRange.MaxSalary);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}