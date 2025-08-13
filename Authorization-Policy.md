# Authorization Policy and RBAC Documentation

This document provides comprehensive information about Role-Based Access Control (RBAC) implementation in the TalentManagement API using JWT tokens and IdentityServer integration.

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [JWT Token Integration](#jwt-token-integration)
- [IdentityServer Integration](#identityserver-integration)
- [Authorization Policies](#authorization-policies)
- [Role Hierarchy](#role-hierarchy)
- [Implementation Details](#implementation-details)
- [Security Flow](#security-flow)
- [Configuration](#configuration)
- [Usage Examples](#usage-examples)
- [Testing Authorization](#testing-authorization)
- [Troubleshooting](#troubleshooting)
- [Best Practices](#best-practices)

## 🎯 Overview

The TalentManagement API implements a robust Role-Based Access Control (RBAC) system that uses:

- **JWT Bearer Tokens** for authentication
- **IdentityServer4/Duende IdentityServer** for token issuance and validation
- **Policy-Based Authorization** for fine-grained access control
- **Role-Based Policies** for hierarchical permissions

**Key Benefits:**
- 🔐 Centralized authentication through IdentityServer
- 🏢 Hierarchical role-based permissions
- 🛡️ Stateless JWT token security
- 📊 Granular endpoint protection
- 🔄 Scalable across multiple APIs

## 🏗️ Architecture

### High-Level Architecture
```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Client App    │    │  IdentityServer  │    │ TalentMgmt API  │
│                 │    │                  │    │                 │
│ 1. Login Request│───►│ 2. Authenticate  │    │                 │
│                 │    │ 3. Issue JWT     │    │                 │
│ 4. Receive JWT  │◄───┤    Token         │    │                 │
│                 │    │                  │    │                 │
│ 5. API Request  │────────────────────────────►│ 6. Validate JWT │
│    + JWT Token  │    │                  │    │ 7. Authorize    │
│                 │    │                  │    │ 8. Return Data  │
│ 9. Receive Data │◄────────────────────────────┤                 │
└─────────────────┘    └──────────────────┘    └─────────────────┘
```

### JWT Token Flow
```mermaid
sequenceDiagram
    participant Client
    participant IdentityServer
    participant API
    
    Client->>IdentityServer: 1. Authenticate (username/password)
    IdentityServer->>IdentityServer: 2. Validate credentials
    IdentityServer->>Client: 3. Return JWT token with roles
    Client->>API: 4. API request with Bearer token
    API->>API: 5. Validate token signature
    API->>API: 6. Extract roles from token
    API->>API: 7. Check policy requirements
    API->>Client: 8. Return authorized response
```

## 🎫 JWT Token Integration

### JWT Configuration

The API is configured to accept JWT tokens from a trusted IdentityServer instance:

```csharp
// Program.cs
builder.Services.AddJWTAuthentication(builder.Configuration);

// ServiceExtensions.cs
public static void AddJWTAuthentication(this IServiceCollection services, IConfiguration configuration)
{
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; // Set to true in production
            options.Authority = configuration["Sts:ServerUrl"]; // IdentityServer URL
            options.Audience = configuration["Sts:Audience"];   // API resource identifier
        });
}
```

### JWT Token Structure

A typical JWT token contains the following claims:

```json
{
  "header": {
    "alg": "RS256",
    "typ": "JWT",
    "kid": "key-id"
  },
  "payload": {
    "iss": "https://localhost:44310",
    "aud": "app.api.employeeprofile",
    "exp": 1640995200,
    "nbf": 1640991600,
    "iat": 1640991600,
    "sub": "user-123",
    "role": ["Employee", "Manager"],
    "email": "user@company.com",
    "name": "John Doe",
    "scope": ["api.read", "api.write"]
  },
  "signature": "signature-data"
}
```

**Important Claims:**
- `iss` - Issuer (IdentityServer URL)
- `aud` - Audience (API identifier)
- `sub` - Subject (User ID)
- `role` - User roles (used for authorization)
- `exp` - Expiration time
- `scope` - API scopes

## 🏢 IdentityServer Integration

### Configuration Settings

```json
{
  "Sts": {
    "ServerUrl": "https://localhost:44310",
    "Audience": "app.api.employeeprofile"
  },
  "JWTSettings": {
    "Key": "C1CF4B7DC4C4175B6618DE4F55CA4",
    "Issuer": "CoreIdentity",
    "Audience": "CoreIdentityUser",
    "DurationInMinutes": 60
  }
}
```

### IdentityServer Setup Requirements

For proper integration, IdentityServer must be configured with:

1. **API Resource Definition**:
```csharp
public static IEnumerable<ApiResource> GetApiResources()
{
    return new List<ApiResource>
    {
        new ApiResource("app.api.employeeprofile", "TalentManagement API")
        {
            Scopes = { "api.read", "api.write", "api.admin" }
        }
    };
}
```

2. **Client Configuration**:
```csharp
public static IEnumerable<Client> GetClients()
{
    return new List<Client>
    {
        new Client
        {
            ClientId = "talent-management-client",
            ClientSecrets = { new Secret("secret".Sha256()) },
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            AllowedScopes = { "app.api.employeeprofile" }
        }
    };
}
```

3. **Role Claims Configuration**:
```csharp
public static IEnumerable<IdentityResource> GetIdentityResources()
{
    return new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResource("roles", "User roles", new[] { "role" })
    };
}
```

## 🛡️ Authorization Policies

### Policy Configuration

The API defines three hierarchical authorization policies:

```csharp
public static void AddAuthorizationPolicies(this IServiceCollection services, IConfiguration configuration)
{
    string admin = configuration["ApiRoles:AdminRole"],        // "HRAdmin"
           manager = configuration["ApiRoles:ManagerRole"],     // "Manager"
           employee = configuration["ApiRoles:EmployeeRole"];   // "Employee"

    services.AddAuthorization(options =>
    {
        // Admin policy - requires HRAdmin role
        options.AddPolicy(AuthorizationConsts.AdminPolicy, 
            policy => policy.RequireRole(admin));

        // Manager policy - requires Manager OR HRAdmin role
        options.AddPolicy(AuthorizationConsts.ManagerPolicy, 
            policy => policy.RequireRole(manager, admin));

        // Employee policy - requires Employee OR Manager OR HRAdmin role
        options.AddPolicy(AuthorizationConsts.EmployeePolicy, 
            policy => policy.RequireRole(employee, manager, admin));
    });
}
```

### Policy Constants

```csharp
public class AuthorizationConsts
{
    public const string AdminPolicy = "AdminPolicy";
    public const string ManagerPolicy = "ManagerPolicy";
    public const string EmployeePolicy = "EmployeePolicy";
}
```

## 👥 Role Hierarchy

### Role Definitions

```json
{
  "ApiRoles": {
    "EmployeeRole": "Employee",
    "ManagerRole": "Manager", 
    "AdminRole": "HRAdmin"
  }
}
```

### Permission Matrix

| Operation | Employee | Manager | HRAdmin |
|-----------|----------|---------|---------|
| **READ Operations** |
| GET /employees | ✅ | ✅ | ✅ |
| GET /positions | ✅ | ✅ | ✅ |
| GET /departments | ✅ | ✅ | ✅ |
| GET /salaryranges | ✅ | ✅ | ✅ |
| **WRITE Operations** |
| POST /employees | ❌ | ❌ | ✅ |
| PUT /employees/{id} | ❌ | ❌ | ✅ |
| DELETE /employees/{id} | ❌ | ❌ | ✅ |
| POST /positions | ❌ | ❌ | ✅ |
| PUT /positions/{id} | ❌ | ❌ | ✅ |
| DELETE /positions/{id} | ❌ | ❌ | ✅ |
| POST /departments | ❌ | ❌ | ✅ |
| PUT /departments/{id} | ❌ | ❌ | ✅ |
| DELETE /departments/{id} | ❌ | ❌ | ✅ |
| POST /salaryranges | ❌ | ❌ | ✅ |
| PUT /salaryranges/{id} | ❌ | ❌ | ✅ |
| DELETE /salaryranges/{id} | ❌ | ❌ | ✅ |
| **PUBLIC Operations** |
| GET /employees/count | ✅ | ✅ | ✅ |
| GET /info | ✅ | ✅ | ✅ |

### Role Inheritance

The system implements hierarchical role inheritance:

```
HRAdmin (highest privilege)
    ├── Has all Manager permissions
    ├── Has all Employee permissions  
    └── Can perform CRUD operations

Manager (medium privilege)
    ├── Has all Employee permissions
    └── Can read all resources

Employee (basic privilege)
    └── Can read all resources
```

## 🔧 Implementation Details

### Controller Authorization

Controllers use attribute-based authorization:

```csharp
[ApiVersion("1.0")]
public class EmployeesController : BaseApiController
{
    // Read operations - requires any authenticated user
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get([FromQuery] GetEmployeesQuery filter)

    // Write operations - requires Admin role
    [HttpPost]
    [Authorize(Policy = AuthorizationConsts.AdminPolicy)]
    public async Task<IActionResult> Post(CreateEmployeeCommand command)

    // Public operations - no authorization required
    [HttpGet("count")]
    public async Task<IActionResult> GetCount([FromQuery] GetEmployeesCountQuery filter)
}
```

### Middleware Pipeline

The authorization middleware is configured in the correct order:

```csharp
// Program.cs - Middleware pipeline order is crucial
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();    // Must come before UseAuthorization
app.UseAuthorization();     // Must come after UseAuthentication
app.MapControllers();
```

### Custom Authorization Requirements

You can extend the system with custom requirements:

```csharp
// Custom requirement example
public class DepartmentAccessRequirement : IAuthorizationRequirement
{
    public string RequiredDepartment { get; }
    
    public DepartmentAccessRequirement(string department)
    {
        RequiredDepartment = department;
    }
}

// Custom handler
public class DepartmentAccessHandler : AuthorizationHandler<DepartmentAccessRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        DepartmentAccessRequirement requirement)
    {
        var userDepartment = context.User.FindFirst("department")?.Value;
        
        if (userDepartment == requirement.RequiredDepartment)
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}
```

## 🔒 Security Flow

### Authentication Flow

1. **Client Authentication**:
   ```http
   POST https://localhost:44310/connect/token
   Content-Type: application/x-www-form-urlencoded

   grant_type=client_credentials
   &client_id=talent-management-client
   &client_secret=secret
   &scope=app.api.employeeprofile
   ```

2. **Token Response**:
   ```json
   {
     "access_token": "eyJhbGciOiJSUzI1NiIs...",
     "expires_in": 3600,
     "token_type": "Bearer",
     "scope": "app.api.employeeprofile"
   }
   ```

3. **API Request**:
   ```http
   GET https://localhost:5001/api/v1/employees
   Authorization: Bearer eyJhbGciOiJSUzI1NiIs...
   ```

### Authorization Flow

1. **Token Validation**:
   - Verify token signature using IdentityServer's public key
   - Check token expiration (`exp` claim)
   - Validate audience (`aud` claim)
   - Validate issuer (`iss` claim)

2. **Role Extraction**:
   - Extract `role` claims from JWT token
   - Map roles to ClaimsPrincipal

3. **Policy Evaluation**:
   - Check if user has required role for the policy
   - Apply hierarchical permissions
   - Allow or deny access

## ⚙️ Configuration

### Required Configuration

#### appsettings.json
```json
{
  "Sts": {
    "ServerUrl": "https://localhost:44310",
    "Audience": "app.api.employeeprofile"
  },
  "ApiRoles": {
    "EmployeeRole": "Employee",
    "ManagerRole": "Manager",
    "AdminRole": "HRAdmin"
  },
  "JWTSettings": {
    "Key": "your-secret-key-here",
    "Issuer": "CoreIdentity", 
    "Audience": "CoreIdentityUser",
    "DurationInMinutes": 60
  }
}
```

#### Environment-Specific Settings

**Development:**
```json
{
  "Sts": {
    "ServerUrl": "https://localhost:44310"
  }
}
```

**Production:**
```json
{
  "Sts": {
    "ServerUrl": "https://identityserver.company.com"
  }
}
```

### SSL/TLS Configuration

For production, ensure proper SSL configuration:

```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;  // Enforce HTTPS in production
        options.Authority = configuration["Sts:ServerUrl"];
        options.Audience = configuration["Sts:Audience"];
        
        // Additional security options
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };
    });
```

## 🧪 Usage Examples

### Frontend Integration

#### JavaScript/TypeScript Example
```typescript
class ApiClient {
    private accessToken: string;

    async authenticate(): Promise<void> {
        const response = await fetch('https://localhost:44310/connect/token', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: new URLSearchParams({
                grant_type: 'client_credentials',
                client_id: 'talent-management-client',
                client_secret: 'secret',
                scope: 'app.api.employeeprofile'
            })
        });

        const tokenData = await response.json();
        this.accessToken = tokenData.access_token;
    }

    async getEmployees(): Promise<Employee[]> {
        const response = await fetch('https://localhost:5001/api/v1/employees', {
            headers: {
                'Authorization': `Bearer ${this.accessToken}`,
                'Content-Type': 'application/json'
            }
        });

        if (response.status === 401) {
            throw new Error('Unauthorized - token expired or invalid');
        }

        if (response.status === 403) {
            throw new Error('Forbidden - insufficient permissions');
        }

        return await response.json();
    }

    async createEmployee(employee: CreateEmployeeRequest): Promise<Employee> {
        const response = await fetch('https://localhost:5001/api/v1/employees', {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${this.accessToken}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(employee)
        });

        if (response.status === 403) {
            throw new Error('Forbidden - Admin role required for this operation');
        }

        return await response.json();
    }
}
```

#### C# Client Example
```csharp
public class TalentManagementApiClient
{
    private readonly HttpClient _httpClient;
    private string _accessToken;

    public TalentManagementApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task AuthenticateAsync()
    {
        var tokenRequest = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = "talent-management-client",
            ["client_secret"] = "secret",
            ["scope"] = "app.api.employeeprofile"
        };

        var response = await _httpClient.PostAsync(
            "https://localhost:44310/connect/token",
            new FormUrlEncodedContent(tokenRequest));

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
        _accessToken = tokenResponse.AccessToken;
    }

    public async Task<List<Employee>> GetEmployeesAsync()
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _accessToken);

        var response = await _httpClient.GetAsync("api/v1/employees");
        
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException("Token expired or invalid");
            
        if (response.StatusCode == HttpStatusCode.Forbidden)
            throw new UnauthorizedAccessException("Insufficient permissions");

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Employee>>();
    }
}
```

### Swagger/OpenAPI Integration

The API automatically includes JWT authentication in Swagger:

```csharp
services.AddSwaggerGen(c =>
{
    // Add JWT security definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Input your Bearer token to access this API"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});
```

## 🧪 Testing Authorization

### Unit Testing Authorization

```csharp
[Test]
public async Task Get_WithValidToken_ReturnsEmployees()
{
    // Arrange
    var mockMediator = new Mock<IMediator>();
    var controller = new EmployeesController();
    
    var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.Role, "Employee"),
        new Claim(ClaimTypes.NameIdentifier, "user123")
    }, "TestAuthType"));

    controller.ControllerContext = new ControllerContext
    {
        HttpContext = new DefaultHttpContext { User = user }
    };

    // Act
    var result = await controller.Get(new GetEmployeesQuery());

    // Assert
    Assert.IsInstanceOf<OkObjectResult>(result);
}

[Test]
public async Task Post_WithoutAdminRole_ReturnsForbidden()
{
    // Arrange
    var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.Role, "Employee")
    }, "TestAuthType"));

    // This test would verify that the authorization policy blocks access
    // In practice, this would be handled by the authorization middleware
}
```

### Integration Testing

```csharp
public class AuthorizationIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthorizationIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Test]
    public async Task GetEmployees_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/employees");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Test]
    public async Task CreateEmployee_WithEmployeeToken_ReturnsForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        var token = GenerateTestToken(new[] { "Employee" });
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/employees", 
            new CreateEmployeeCommand());

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Test]
    public async Task CreateEmployee_WithAdminToken_ReturnsCreated()
    {
        // Arrange
        var client = _factory.CreateClient();
        var token = GenerateTestToken(new[] { "HRAdmin" });
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/employees", 
            new CreateEmployeeCommand 
            { 
                FirstName = "Test",
                LastName = "User",
                Email = "test@company.com"
            });

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private string GenerateTestToken(string[] roles)
    {
        // Implementation to generate JWT token for testing
        // This would typically use the same signing key as your test environment
    }
}
```

## 🔍 Troubleshooting

### Common Issues

#### 1. 401 Unauthorized Errors

**Possible Causes:**
- Invalid or expired JWT token
- Wrong audience/issuer configuration
- IdentityServer not accessible
- Token signature validation failed

**Solutions:**
```csharp
// Check token expiration
var tokenHandler = new JwtSecurityTokenHandler();
var token = tokenHandler.ReadJwtToken(jwtToken);
Console.WriteLine($"Token expires: {token.ValidTo}");

// Validate configuration
if (configuration["Sts:ServerUrl"] != tokenIssuer)
{
    throw new InvalidOperationException("Token issuer mismatch");
}
```

#### 2. 403 Forbidden Errors

**Possible Causes:**
- User lacks required role
- Policy configuration error
- Role claim missing from token

**Solutions:**
```csharp
// Debug user claims
foreach (var claim in User.Claims)
{
    Console.WriteLine($"{claim.Type}: {claim.Value}");
}

// Check role claims specifically
var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
Console.WriteLine($"User roles: {string.Join(", ", roles)}");
```

#### 3. IdentityServer Connection Issues

**Symptoms:**
- Slow API responses
- Intermittent 401 errors
- Token validation failures

**Solutions:**
```csharp
// Add retry policy
services.AddHttpClient("IdentityServer")
    .AddPolicyHandler(GetRetryPolicy());

private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}
```

#### 4. Role Claim Issues

**Problem:** Roles not being read from token correctly

**Solution:**
```csharp
// Custom JWT token validation
services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
    options.TokenValidationParameters.NameClaimType = ClaimTypes.Name;
});
```

### Debugging Tips

#### Enable Detailed Logging

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore.Authorization": "Debug",
      "Microsoft.AspNetCore.Authentication": "Debug"
    }
  }
}
```

#### Custom Middleware for Debugging

```csharp
public class AuthorizationDebuggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthorizationDebuggingMiddleware> _logger;

    public AuthorizationDebuggingMiddleware(RequestDelegate next, ILogger<AuthorizationDebuggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity.IsAuthenticated)
        {
            var claims = string.Join(", ", context.User.Claims.Select(c => $"{c.Type}={c.Value}"));
            _logger.LogInformation("Authenticated user claims: {Claims}", claims);
        }

        await _next(context);
    }
}
```

## ✅ Best Practices

### Security Best Practices

1. **Use HTTPS in Production**
   ```csharp
   options.RequireHttpsMetadata = true; // Always true in production
   ```

2. **Validate All Token Claims**
   ```csharp
   options.TokenValidationParameters = new TokenValidationParameters
   {
       ValidateIssuer = true,
       ValidateAudience = true,
       ValidateLifetime = true,
       ValidateIssuerSigningKey = true,
       ClockSkew = TimeSpan.FromMinutes(5) // Reduce clock skew tolerance
   };
   ```

3. **Use Short Token Lifetimes**
   ```json
   {
     "JWTSettings": {
       "DurationInMinutes": 60  // 1 hour maximum
     }
   }
   ```

4. **Implement Token Refresh**
   ```csharp
   // Implement refresh token logic
   public async Task<string> RefreshTokenAsync(string refreshToken)
   {
       // Validate refresh token and issue new access token
   }
   ```

### Performance Best Practices

1. **Cache Token Validation Results**
   ```csharp
   services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
   {
       options.SaveToken = true;
       options.MapInboundClaims = false; // Reduce claim mapping overhead
   });
   ```

2. **Use Connection Pooling for IdentityServer**
   ```csharp
   services.AddHttpClient("IdentityServer", client =>
   {
       client.BaseAddress = new Uri(configuration["Sts:ServerUrl"]);
   }).ConfigurePrimaryHttpMessageHandler(() =>
   {
       return new HttpClientHandler
       {
           MaxConnectionsPerServer = 20
       };
   });
   ```

3. **Optimize Authorization Policies**
   ```csharp
   // Prefer specific role checks over complex policy logic
   [Authorize(Roles = "HRAdmin")]
   public async Task<IActionResult> AdminOnlyAction()
   {
       // Implementation
   }
   ```

### Development Best Practices

1. **Use Policy Constants**
   ```csharp
   // Good - centralized constants
   [Authorize(Policy = AuthorizationConsts.AdminPolicy)]
   
   // Avoid - magic strings
   [Authorize(Policy = "AdminPolicy")]
   ```

2. **Implement Graceful Error Handling**
   ```csharp
   public class AuthorizationErrorHandlingMiddleware
   {
       public async Task InvokeAsync(HttpContext context)
       {
           try
           {
               await _next(context);
           }
           catch (UnauthorizedAccessException ex)
           {
               context.Response.StatusCode = 401;
               await context.Response.WriteAsync("Unauthorized access");
           }
       }
   }
   ```

3. **Document Authorization Requirements**
   ```csharp
   /// <summary>
   /// Creates a new employee record.
   /// </summary>
   /// <param name="command">Employee creation data</param>
   /// <returns>Created employee</returns>
   /// <remarks>
   /// Requires HRAdmin role. Only HR administrators can create employee records.
   /// </remarks>
   [HttpPost]
   [Authorize(Policy = AuthorizationConsts.AdminPolicy)]
   public async Task<IActionResult> Post(CreateEmployeeCommand command)
   ```

### Monitoring and Auditing

1. **Log Authorization Events**
   ```csharp
   public class AuthorizationLoggingHandler : IAuthorizationHandler
   {
       private readonly ILogger<AuthorizationLoggingHandler> _logger;

       public Task HandleAsync(AuthorizationHandlerContext context)
       {
           if (context.HasFailed)
           {
               _logger.LogWarning("Authorization failed for user {User} on resource {Resource}",
                   context.User.Identity.Name, context.Resource);
           }

           return Task.CompletedTask;
       }
   }
   ```

2. **Monitor Token Usage**
   ```csharp
   services.AddSingleton<IAuthorizationMiddlewareResultHandler, 
       CustomAuthorizationMiddlewareResultHandler>();
   ```

3. **Audit Trail Implementation**
   ```csharp
   public class AuditActionFilter : ActionFilterAttribute
   {
       public override void OnActionExecuted(ActionExecutedContext context)
       {
           var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
           var action = $"{context.Controller}.{context.ActionDescriptor.DisplayName}";
           
           // Log to audit trail
           Log.Information("User {UserId} performed action {Action}", userId, action);
       }
   }
   ```

---

## 📞 Support and Further Reading

### Official Documentation
- [ASP.NET Core Authorization](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/)
- [JWT Bearer Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/)
- [IdentityServer4 Documentation](https://identityserver4.readthedocs.io/)

### Internal Documentation
- [API Endpoints Documentation](README.md#api-endpoints)
- [Configuration Guide](README.md#configuration)
- [Testing Documentation](TalentManagement.Tests/README.md)

---

**Last Updated:** January 2025  
**Version:** 1.0  
**Framework:** .NET 9.0 / ASP.NET Core