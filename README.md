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

- **Employee Management** - Read operations and paginated data retrieval for employee records
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
- `GET /api/v1/employees` - Get all employees with optional filtering and pagination
- `GET /api/v1/employees/{id}` - Get employee by ID
- `POST /api/v1/employees` - Create new employee
- `POST /api/v1/employees/Paged` - Get paginated employee data (supports NgX-DataTables)
- `PUT /api/v1/employees/{id}` - Update employee
- `DELETE /api/v1/employees/{id}` - Delete employee
- `GET /api/v1/employees/count` - Get employees count with optional filtering

### Positions (Authorization Required)
- `GET /api/v1/positions` - Get all positions with optional filtering and pagination
- `GET /api/v1/positions/{id}` - Get position by ID
- `POST /api/v1/positions` - Create new position (Admin only)
- `POST /api/v1/positions/AddMock` - Add mock position data (Admin only)
- `POST /api/v1/positions/Paged` - Get paginated position data
- `PUT /api/v1/positions/{id}` - Update position (Admin only)
- `DELETE /api/v1/positions/{id}` - Delete position (Admin only)
- `GET /api/v1/positions/count` - Get positions count with optional filtering

### Departments
- `GET /api/v1/departments` - Get all departments with optional filtering by name and paging
- `GET /api/v1/departments/{id}` - Get department by ID
- `POST /api/v1/departments` - Create new department
- `POST /api/v1/departments/Paged` - Get paginated department data
- `PUT /api/v1/departments/{id}` - Update department
- `DELETE /api/v1/departments/{id}` - Delete department
- `GET /api/v1/departments/count` - Get departments count with optional filtering

### Salary Ranges
- `GET /api/v1/salaryranges` - Get all salary ranges with optional filtering by name and paging
- `GET /api/v1/salaryranges/{id}` - Get salary range by ID
- `POST /api/v1/salaryranges` - Create new salary range
- `POST /api/v1/salaryranges/Paged` - Get paginated salary range data
- `PUT /api/v1/salaryranges/{id}` - Update salary range
- `DELETE /api/v1/salaryranges/{id}` - Delete salary range
- `GET /api/v1/salaryranges/count` - Get salary ranges count with optional filtering

### USAJobs Integration
- `POST /api/v1/usajobs/search` - Search for jobs from USAJobs API
- `GET /api/v1/usajobs/search` - Search USAJobs with GET method for simple integration
- `GET /api/v1/usajobs/{positionId}` - Get detailed information about a specific USAJobs posting
- `GET /api/v1/usajobs/health` - Test connectivity to USAJobs API
- `GET /api/v1/usajobs/info` - Get information about USAJobs API usage and limits

### USAJobs Code Lists
- `GET /api/v1/usajobscodelist/occupational-series` - Get all occupational series codes
- `GET /api/v1/usajobscodelist/occupational-series/search` - Search occupational series by keyword
- `GET /api/v1/usajobscodelist/occupational-series/{code}` - Get occupational series by code
- `GET /api/v1/usajobscodelist/pay-plans` - Get all pay plan codes
- `GET /api/v1/usajobscodelist/pay-plans/{code}` - Get pay plan by code
- `GET /api/v1/usajobscodelist/hiring-paths` - Get all hiring path codes
- `GET /api/v1/usajobscodelist/position-schedule-types` - Get all position schedule type codes
- `GET /api/v1/usajobscodelist/security-clearances` - Get all security clearance codes
- `GET /api/v1/usajobscodelist/countries` - Get all country codes
- `GET /api/v1/usajobscodelist/postal-codes` - Get all postal code (state) codes
- `GET /api/v1/usajobscodelist/agency-subelements` - Get all agency subelements
- `GET /api/v1/usajobscodelist/gsa-geo-location-codes` - Get all GSA geo location codes
- `GET /api/v1/usajobscodelist/geo-locations` - Get all geo locations
- `GET /api/v1/usajobscodelist/travel-requirements` - Get all travel requirements
- `GET /api/v1/usajobscodelist/remote-work-options` - Get all remote work options
- `GET /api/v1/usajobscodelist/country-subdivisions` - Get all country subdivisions
- `GET /api/v1/usajobscodelist/travel-percentages` - Get all travel percentages
- `GET /api/v1/usajobscodelist/position-offering-types` - Get all position offering types
- `GET /api/v1/usajobscodelist/who-may-apply` - Get all who may apply codes
- `GET /api/v1/usajobscodelist/academic-honors` - Get all academic honors
- `GET /api/v1/usajobscodelist/action-codes` - Get all action codes
- `GET /api/v1/usajobscodelist/degree-type-codes` - Get all degree type codes
- `GET /api/v1/usajobscodelist/document-formats` - Get all document formats
- `GET /api/v1/usajobscodelist/race-codes` - Get all race codes
- `GET /api/v1/usajobscodelist/ethnicities` - Get all ethnicities
- `GET /api/v1/usajobscodelist/documentations` - Get all documentations
- `GET /api/v1/usajobscodelist/federal-employment-statuses` - Get all federal employment statuses
- `GET /api/v1/usajobscodelist/language-proficiencies` - Get all language proficiencies
- `GET /api/v1/usajobscodelist/language-codes` - Get all language codes
- `GET /api/v1/usajobscodelist/military-status-codes` - Get all military status codes
- `GET /api/v1/usajobscodelist/referee-type-codes` - Get all referee type codes
- `GET /api/v1/usajobscodelist/special-hirings` - Get all special hirings
- `GET /api/v1/usajobscodelist/remuneration-rate-interval-codes` - Get all remuneration rate interval codes
- `GET /api/v1/usajobscodelist/application-statuses` - Get all application statuses
- `GET /api/v1/usajobscodelist/academic-levels` - Get all academic levels
- `GET /api/v1/usajobscodelist/key-standard-requirements` - Get all key standard requirements
- `GET /api/v1/usajobscodelist/required-standard-documents` - Get all required standard documents
- `GET /api/v1/usajobscodelist/disabilities` - Get all disabilities
- `GET /api/v1/usajobscodelist/applicant-suppliers` - Get all applicant suppliers
- `GET /api/v1/usajobscodelist/mission-critical-codes` - Get all mission critical codes
- `GET /api/v1/usajobscodelist/announcement-closing-types` - Get all announcement closing types
- `GET /api/v1/usajobscodelist/service-types` - Get all service types
- `GET /api/v1/usajobscodelist/location-expansions` - Get all location expansions
- `POST /api/v1/usajobscodelist/refresh` - Refresh all code lists from the USAJobs API
- `GET /api/v1/usajobscodelist/health` - Check if the USAJobs code list service is available

### System Information
- `GET /info` - Get application version and last update information

## Documentation

This project includes comprehensive documentation covering various aspects of the system:

### Architecture & Design
- **[Web API Design](WebAPIDesign.md)** - Complete Position Description Web API design document with endpoints, data models, and integration patterns
- **[Cache Provider Architecture](CACHE-PROVIDER.md)** - Cache system architecture tutorial covering both Memory and Redis providers

### External API Integration
- **[USAJobs Code List Cache](ExternalAPI-Code-List-Cache.md)** - Cache implementation strategy for USAJobs code list APIs
- **[USAJobs Search Cache](ExternalAPI-Job-Search-Cache.md)** - Cache implementation for USAJobs search functionality

### Testing
- **[Test Documentation](TalentManagement.Tests/README.md)** - Comprehensive test suite documentation with coverage details and running instructions

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