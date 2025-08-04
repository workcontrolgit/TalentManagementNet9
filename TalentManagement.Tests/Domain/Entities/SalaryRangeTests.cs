using TalentManagement.Domain.Common;
using TalentManagement.Domain.Entities;

namespace TalentManagement.Tests.Domain.Entities
{
    public class SalaryRangeTests
    {
        [Fact]
        public void SalaryRange_Constructor_ShouldInitializePositionsCollection()
        {
            var salaryRange = new SalaryRange();

            Assert.NotNull(salaryRange.Positions);
            Assert.Empty(salaryRange.Positions);
            Assert.IsAssignableFrom<ICollection<Position>>(salaryRange.Positions);
        }

        [Fact]
        public void SalaryRange_InheritsFromAuditableBaseEntity()
        {
            var salaryRange = new SalaryRange();

            Assert.IsAssignableFrom<AuditableBaseEntity>(salaryRange);
            Assert.IsAssignableFrom<BaseEntity>(salaryRange);
        }

        [Fact]
        public void SalaryRange_Properties_ShouldSetAndGetCorrectly()
        {
            var salaryRange = new SalaryRange();
            var testName = "Junior Level";
            var testId = Guid.NewGuid();
            var testMinSalary = 50000m;
            var testMaxSalary = 75000m;

            salaryRange.Id = testId;
            salaryRange.Name = testName;
            salaryRange.MinSalary = testMinSalary;
            salaryRange.MaxSalary = testMaxSalary;

            Assert.Equal(testId, salaryRange.Id);
            Assert.Equal(testName, salaryRange.Name);
            Assert.Equal(testMinSalary, salaryRange.MinSalary);
            Assert.Equal(testMaxSalary, salaryRange.MaxSalary);
        }

        [Fact]
        public void SalaryRange_DecimalProperties_ShouldHandlePrecision()
        {
            var salaryRange = new SalaryRange();
            var preciseMinSalary = 50000.99m;
            var preciseMaxSalary = 75000.01m;

            salaryRange.MinSalary = preciseMinSalary;
            salaryRange.MaxSalary = preciseMaxSalary;

            Assert.Equal(preciseMinSalary, salaryRange.MinSalary);
            Assert.Equal(preciseMaxSalary, salaryRange.MaxSalary);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void SalaryRange_Name_CanBeSetToEmptyOrNull(string name)
        {
            var salaryRange = new SalaryRange();

            salaryRange.Name = name;

            Assert.Equal(name, salaryRange.Name);
        }

        [Fact]
        public void SalaryRange_NavigationProperties_ShouldBeSettable()
        {
            var salaryRange = new SalaryRange();
            var positions = new List<Position>
            {
                new Position { Id = Guid.NewGuid(), PositionTitle = "Developer" },
                new Position { Id = Guid.NewGuid(), PositionTitle = "Senior Developer" }
            };

            salaryRange.Positions = positions;

            Assert.Equal(positions, salaryRange.Positions);
            Assert.Equal(2, salaryRange.Positions.Count);
        }

        [Fact]
        public void SalaryRange_MultiplePositions_CanBeAddedToCollection()
        {
            var salaryRange = new SalaryRange();
            var position1 = new Position { Id = Guid.NewGuid(), PositionTitle = "Junior Developer" };
            var position2 = new Position { Id = Guid.NewGuid(), PositionTitle = "Senior Developer" };

            salaryRange.Positions.Add(position1);
            salaryRange.Positions.Add(position2);

            Assert.Equal(2, salaryRange.Positions.Count);
            Assert.Contains(position1, salaryRange.Positions);
            Assert.Contains(position2, salaryRange.Positions);
        }

        [Fact]
        public void SalaryRange_Name_ShouldAcceptLongStrings()
        {
            var salaryRange = new SalaryRange();
            var longName = new string('A', 250);

            salaryRange.Name = longName;

            Assert.Equal(longName, salaryRange.Name);
        }

        [Fact]
        public void SalaryRange_Id_ShouldAcceptValidGuid()
        {
            var salaryRange = new SalaryRange();
            var validGuid = Guid.NewGuid();

            salaryRange.Id = validGuid;

            Assert.Equal(validGuid, salaryRange.Id);
        }

        [Fact]
        public void SalaryRange_Id_ShouldAcceptEmptyGuid()
        {
            var salaryRange = new SalaryRange();

            salaryRange.Id = Guid.Empty;

            Assert.Equal(Guid.Empty, salaryRange.Id);
        }

        [Fact]
        public void SalaryRange_SalaryValues_ShouldAcceptZero()
        {
            var salaryRange = new SalaryRange();

            salaryRange.MinSalary = 0m;
            salaryRange.MaxSalary = 0m;

            Assert.Equal(0m, salaryRange.MinSalary);
            Assert.Equal(0m, salaryRange.MaxSalary);
        }

        [Fact]
        public void SalaryRange_SalaryValues_ShouldAcceptNegativeValues()
        {
            var salaryRange = new SalaryRange();

            salaryRange.MinSalary = -1000m;
            salaryRange.MaxSalary = -500m;

            Assert.Equal(-1000m, salaryRange.MinSalary);
            Assert.Equal(-500m, salaryRange.MaxSalary);
        }

        [Fact]
        public void SalaryRange_SalaryValues_ShouldAcceptLargeValues()
        {
            var salaryRange = new SalaryRange();
            var largeMinSalary = 999999999.99m;
            var largeMaxSalary = 1000000000.00m;

            salaryRange.MinSalary = largeMinSalary;
            salaryRange.MaxSalary = largeMaxSalary;

            Assert.Equal(largeMinSalary, salaryRange.MinSalary);
            Assert.Equal(largeMaxSalary, salaryRange.MaxSalary);
        }

        [Fact]
        public void SalaryRange_AuditProperties_ShouldBeInherited()
        {
            var salaryRange = new SalaryRange();
            var testDate = DateTime.UtcNow;
            var testUser = "testuser";

            salaryRange.Created = testDate;
            salaryRange.CreatedBy = testUser;
            salaryRange.LastModified = testDate.AddHours(1);
            salaryRange.LastModifiedBy = testUser;

            Assert.Equal(testDate, salaryRange.Created);
            Assert.Equal(testUser, salaryRange.CreatedBy);
            Assert.Equal(testDate.AddHours(1), salaryRange.LastModified);
            Assert.Equal(testUser, salaryRange.LastModifiedBy);
        }

        [Fact]
        public void SalaryRange_DefaultValues_ShouldBeCorrect()
        {
            var salaryRange = new SalaryRange();

            Assert.Equal(Guid.Empty, salaryRange.Id);
            Assert.Null(salaryRange.Name);
            Assert.Equal(0m, salaryRange.MinSalary);
            Assert.Equal(0m, salaryRange.MaxSalary);
            Assert.NotNull(salaryRange.Positions);
            Assert.Empty(salaryRange.Positions);
        }
    }
}