# Talent Management System - Test Documentation

This document provides comprehensive information about the test suite for the Talent Management System, including test structure, coverage, and execution instructions.

## 📋 Table of Contents

- [Overview](#overview)
- [Test Structure](#test-structure)
- [Test Coverage](#test-coverage)
- [Prerequisites](#prerequisites)
- [Running Tests](#running-tests)
- [Test Categories](#test-categories)
- [CI/CD Integration](#cicd-integration)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)

## 🎯 Overview

The test suite provides comprehensive coverage for the Talent Management System, built using Clean Architecture principles. The tests ensure reliability, maintainability, and correctness across all application layers.

**Current Test Statistics:**
- **Total Tests:** 92
- **Pass Rate:** 100%
- **Coverage:** All Position entity functionality
- **Test Framework:** xUnit
- **Mocking Framework:** Moq
- **Database:** Entity Framework In-Memory

## 🏗️ Test Structure

The tests are organized by architectural layers following Clean Architecture principles:

```
TalentManagement.Tests/
├── Domain/
│   └── Entities/
│       └── PositionTests.cs                    # Entity behavior tests
├── Handlers/
│   └── Positions/
│       ├── CreatePositionCommandHandlerTests.cs        # Create command tests
│       ├── CreatePositionCommandValidatorTests.cs      # Validation tests
│       ├── UpdatePositionCommandHandlerTests.cs        # Update command tests
│       ├── DeletePositionCommandHandlerTests.cs        # Delete command tests
│       ├── GetPositionByIdQueryHandlerTests.cs         # Single query tests
│       └── GetPositionsQueryHandlerTests.cs            # List query tests
├── Controllers/
│   └── PositionsControllerTests.cs            # API controller tests
├── Repositories/
│   └── PositionRepositoryAsyncTests.cs        # Data access tests
└── README.md                                  # This documentation
```

## 🎯 Test Coverage

### Domain Layer Tests
- **Entity Validation:** Property setters/getters, navigation properties
- **Business Rules:** Inheritance validation, collection management
- **Constructor Behavior:** Initialization logic

### Application Layer Tests
- **Command Handlers:** CRUD operations (Create, Update, Delete)
- **Query Handlers:** Data retrieval (single item, paginated lists)
- **Validators:** Input validation rules and business constraints
- **Error Handling:** Exception scenarios and edge cases

### Infrastructure Layer Tests
- **Repository Operations:** Data persistence and retrieval
- **Database Interactions:** Filtering, pagination, searching
- **Data Validation:** Unique constraints and business rules

### Presentation Layer Tests
- **Controller Actions:** All REST API endpoints
- **Request/Response Handling:** Data transfer and serialization
- **HTTP Status Codes:** Proper response codes for different scenarios

## ⚙️ Prerequisites

Before running the tests, ensure you have:

1. **.NET 9.0 SDK** or later installed
2. **Visual Studio 2022** or **VS Code** with C# extension
3. **Git** for version control

### Verify Installation
```bash
dotnet --version  # Should show 9.0.x or later
```

## 🚀 Running Tests

### Command Line Options

#### Run All Tests
```bash
# Navigate to the test project directory
cd TalentManagement.Tests

# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run with minimal output
dotnet test --verbosity minimal
```

#### Run Specific Test Categories
```bash
# Run only Position-related tests
dotnet test --filter "FullyQualifiedName~Position"

# Run only Domain layer tests
dotnet test --filter "FullyQualifiedName~Domain"

# Run only Handler tests
dotnet test --filter "FullyQualifiedName~Handlers"

# Run only Controller tests
dotnet test --filter "FullyQualifiedName~Controllers"

# Run only Repository tests
dotnet test --filter "FullyQualifiedName~Repositories"
```

#### Run Specific Test Classes
```bash
# Run specific test class
dotnet test --filter "ClassName=PositionTests"

# Run specific test method
dotnet test --filter "MethodName=Handle_ValidRequest_ShouldReturnSuccessResponse"
```

### Visual Studio

1. **Test Explorer:** 
   - Open `View` → `Test Explorer`
   - Click "Run All Tests" or run individual tests

2. **Solution Explorer:**
   - Right-click on test project → `Run Tests`
   - Right-click on specific test file → `Run Tests`

### VS Code

1. Install the `.NET Core Test Explorer` extension
2. Open Command Palette (`Ctrl+Shift+P`)
3. Run `Test: Run All Tests` or `Test: Run Test at Cursor`

## 📊 Test Categories

### Unit Tests (Fast)
- **Domain Entity Tests:** Pure business logic, no dependencies
- **Handler Tests:** Business operations with mocked dependencies
- **Validator Tests:** Input validation rules

### Integration Tests (Medium)
- **Repository Tests:** Database operations with in-memory database
- **Controller Tests:** API endpoints with mocked services

### Characteristics by Category

| Category | Speed | Isolation | External Dependencies |
|----------|-------|-----------|----------------------|
| Unit | Very Fast | High | None |
| Integration | Medium | Medium | In-Memory Database |

## 🧪 Test Examples

### Running Specific Scenarios

```bash
# Test CRUD operations
dotnet test --filter "FullyQualifiedName~Create OR FullyQualifiedName~Update OR FullyQualifiedName~Delete"

# Test validation scenarios
dotnet test --filter "FullyQualifiedName~Validator"

# Test error handling
dotnet test --filter "MethodName~Exception OR MethodName~Error"

# Test null scenarios  
dotnet test --filter "MethodName~Null"
```

### Performance Testing
```bash
# Run tests with execution time
dotnet test --logger "console;verbosity=detailed"

# Generate test results file
dotnet test --logger "trx;LogFileName=TestResults.trx"
```

## 🔄 CI/CD Integration

### GitHub Actions Example
```yaml
name: Tests
on: [push, pull_request]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'
    - name: Restore dependencies
      run: dotnet restore
    - name: Build
      run: dotnet build --no-restore
    - name: Test
      run: dotnet test --no-build --verbosity normal
```

### Azure DevOps Example
```yaml
- task: DotNetCoreCLI@2
  displayName: 'Run Tests'
  inputs:
    command: 'test'
    projects: '**/*Tests.csproj'
    arguments: '--configuration Release --collect:"XPlat Code Coverage"'
```

## 🔧 Troubleshooting

### Common Issues

#### 1. Tests Fail to Run
```bash
# Clean and rebuild
dotnet clean
dotnet build
dotnet test
```

#### 2. In-Memory Database Issues
```bash
# Ensure EF In-Memory package is installed
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

#### 3. Mock Setup Issues
```bash
# Verify Moq package version
dotnet list package | grep Moq
```

#### 4. Missing Dependencies
```bash
# Restore all packages
dotnet restore
```

### Debugging Tests

#### Visual Studio
1. Set breakpoints in test methods
2. Right-click → `Debug Tests`
3. Use `Test` → `Debug` → `All Tests`

#### Command Line
```bash
# Run with debugging
dotnet test --verbosity diagnostic
```

### Test Output Analysis

#### Failed Test Investigation
```bash
# Run failed tests only
dotnet test --filter "Outcome=Failed"

# Detailed error output
dotnet test --logger "console;verbosity=detailed"
```

## 🤝 Contributing

### Adding New Tests

1. **Follow Naming Conventions:**
   ```csharp
   [Fact]
   public async Task MethodName_Scenario_ExpectedResult()
   ```

2. **Use Arrange-Act-Assert Pattern:**
   ```csharp
   [Fact]
   public async Task Handle_ValidRequest_ShouldReturnSuccess()
   {
       // Arrange
       var command = new CreatePositionCommand { /* setup */ };
       
       // Act
       var result = await _handler.Handle(command, CancellationToken.None);
       
       // Assert
       Assert.True(result.Succeeded);
   }
   ```

3. **Test Categories to Include:**
   - Happy path scenarios
   - Edge cases (null, empty, boundary values)
   - Error conditions
   - Validation scenarios
   - Cancellation token handling

### Code Coverage Goals

- **Domain Layer:** 100% (pure business logic)
- **Application Layer:** 95%+ (critical business operations)
- **Infrastructure Layer:** 80%+ (data access operations)
- **Presentation Layer:** 90%+ (API endpoints)

### Best Practices

1. **Keep tests fast and isolated**
2. **Use descriptive test names**
3. **Mock external dependencies**
4. **Test one thing per test method**
5. **Use appropriate assertions**
6. **Clean up resources properly**

## 📈 Metrics and Reporting

### Generate Test Reports
```bash
# Generate detailed test report
dotnet test --logger "html;LogFileName=TestResults.html"

# Generate code coverage report (requires coverlet)
dotnet test --collect:"XPlat Code Coverage"
```

### Continuous Monitoring
- **Test execution time trends**
- **Pass/fail rates over time** 
- **Code coverage metrics**
- **Test reliability indicators**

---

## 📞 Support

For questions or issues with the test suite:

1. **Check this documentation first**
2. **Review existing test examples**
3. **Create an issue** with detailed error information
4. **Include environment details** (.NET version, OS, etc.)

---

**Last Updated:** January 2025  
**Test Suite Version:** 1.0  
**Framework:** .NET 9.0 / xUnit / Moq