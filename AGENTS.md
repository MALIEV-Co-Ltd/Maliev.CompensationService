# Agentic Coding Guidelines for Maliev.CompensationService

This document defines the protocols, code style, and operational workflows for AI agents working in this repository.

## 1. Project Context & Technology Stack

- **Framework:** .NET 10.0 (Preview/Latest)
- **Architecture:** Domain-Driven Design (DDD) with Clean Architecture and CQRS.
- **Data Access:** Entity Framework Core 10.x with PostgreSQL.
- **Messaging:** MassTransit with RabbitMQ.
- **Testing:** xUnit, Testcontainers (Postgres, Redis, RabbitMQ), Moq, WebApplicationFactory.
- **Platform:** Maliev.Aspire.ServiceDefaults integration.

## 2. Build, Test, and Lint Commands

All commands run from within this service directory (`B:\maliev\Maliev.CompensationService`).

```powershell
# Build (treats warnings as errors — all must be fixed)
dotnet build Maliev.CompensationService.slnx

# Run all tests
dotnet test Maliev.CompensationService.slnx --verbosity normal

# Run a single test method
dotnet test --filter "FullyQualifiedName~CompensationControllerTests.GetCompensationDetails_ShouldReturnOk_WhenEmployeeExists"

# Run all tests in a class
dotnet test --filter "FullyQualifiedName~CompensationControllerTests"

# Run with code coverage
dotnet test Maliev.CompensationService.slnx --collect:"XPlat Code Coverage"

# Format check
dotnet format Maliev.CompensationService.slnx

# EF Core migrations (Infrastructure project only)
dotnet ef migrations add <Name> --project Maliev.CompensationService.Infrastructure --startup-project Maliev.CompensationService.Infrastructure
```

## 3. Code Style & Conventions

### Workspace Structure
```
Maliev.CompensationService/
├── Maliev.CompensationService.Api/           # Controllers, Consumers, Middleware
├── Maliev.CompensationService.Application/   # Use cases, DTOs, Interfaces, Handlers
├── Maliev.CompensationService.Domain/        # Entities, value objects, domain interfaces
├── Maliev.CompensationService.Infrastructure/ # EF Core DbContext, repositories, HTTP clients
├── Maliev.CompensationService.Tests/         # Unit + Integration tests (xUnit)
├── Directory.Build.props                     # Central package versioning
└── Maliev.CompensationService.slnx          # Solution file (.slnx preferred over .sln)
```

### C# Naming & Formatting
- **Namespaces**: File-scoped (`namespace Maliev.CompensationService.Domain.Entities;`)
- **Classes/Methods/Properties**: `PascalCase`
- **Private fields**: `_camelCase` (underscore prefix)
- **Parameters/locals**: `camelCase`
- **Async methods**: Suffix with `Async` (e.g., `GetByEmployeeIdAsync`)
- **Interfaces**: Prefix with `I` (e.g., `ICompensationRepository`)
- **Permissions**: GCP-style `{domain}.{plural-resource}.{action}` as `public const string` in a `Permissions` static class
  - Valid: `compensation.compensations.create`, `compensation.salaries.update`
  - Invalid: `compensation.compensation.create` (singular), `compensation.create` (missing resource)
- **XML docs**: Required on ALL public methods and properties
- **Nullable**: Enabled (`<Nullable>enable</Nullable>`). Use `?` explicitly
- **Imports**: System first, then third-party, then local. Alphabetize within groups. Remove unused `using`
- **Braces**: Allman style (new line) for methods and control structures. Expression-bodied for properties/accessors
- **Indentation**: 4 spaces, LF line endings, UTF-8, trim trailing whitespace

### C# Patterns
- **DI**: Constructor injection with `private readonly` fields
- **Controllers**: `[ApiController]`, `[ApiVersion("1")]`, `[Route("compensation/v{version:apiVersion}")]`
- **Logging**: `ILogger<T>` with structured placeholders (never interpolate): `_logger.LogInformation("Processing {EmployeeId}", employeeId)`
- **Error handling**: Global exception middleware. Return `ProblemDetails` / `ErrorResponse` DTOs. Never expose stack traces
- **JSON**: Snake_case_lower for Auth service (`JsonNamingPolicy.SnakeCaseLower`); other services may vary — check existing conventions
- **Manual mapping**: Static extension methods (`ToDto()`, `ToEntity()`). AutoMapper is banned
- **Validation**: `System.ComponentModel.DataAnnotations` on DTOs. FluentValidation is banned

## 4. Architectural Patterns

### Layer Dependencies
1.  **Domain:** No dependencies. Contains Entities, Enums, Domain Services.
2.  **Application:** Depends on Domain. Contains CQRS Handlers, DTOs, Interfaces, Validators.
3.  **Infrastructure:** Depends on Application and Domain. Implements Repositories, EF Configurations, MassTransit Consumers.
4.  **Api:** Depends on Application and Infrastructure. Entry point (Controllers).

### CQRS (MediatR)
- **Commands:** Mutate state. Return DTOs or Unit.
- **Queries:** Read state. Return DTOs.
- **Handlers:** Implement `IRequestHandler<TRequest, TResponse>`. Keep logic focused on orchestration; delegate business rules to Domain.

### Data Access
- **Repositories:** Define interfaces in `Application`, implement in `Infrastructure`.
- **EF Core:** Use `IEntityTypeConfiguration<T>` for fluent API mapping in `Infrastructure/Data/Configurations`.
- **Transactions:** Use `ExecuteInTransactionAsync` for write operations involving multiple steps.

---

## Banned Libraries (Build Will Fail)

| Banned | Use Instead |
|--------|-------------|
| AutoMapper | Manual mapping extensions |
| FluentValidation | DataAnnotations or manual validation |
| FluentAssertions | Standard xUnit `Assert.*` |
| Swashbuckle/Swagger | Scalar (at `/compensation/scalar`) |
| InMemoryDatabase (EF Core) | Testcontainers with real PostgreSQL |

---

## 5. Testing Guidelines

### Unit Tests (`Maliev.CompensationService.Tests/Unit`)
- Isolate the System Under Test (SUT).
- Use `Moq` for dependencies.
- Naming: `MethodName_StateUnderTest_ExpectedBehavior`.

### Integration Tests (`Maliev.CompensationService.Tests/Integration`)
- **Infrastructure:** Tests require Docker. The project uses `Testcontainers` which spins up real Postgres/RabbitMQ instances.
- **Fixtures:** Use `TestcontainersFixture` and `IClassFixture<WebApplicationFactory<Program>>`.
- **Environment:** Use `Environment.SetEnvironmentVariable` in test setup to override connection strings with container ports.
- **Databases:** Ensure `EnsureCreatedAsync()` is called or migrations applied in test scope before data setup.

### Testing Strategy (4-Tier Pyramid Context)

This service's tests cover **Tier 1 (Unit)** and **Tier 2 (Service Integration)** of the Maliev testing pyramid:

| Tier | What to Test | Infrastructure |
|------|-------------|---------------|
| **Unit** | Business logic, domain models, service methods with mocked dependencies | None (mocks only) |
| **Service Integration** | API endpoints, database persistence, permission enforcement, input validation | `BaseIntegrationTestFactory` + Testcontainers (Postgres/Redis/RabbitMQ) |

**Tier 3 (System Integration)** — cross-service workflows and event chains — is tested in `Maliev.Aspire.Tests/`.

#### Key Rules
- **Framework**: xUnit with standard `Assert` (`Assert.Equal`, `Assert.NotNull`, etc.)
- **Naming**: `MethodName_StateUnderTest_ExpectedBehavior` or `HTTP_METHOD_Path_Scenario_ExpectedStatus`
- **Coverage**: Minimum 80% per service
- **Integration tests**: `BaseIntegrationTestFactory<TProgram, TDbContext>` with Testcontainers (PostgreSQL, Redis, RabbitMQ). Never InMemoryDatabase
- **System tests** (Tier 3): `AspireTestFixture` with `[Collection("AspireDomainTests")]` — shared AppHost, never one per class
- **Eventual consistency**: Use `TestHelpers.WaitForAsync`. Never `Task.Delay`
- **MassTransit consumers**: Must have consumer tests using `AddMassTransitTestHarness()`
- Use `[Fact]` for single cases, `[Theory]` for parameterized tests

> Full ecosystem test strategy: `Maliev.Aspire.Tests/TEST_PLAN.md`

## 6. Agent Behavior Rules

1.  **Proactiveness:** If you fix a bug, add a test case that reproduces it first (Red-Green-Refactor).
2.  **Verification:** NEVER submit code without running `dotnet build`. Run relevant tests (`dotnet test`) for the modified areas.
3.  **Context Awareness:** Read related files (e.g., the Interface when modifying the Implementation, or the DTO when modifying the Controller) to ensure consistency.
4.  **Safety:** When running shell commands, verify the working directory. Do not delete data/files without explicit confirmation unless inside a temporary test context.
5.  **Paths:** Always use absolute paths for file operations.

---

## Mandatory Rules

- **`TreatWarningsAsErrors = true`**: Zero warnings allowed. No suppression
- **`[RequirePermission("compensation.resources.action")]`**: On all endpoints, not plain `[Authorize]`
- **API versioning**: All routes versioned (`v1/`)
- **Service prefix**: Routes prefixed with service domain (`/compensation`)
- **Scalar docs**: Configured at `/compensation/scalar`
- **Secrets**: Never hardcoded. Use GCP Secret Manager or environment variables
- **Async/await**: All the way down. Pass `CancellationToken`
- **EF Core Design package**: Only in Infrastructure project, never in Api
- **PostgreSQL xmin**: Shadow property only — `entity.Property<uint>("xmin").HasColumnType("xid").IsRowVersion()`. Never add entity property
- **Temporary files**: Generate in `/temp` folder, clean up afterwards

## Database & EF Core — Mandatory Rules

### EF Core Design Package
- `Microsoft.EntityFrameworkCore.Design` MUST NOT be in Api projects
- It belongs ONLY in the Infrastructure (or Data) project where migrations live
- Migration commands must target Infrastructure as both project and startup-project:
  ```
  dotnet ef migrations add <Name> --project Maliev.CompensationService.Infrastructure --startup-project Maliev.CompensationService.Infrastructure
  ```

### PostgreSQL xmin Concurrency — Mandatory Pattern
Use shadow property ONLY. Never add a Xmin/xmin property to domain entities.
```csharp
entity.Property<uint>("xmin").HasColumnType("xid").IsRowVersion();
```
- Never use `UseXminAsConcurrencyToken()` (removed in Npgsql EF v7)
- Never use entity property `public uint Xmin { get; set; }` or `public uint xmin { get; set; }`
- Never use `.Ignore(e => e.Xmin)` — remove the entity property instead

---

## Git Rules

- Each `Maliev.*` folder is an independent git repo. Work from within this service directory for git commands
- **Commit early and often** after every meaningful unit of work. Do not accumulate changes
- **Never use `git checkout` to restore files** — commit first, then `git revert` or `git reset --soft`
- Feature branches merged to `develop` via PR. Do not push without being asked

---
*Generated by Antigravity for Maliev.CompensationService*
