# TalentManagement API

A comprehensive talent management system built with .NET 9 and Clean Architecture principles.

## Overview

TalentManagement is a RESTful API for managing organizational talent including employees, positions, departments, and salary ranges. The system follows Clean Architecture principles with CQRS pattern implementation using MediatR.

## Architecture

The solution is organized into the following layers:

### Core
- **TalentManagement.Domain** - Contains entities, enums, and domain logic
- **TalentManagement.Application** - Business logic, commands, queries, and application services

### Infrastructure
- **TalentManagement.Infrastructure.Persistence** - Data access layer with Entity Framework Core
- **TalentManagement.Infrastructure.Shared** - Shared services like email, logging, and mock data generation

### Presentation
- **TalentManagement.WebApi** - REST API controllers and configuration

### Tests
- **TalentManagement.Tests** - Unit tests for all layers

## Features

- **Employee Management** - CRUD operations for employee records
- **Position Management** - Manage job positions with departments and salary ranges
- **Department Management** - Organize positions by departments
- **Salary Range Management** - Define salary ranges for positions
- **API Versioning** - Versioned endpoints for backward compatibility
- **Swagger Documentation** - Auto-generated API documentation
- **Health Checks** - Built-in health monitoring
- **Logging** - Structured logging with Serilog
- **Authentication** - JWT-based authentication support

## Technologies

- **.NET 9** - Latest .NET framework
- **Entity Framework Core 9** - ORM for data access
- **MediatR** - CQRS and mediator pattern implementation
- **AutoMapper** - Object-to-object mapping
- **FluentValidation** - Input validation
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation
- **xUnit** - Unit testing framework
- **Bogus** - Test data generation

## Getting Started

### Prerequisites

- .NET 9 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Setup

1. Clone the repository
2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Update the connection string in `appsettings.json`

4. Run database migrations:
   ```bash
   dotnet ef database update
   ```

5. Run the application:
   ```bash
   dotnet run --project TalentManagement/TalentManagement.WebApi
   ```

6. Navigate to `https://localhost:5001/swagger` to view the API documentation

### Running Tests

Execute unit tests with:
```bash
dotnet test
```

## API Endpoints

### Employees
- `GET /api/v1/employees` - Get all employees
- `GET /api/v1/employees/{id}` - Get employee by ID
- `POST /api/v1/employees` - Create new employee
- `PUT /api/v1/employees/{id}` - Update employee
- `DELETE /api/v1/employees/{id}` - Delete employee

### Positions
- `GET /api/v1/positions` - Get all positions
- `GET /api/v1/positions/{id}` - Get position by ID
- `POST /api/v1/positions` - Create new position
- `PUT /api/v1/positions/{id}` - Update position
- `DELETE /api/v1/positions/{id}` - Delete position

### Departments
- `GET /api/v1/departments` - Get all departments

### Salary Ranges
- `GET /api/v1/salaryranges` - Get all salary ranges

## Configuration

Key configuration options in `appsettings.json`:

- **ConnectionStrings** - Database connection settings
- **JWTSettings** - JWT authentication configuration
- **MailSettings** - Email service configuration
- **Logging** - Serilog configuration

## Project Structure

```
TalentManagement/
├── TalentManagement.Domain/          # Domain entities and business rules
├── TalentManagement.Application/     # Application services and business logic
├── TalentManagement.Infrastructure.Persistence/  # Data access layer
├── TalentManagement.Infrastructure.Shared/       # Shared services
├── TalentManagement.WebApi/          # API controllers and configuration
└── TalentManagement.Tests/           # Unit tests
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## License

This project is licensed under the MIT License.

## Author

**Fuji Nguyen** - [workcontrolgit](https://github.com/workcontrolgit)