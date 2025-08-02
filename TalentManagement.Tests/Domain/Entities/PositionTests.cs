using TalentManagement.Domain.Entities;
using TalentManagement.Domain.Common;

namespace TalentManagement.Tests.Domain.Entities
{
    public class PositionTests
    {
        [Fact]
        public void Position_Constructor_ShouldInitializeEmployeesCollection()
        {
            var position = new Position();

            Assert.NotNull(position.Employees);
            Assert.Empty(position.Employees);
            Assert.IsType<HashSet<Employee>>(position.Employees);
        }

        [Fact]
        public void Position_Properties_ShouldSetAndGetCorrectly()
        {
            var position = new Position();
            var departmentId = Guid.NewGuid();
            var salaryRangeId = Guid.NewGuid();

            position.PositionTitle = "Software Engineer";
            position.PositionNumber = "SE001";
            position.PositionDescription = "Develops software applications";
            position.DepartmentId = departmentId;
            position.SalaryRangeId = salaryRangeId;

            Assert.Equal("Software Engineer", position.PositionTitle);
            Assert.Equal("SE001", position.PositionNumber);
            Assert.Equal("Develops software applications", position.PositionDescription);
            Assert.Equal(departmentId, position.DepartmentId);
            Assert.Equal(salaryRangeId, position.SalaryRangeId);
        }

        [Fact]
        public void Position_NavigationProperties_ShouldBeSettable()
        {
            var position = new Position();
            var department = new Department { Name = "IT" };
            var salaryRange = new SalaryRange { MinSalary = 50000, MaxSalary = 80000 };
            var employee = new Employee { FirstName = "John", LastName = "Doe" };

            position.Department = department;
            position.SalaryRange = salaryRange;
            position.Employees.Add(employee);

            Assert.Equal(department, position.Department);
            Assert.Equal(salaryRange, position.SalaryRange);
            Assert.Contains(employee, position.Employees);
            Assert.Single(position.Employees);
        }

        [Fact]
        public void Position_InheritsFromAuditableBaseEntity()
        {
            var position = new Position();

            Assert.IsAssignableFrom<AuditableBaseEntity>(position);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Position_PositionTitle_CanBeSetToEmptyOrNull(string title)
        {
            var position = new Position { PositionTitle = title };

            Assert.Equal(title, position.PositionTitle);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Position_PositionNumber_CanBeSetToEmptyOrNull(string number)
        {
            var position = new Position { PositionNumber = number };

            Assert.Equal(number, position.PositionNumber);
        }

        [Fact]
        public void Position_MultipleEmployees_CanBeAddedToCollection()
        {
            var position = new Position();
            var employee1 = new Employee { FirstName = "John", LastName = "Doe" };
            var employee2 = new Employee { FirstName = "Jane", LastName = "Smith" };

            position.Employees.Add(employee1);
            position.Employees.Add(employee2);

            Assert.Equal(2, position.Employees.Count);
            Assert.Contains(employee1, position.Employees);
            Assert.Contains(employee2, position.Employees);
        }
    }
}