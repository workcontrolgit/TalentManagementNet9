using TalentManagement.Domain.Entities;
using TalentManagement.Domain.Common;
using TalentManagement.Domain.Enums;

namespace TalentManagement.Tests.Domain.Entities
{
    public class EmployeeTests
    {
        [Fact]
        public void Employee_Constructor_ShouldInitializeCorrectly()
        {
            var employee = new Employee();

            Assert.NotNull(employee);
            Assert.Equal(Guid.Empty, employee.Id);
        }

        [Fact]
        public void Employee_Properties_ShouldSetAndGetCorrectly()
        {
            var employee = new Employee();
            var positionId = Guid.NewGuid();
            var birthday = DateTime.Now.AddYears(-30);

            employee.FirstName = "John";
            employee.MiddleName = "Michael";
            employee.LastName = "Doe";
            employee.PositionId = positionId;
            employee.Salary = 75000.50m;
            employee.Birthday = birthday;
            employee.Email = "john.doe@company.com";
            employee.Gender = Gender.Male;
            employee.EmployeeNumber = "EMP001";
            employee.Prefix = "Mr.";
            employee.Phone = "+1-555-123-4567";

            Assert.Equal("John", employee.FirstName);
            Assert.Equal("Michael", employee.MiddleName);
            Assert.Equal("Doe", employee.LastName);
            Assert.Equal(positionId, employee.PositionId);
            Assert.Equal(75000.50m, employee.Salary);
            Assert.Equal(birthday, employee.Birthday);
            Assert.Equal("john.doe@company.com", employee.Email);
            Assert.Equal(Gender.Male, employee.Gender);
            Assert.Equal("EMP001", employee.EmployeeNumber);
            Assert.Equal("Mr.", employee.Prefix);
            Assert.Equal("+1-555-123-4567", employee.Phone);
        }

        [Fact]
        public void Employee_NavigationProperties_ShouldBeSettable()
        {
            var employee = new Employee();
            var position = new Position 
            { 
                PositionTitle = "Software Engineer",
                PositionNumber = "SE001"
            };

            employee.Position = position;

            Assert.Equal(position, employee.Position);
            Assert.Equal("Software Engineer", employee.Position.PositionTitle);
        }

        [Fact]
        public void Employee_InheritsFromAuditableBaseEntity()
        {
            var employee = new Employee();

            Assert.IsAssignableFrom<AuditableBaseEntity>(employee);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Employee_FirstName_CanBeSetToEmptyOrNull(string firstName)
        {
            var employee = new Employee { FirstName = firstName };

            Assert.Equal(firstName, employee.FirstName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Employee_LastName_CanBeSetToEmptyOrNull(string lastName)
        {
            var employee = new Employee { LastName = lastName };

            Assert.Equal(lastName, employee.LastName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Employee_MiddleName_CanBeSetToEmptyOrNull(string middleName)
        {
            var employee = new Employee { MiddleName = middleName };

            Assert.Equal(middleName, employee.MiddleName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Employee_Email_CanBeSetToEmptyOrNull(string email)
        {
            var employee = new Employee { Email = email };

            Assert.Equal(email, employee.Email);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Employee_EmployeeNumber_CanBeSetToEmptyOrNull(string employeeNumber)
        {
            var employee = new Employee { EmployeeNumber = employeeNumber };

            Assert.Equal(employeeNumber, employee.EmployeeNumber);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Employee_Phone_CanBeSetToEmptyOrNull(string phone)
        {
            var employee = new Employee { Phone = phone };

            Assert.Equal(phone, employee.Phone);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Employee_Prefix_CanBeSetToEmptyOrNull(string prefix)
        {
            var employee = new Employee { Prefix = prefix };

            Assert.Equal(prefix, employee.Prefix);
        }

        [Fact]
        public void Employee_Salary_ShouldAcceptDecimalValues()
        {
            var employee = new Employee();
            var salary = 125750.75m;

            employee.Salary = salary;

            Assert.Equal(salary, employee.Salary);
        }

        [Fact]
        public void Employee_Salary_ShouldAcceptZero()
        {
            var employee = new Employee();

            employee.Salary = 0m;

            Assert.Equal(0m, employee.Salary);
        }

        [Theory]
        [InlineData(Gender.Male)]
        [InlineData(Gender.Female)]
        public void Employee_Gender_ShouldAcceptValidGenderValues(Gender gender)
        {
            var employee = new Employee { Gender = gender };

            Assert.Equal(gender, employee.Gender);
        }

        [Fact]
        public void Employee_Birthday_ShouldAcceptValidDate()
        {
            var employee = new Employee();
            var birthday = new DateTime(1990, 5, 15);

            employee.Birthday = birthday;

            Assert.Equal(birthday, employee.Birthday);
        }

        [Fact]
        public void Employee_PositionId_ShouldAcceptValidGuid()
        {
            var employee = new Employee();
            var positionId = Guid.NewGuid();

            employee.PositionId = positionId;

            Assert.Equal(positionId, employee.PositionId);
        }

        [Fact]
        public void Employee_PositionId_ShouldAcceptEmptyGuid()
        {
            var employee = new Employee();

            employee.PositionId = Guid.Empty;

            Assert.Equal(Guid.Empty, employee.PositionId);
        }
    }
}