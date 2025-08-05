using TalentManagement.Domain.Common;
using TalentManagement.Domain.Entities;

namespace TalentManagement.Tests.Domain.Entities
{
    public class DepartmentTests
    {
        [Fact]
        public void Department_Constructor_ShouldInitializePositionsCollection()
        {
            var department = new Department();

            Assert.NotNull(department.Positions);
            Assert.Empty(department.Positions);
            Assert.IsAssignableFrom<ICollection<Position>>(department.Positions);
        }

        [Fact]
        public void Department_InheritsFromAuditableBaseEntity()
        {
            var department = new Department();

            Assert.IsAssignableFrom<AuditableBaseEntity>(department);
            Assert.IsAssignableFrom<BaseEntity>(department);
        }

        [Fact]
        public void Department_Properties_ShouldSetAndGetCorrectly()
        {
            var department = new Department();
            var testName = "Human Resources";
            var testId = Guid.NewGuid();

            department.Id = testId;
            department.Name = testName;

            Assert.Equal(testId, department.Id);
            Assert.Equal(testName, department.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Department_Name_CanBeSetToEmptyOrNull(string name)
        {
            var department = new Department();

            department.Name = name;

            Assert.Equal(name, department.Name);
        }

        [Fact]
        public void Department_NavigationProperties_ShouldBeSettable()
        {
            var department = new Department();
            var positions = new List<Position>
            {
                new Position { Id = Guid.NewGuid(), PositionTitle = "Manager" },
                new Position { Id = Guid.NewGuid(), PositionTitle = "Developer" }
            };

            department.Positions = positions;

            Assert.Equal(positions, department.Positions);
            Assert.Equal(2, department.Positions.Count);
        }

        [Fact]
        public void Department_MultiplePositions_CanBeAddedToCollection()
        {
            var department = new Department();
            var position1 = new Position { Id = Guid.NewGuid(), PositionTitle = "Manager" };
            var position2 = new Position { Id = Guid.NewGuid(), PositionTitle = "Developer" };

            department.Positions.Add(position1);
            department.Positions.Add(position2);

            Assert.Equal(2, department.Positions.Count);
            Assert.Contains(position1, department.Positions);
            Assert.Contains(position2, department.Positions);
        }

        [Fact]
        public void Department_Name_ShouldAcceptLongStrings()
        {
            var department = new Department();
            var longName = new string('A', 250); // Test with max length typically allowed

            department.Name = longName;

            Assert.Equal(longName, department.Name);
        }

        [Fact]
        public void Department_Id_ShouldAcceptValidGuid()
        {
            var department = new Department();
            var validGuid = Guid.NewGuid();

            department.Id = validGuid;

            Assert.Equal(validGuid, department.Id);
        }

        [Fact]
        public void Department_Id_ShouldAcceptEmptyGuid()
        {
            var department = new Department();

            department.Id = Guid.Empty;

            Assert.Equal(Guid.Empty, department.Id);
        }

        [Fact]
        public void Department_AuditProperties_ShouldBeInherited()
        {
            var department = new Department();
            var testDate = DateTime.UtcNow;
            var testUser = "testuser";

            department.Created = testDate;
            department.CreatedBy = testUser;
            department.LastModified = testDate.AddHours(1);
            department.LastModifiedBy = testUser;

            Assert.Equal(testDate, department.Created);
            Assert.Equal(testUser, department.CreatedBy);
            Assert.Equal(testDate.AddHours(1), department.LastModified);
            Assert.Equal(testUser, department.LastModifiedBy);
        }
    }
}