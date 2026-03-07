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

Agents must verify all changes using these commands.

### Build
```bash
dotnet build B:\maliev\Maliev.CompensationService\Maliev.CompensationService.slnx
```

### Run All Tests
```bash
dotnet test B:\maliev\Maliev.CompensationService\Maliev.CompensationService.slnx
```

### Run a Single Test
To run a specific test method, use the `--filter` option with the fully qualified name:
```bash
dotnet test --filter "FullyQualifiedName~Maliev.CompensationService.Tests.Integration.Controllers.CompensationControllerTests.GetCompensationDetails_ShouldReturnOk_WhenEmployeeExists"
```
*Note: Use `~` for "contains" matching if the full name is complex.*

### Formatting & Linting
If `dotnet format` is available/configured:
```bash
dotnet format B:\maliev\Maliev.CompensationService\Maliev.CompensationService.slnx
```
Otherwise, ensure adherence to standard .NET conventions manually.

## 3. Code Style & Conventions

### General
- **Namespaces:** Use file-scoped namespaces (e.g., `namespace Maliev.CompensationService.Domain.Entities;`).
- **Indentation:** 4 spaces.
- **Braces:** Allman style (braces on new lines).
- **Comments:** Provide XML documentation (`///`) for all public classes, interfaces, and methods.

### Naming
- **Classes/Methods/Properties:** PascalCase (e.g., `CompensationRecord`, `CalculateTotal`).
- **Parameters/Locals:** camelCase (e.g., `employeeId`, `baseSalary`).
- **Private Fields:** camelCase with underscore prefix (e.g., `_repository`, `_logger`).
- **Interfaces:** Prefix with 'I' (e.g., `ICompensationRepository`).
- **Async Methods:** Suffix with 'Async' (e.g., `GetByEmployeeIdAsync`).

### Dependencies & Imports
- **Usings:** Place `using` directives at the very top of the file. Remove unused directives.
- **Injection:** Use Constructor Injection. Assign to `readonly` private fields.

### Error Handling
- Use exceptions for exceptional flow, but prefer Result/Option patterns if established in `Common`.
- In Application layer, handle domain validation errors and return appropriate responses or exceptions caught by middleware.

## 4. Architectural Patterns

### Layer Dependencies
1.  **Domain:** No dependencies. Contains Entities, Enums, Domain Services.
2.  **Application:** Depends on Domain. Contains CQRS Handlers, DTOs, Interfaces, Validators.
3.  **Infrastructure:** Depends on Application and Domain. Implements Repositories, EF Configurations, MassTransit Consumers.
4.  **Api:** Depends on Application and Infrastructure. Entry point (Controllers).

### CQRS (MediatR)
- **Commands:** Mutate state. Return DTOs or Unit.
- **Queries:** Read state. Return DTOs.
- **Handlers:** Implement `IRequestHandler<TRequest, TResponse>`. keep logic focused on orchestration; delegate business rules to Domain.

### Data Access
- **Repositories:** Define interfaces in `Application`, implement in `Infrastructure`.
- **EF Core:** Use `IEntityTypeConfiguration<T>` for fluent API mapping in `Infrastructure/Data/Configurations`.
- **Transactions:** Use `ExecuteInTransactionAsync` for write operations involving multiple steps.

## 5. Testing Guidelines

### Unit Tests (`Maliev.CompensationService.Tests/Unit`)
- Isolate the System Under Test (SUT).
- Use `Moq` for dependencies.
- Naming: `MethodName_ShouldExpectedResult_WhenCondition`.

### Integration Tests (`Maliev.CompensationService.Tests/Integration`)
- **Infrastructure:** Tests require Docker. The project uses `Testcontainers` which spins up real Postgres/RabbitMQ instances.
- **Fixtures:** Use `TestcontainersFixture` and `IClassFixture<WebApplicationFactory<Program>>`.
- **Environment:** Use `Environment.SetEnvironmentVariable` in test setup to override connection strings with container ports.
- **Databases:** Ensure `EnsureCreatedAsync()` is called or migrations applied in test scope before data setup.

## 6. Agent Behavior Rules

1.  **Proactiveness:** If you fix a bug, add a test case that reproduces it first (Red-Green-Refactor).
2.  **Verification:** NEVER submit code without running `dotnet build`. Run relevant tests (`dotnet test`) for the modified areas.
3.  **Context Awareness:** Read related files (e.g., the Interface when modifying the Implementation, or the DTO when modifying the Controller) to ensure consistency.
4.  **Safety:** When running shell commands, verify the working directory. Do not delete data/files without explicit confirmation unless inside a temporary test context.
5.  **Paths:** Always use absolute paths for file operations.

---
*Generated by Antigravity for Maliev.CompensationService*


## Database & EF Core — Mandatory Rules

### EF Core Design Package
- ❌ `Microsoft.EntityFrameworkCore.Design` MUST NOT be in Api projects
- ✅ It belongs ONLY in the Infrastructure (or Data) project where migrations live
- Migration commands must target Infrastructure, not Api:
  ```
  dotnet ef migrations add <Name> --project Maliev.<Domain>Service.Infrastructure --startup-project ../Maliev.<Domain>Service.Api
  ```

### PostgreSQL xmin Concurrency — Mandatory Pattern
Use shadow property ONLY. Never add a Xmin/xmin property to domain entities.
```csharp
entity.Property<uint>("xmin").HasColumnType("xid").IsRowVersion();
```
- ❌ Never use `UseXminAsConcurrencyToken()` (removed in Npgsql EF v7)
- ❌ Never use entity property `public uint Xmin { get; set; }` or `public uint xmin { get; set; }`
- ❌ Never use `.Ignore(e => e.Xmin)` — remove the entity property instead
