# WebAPI Design - Clean Architecture with CQRS

This document explains the architectural patterns, validation strategies, and testing approaches used in the TalentManagement Web API.

## Table of Contents
- [Clean Architecture Overview](#clean-architecture-overview)
- [CQRS Pattern Implementation](#cqrs-pattern-implementation)
- [Fluent Validation](#fluent-validation)
- [Testing Strategy](#testing-strategy)
- [Project Structure](#project-structure)

## Clean Architecture Overview

The TalentManagement API follows Clean Architecture principles with clear separation of concerns across multiple layers:

### Layer Structure

```
TalentManagement/
├── TalentManagement.Domain/           # Core business entities and enums
├── TalentManagement.Application/      # Business logic and use cases
├── TalentManagement.Infrastructure.Persistence/  # Data access layer
├── TalentManagement.Infrastructure.Shared/      # Cross-cutting concerns
├── TalentManagement.WebApi/          # API controllers and presentation
└── TalentManagement.Tests/           # Unit and integration tests
```

### Key Benefits
- **Dependency Inversion**: Inner layers don't depend on outer layers
- **Testability**: Business logic is isolated and easily testable
- **Flexibility**: Infrastructure can be swapped without affecting core business logic
- **Maintainability**: Clear boundaries between layers reduce coupling

## CQRS Pattern Implementation

The application implements Command Query Responsibility Segregation (CQRS) using MediatR.

### Commands (Write Operations)
Commands handle write operations and return responses wrapped in `Response<T>`:

```csharp
// Example: CreateEmployeeCommand.cs
public partial class CreateEmployeeCommand : IRequest<Response<Guid>>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    // ... other properties
}

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Response<Guid>>
{
    private readonly IEmployeeRepositoryAsync _repository;
    private readonly IMapper _mapper;

    public async Task<Response<Guid>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = _mapper.Map<Employee>(request);
        await _repository.AddAsync(employee);
        return new Response<Guid>(employee.Id);
    }
}
```

### Queries (Read Operations)
Queries handle read operations and return paginated responses:

```csharp
// Example: GetEmployeesQuery.cs
public class GetEmployeesQuery : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    // ... filter properties
}

public class GetAllEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, PagedResponse<IEnumerable<Entity>>>
{
    public async Task<PagedResponse<IEnumerable<Entity>>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        // Implementation with filtering, pagination, and field selection
    }
}
```

### MediatR Pipeline Configuration

The application layer configures MediatR with validation behavior:

```csharp
// ServiceExtensions.cs
public static void AddApplicationLayer(this IServiceCollection services)
{
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
}
```

## Fluent Validation

The API uses FluentValidation for comprehensive input validation with custom validation behaviors.

### Validation Pipeline
All commands pass through a `ValidationBehavior<TRequest, TResponse>` that:
1. Collects all validators for the request type
2. Executes validation asynchronously
3. Throws `ValidationException` if validation fails
4. Continues to the handler if validation passes

### Example Validator

```csharp
// CreateEmployeeCommandValidator.cs
public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    private readonly IEmployeeRepositoryAsync _employeeRepository;

    public CreateEmployeeCommandValidator(IEmployeeRepositoryAsync employeeRepository)
    {
        _employeeRepository = employeeRepository;

        RuleFor(p => p.FirstName)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .EmailAddress().WithMessage("{PropertyName} must be a valid email address.")
            .MaximumLength(100).WithMessage("{PropertyName} must not exceed 100 characters.");

        RuleFor(p => p.EmployeeNumber)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .MaximumLength(20).WithMessage("{PropertyName} must not exceed 20 characters.")
            .MustAsync(IsUniqueEmployeeNumber).WithMessage("{PropertyName} already exists.");

        RuleFor(p => p.Salary)
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

        RuleFor(p => p.Birthday)
            .LessThan(DateTime.Today).WithMessage("{PropertyName} must be in the past.");
    }

    private async Task<bool> IsUniqueEmployeeNumber(string employeeNumber, CancellationToken token)
    {
        return await _employeeRepository.IsUniqueEmployeeNumberAsync(employeeNumber);
    }
}
```

### Validation Features
- **Property-level validation**: Length, format, and business rule validation
- **Async validation**: Database uniqueness checks
- **Custom messages**: User-friendly error messages with property name placeholders
- **Conditional validation**: Rules that apply based on other property values
- **Cross-field validation**: Validation that compares multiple properties

### Error Handling
Validation errors are automatically caught and converted to HTTP 400 responses through the global error handling middleware.

## Testing Strategy

The project follows a comprehensive testing strategy covering multiple layers:

### Test Organization

```
TalentManagement.Tests/
├── Controllers/          # API controller tests
├── Handlers/            # Command/Query handler tests
│   ├── Employees/       # Employee-specific handler tests
│   ├── Departments/     # Department-specific handler tests
│   └── ...
├── Domain/              # Domain entity tests
├── Repositories/        # Repository tests
└── README.md
```

### Unit Testing Patterns

#### 1. Handler Tests
Handler tests use mocked dependencies to test business logic in isolation:

```csharp
public class CreateEmployeeCommandHandlerTests
{
    private readonly Mock<IEmployeeRepositoryAsync> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CreateEmployeeCommandHandler _handler;

    [Fact]
    public async Task Handle_ValidRequest_ShouldReturnSuccessResponse()
    {
        // Arrange
        var command = new CreateEmployeeCommand { /* properties */ };
        var employee = new Employee { /* properties */ };
        
        _mockMapper.Setup(m => m.Map<Employee>(command)).Returns(employee);
        _mockRepository.Setup(r => r.AddAsync(employee)).Returns(Task.FromResult(employee));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(employee.Id, result.Data);
        _mockRepository.Verify(r => r.AddAsync(employee), Times.Once);
    }
}
```

#### 2. Controller Tests
Controller tests verify HTTP-specific behavior and proper integration with MediatR:

```csharp
public class EmployeesControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly EmployeesController _controller;

    [Fact]
    public async Task Post_WithValidCommand_ShouldReturnCreatedResult()
    {
        // Arrange
        var command = new CreateEmployeeCommand { /* properties */ };
        var expectedResponse = new Response<Guid>(Guid.NewGuid());
        
        _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Post(command);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(expectedResponse, createdResult.Value);
    }
}
```

#### 3. Validation Tests
Validation tests ensure business rules are properly enforced:

```csharp
public class CreateEmployeeCommandValidatorTests
{
    [Fact]
    public async Task Should_Have_Error_When_FirstName_Is_Empty()
    {
        // Arrange
        var command = new CreateEmployeeCommand { FirstName = "" };
        
        // Act
        var result = await _validator.TestValidateAsync(command);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }
}
```

### Testing Principles
- **Arrange-Act-Assert Pattern**: Clear test structure
- **Mock External Dependencies**: Isolate units under test
- **Test Edge Cases**: Invalid inputs, boundary conditions, error scenarios
- **Descriptive Test Names**: Clear intent and expected behavior
- **Single Responsibility**: Each test verifies one specific behavior

## Project Structure

### Domain Layer (`TalentManagement.Domain`)
- **Entities**: Core business objects (Employee, Department, Position, SalaryRange)
- **Enums**: Domain-specific enumerations (Gender, Roles)
- **Common**: Base classes for entities (BaseEntity, AuditableBaseEntity)
- **Settings**: Configuration objects (JWTSettings, MailSettings)

### Application Layer (`TalentManagement.Application`)
- **Features**: CQRS commands and queries organized by aggregate
- **DTOs**: Data transfer objects for external integrations
- **Behaviours**: Cross-cutting concerns (ValidationBehaviour)
- **Interfaces**: Contracts for repositories and services
- **Mappings**: AutoMapper profiles
- **Wrappers**: Response wrapper classes
- **Parameters**: Query parameter classes for filtering and pagination

### Infrastructure Layers
- **Persistence**: Entity Framework, repositories, database context
- **Shared**: External services, caching, email services, mock data generation

### Presentation Layer (`TalentManagement.WebApi`)
- **Controllers**: API endpoints organized by version
- **Extensions**: Service registration and configuration
- **Middlewares**: Global error handling
- **Models**: API-specific models

This architecture ensures maintainability, testability, and clear separation of concerns while providing robust validation and comprehensive test coverage.