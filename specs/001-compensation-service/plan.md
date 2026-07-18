# Implementation Plan: Employee Compensation Management Service

**Branch**: `001-compensation-service` | **Date**: 2025-12-28 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-compensation-service/spec.md`

## Summary

The Compensation Service is a high-security microservice for managing employee compensation, salary records, bonuses, commissions, and benefits enrollment. It handles sensitive financial data with encryption at rest, comprehensive audit trails, and strict permission-based access control. The service integrates with the Employee Service via events and supports bulk operations for organization-wide salary adjustments.

## Technical Context

**Language/Version**: .NET 10.0
**Framework**: ASP.NET Core 10.0
**Primary Dependencies**: Entity Framework Core 10.x, MassTransit, Maliev.Aspire.ServiceDefaults
**Storage**: PostgreSQL 18
**Caching**: Redis 7.x
**Messaging**: RabbitMQ + MassTransit
**Testing**: xUnit with Testcontainers (PostgreSQL, RabbitMQ, Redis)
**Encryption**: AES-256-GCM via IEncryptionService
**Validation**: Data Annotations (.NET Native)
**Target Platform**: Linux containers (GKE)
**Project Type**: Microservice API
**Performance Goals**: <2s view, <5s updates, 100 concurrent users, <10min bulk ops (500 employees)
**Constraints**: 99.9% uptime, zero salary data in logs, encryption at rest for all financial data
**Scale/Scope**: 10,000+ employees, ~70-80 C# files, ~12,000 LOC

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Service Autonomy ✅
- **Own database**: PostgreSQL instance for compensation data
- **Own domain logic**: Compensation calculations, benefits enrollment, bulk operations
- **Event-based integration**: Consumes EmployeeCreated/EmployeeTerminated, publishes CompensationChanged/BenefitsEnrollmentUpdated
- **No direct database access**: Interacts with Employee Service via events only

### Explicit Contracts ✅
- **OpenAPI/Scalar**: All endpoints documented
- **Versioned contracts**: API v1 with backward-compatible migrations
- **Event schemas**: Defined for all published/consumed events

### Test-First Development ✅
- **Tests before implementation**: Unit, integration, contract tests
- **Red-Green-Refactor**: Mandatory workflow
- **80%+ coverage**: For business-critical logic (compensation calculations, encryption)

### Real Infrastructure Testing ✅
- **PostgreSQL**: Testcontainers for all database tests
- **RabbitMQ**: Testcontainers for event consumers/publishers
- **Redis**: Testcontainers for caching tests
- **No in-memory substitutes**: All tests use real infrastructure

### Auditability & Observability ✅
- **Structured JSON logging**: With correlation IDs, user IDs, operation names
- **Audit logs**: Separate from operational logs, retained indefinitely
- **Health checks**: Liveness and readiness endpoints
- **Log level configuration**: Follows constitution mandated levels
- **Metrics**: Business metrics (compensation changes, enrollments) + system metrics (latency, errors)
- **Distributed tracing**: Trace IDs across all operations

### Security & Compliance ✅
- **JWT authentication**: Via AddJwtAuthentication
- **Permission-based authorization**: Read, Update, Admin, Reports permissions
- **Encryption at rest**: Base salary, previous/new salary, dependent national IDs
- **Encryption in transit**: HTTPS mandatory
- **No salary data in logs**: Explicit filtering

### Secrets Management ✅
- **Google Secret Manager**: Via builder.AddGoogleSecretManagerVolume()
- **Encryption keys**: Retrieved from configuration, not hardcoded
- **No secrets in source**: All credentials externalized

### Zero Warnings Policy ✅
- **Build configuration**: Warnings treated as errors
- **Clean builds**: No warnings allowed

### Clean Project Artifacts ✅
- **.gitignore**: Excludes temporary files
- **.dockerignore**: Excludes build artifacts, specs, IDE files, Test projects
- **No additional markdown**: Only README.md at root
- **CODEOWNERS**: Mandatory at .github/CODEOWNERS

### Docker Best Practices ✅
- **Dockerfile location**: Maliev.CompensationService.Api/Dockerfile
- **Built-in app user**: No custom user creation
- **Multi-stage build**: SDK for build, ASP.NET runtime for final
- **.NET 10 images**: mcr.microsoft.com/dotnet/sdk:10.0 and aspnet:10.0
- **BuildKit secrets**: For NuGet credentials
- **Health check**: Validates liveness endpoint
- **Port 8080**: EXPOSE 8080, ENV ASPNETCORE_URLS=http://+:8080

### .NET Aspire Integration ✅
- **ServiceDefaults as NuGet**: From GitHub Packages
- **nuget.config**: At repository root with credential placeholders
- **CI/CD authentication**: GITOPS_PAT for NuGet restore
- **BuildKit secrets**: In Dockerfile for NuGet credentials
- **Program.cs calls**: builder.AddServiceDefaults() and app.MapDefaultEndpoints()

### Code Quality & Library Standards ✅
- **NO AutoMapper**: Explicit mapping only
- **NO FluentValidation**: Data Annotations for validation
- **NO FluentAssertions**: Standard xUnit Assert

### Project Structure & Naming ✅
- **Flat structure**: Projects at repository root (no /src or /tests folders)
- **Naming convention**: Maliev.CompensationService.Api (full company prefix)
- **Dockerfile placement**: Inside API project folder

### CI/CD Standards ✅
- **Workflow filenames**: ci-develop.yml, ci-staging.yml, ci-main.yml
- **No docker-compose**: Testcontainers for all integration tests

### Business Metrics & Analytics ✅
- **Business metrics**: Compensation changes/day, enrollments/day, bulk ops completed
- **System metrics**: Latency (p50, p95, p99), error rates, concurrent users
- **Structured format**: Compatible with Prometheus/OpenTelemetry
- **Metric tags**: service_name, version, region, environment
- **Tests**: Validate metrics endpoint presence and format

### Complexity Tracking

No violations - all constitution requirements met.

## Project Structure

### Documentation (this feature)

```text
specs/001-compensation-service/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
│   ├── openapi.yaml
│   └── events/
│       ├── compensation-changed.json
│       ├── benefits-enrollment-updated.json
│       ├── bulk-salary-increase-completed.json
│       ├── employee-created.json
│       └── employee-terminated.json
└── tasks.md             # Phase 2 output (created by /speckit.tasks)
```

### Source Code (repository root)

```text
Maliev.CompensationService/
├── .github/
│   ├── CODEOWNERS
│   └── workflows/
│       ├── ci-develop.yml
│       ├── ci-staging.yml
│       └── ci-main.yml
├── Maliev.CompensationService.Api/
│   ├── Controllers/
│   │   ├── CompensationController.cs
│   │   ├── BenefitsController.cs
│   │   ├── BulkOperationsController.cs
│   │   └── ReportsController.cs
│   ├── Filters/
│   │   └── SalaryLoggingFilter.cs
│   ├── Dockerfile
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
├── Maliev.CompensationService.Application/
│   ├── Commands/
│   │   ├── RecordCompensationChangeCommand.cs
│   │   ├── UpdateBenefitsEnrollmentCommand.cs
│   │   ├── EnrollInBenefitCommand.cs
│   │   ├── TerminateBenefitCommand.cs
│   │   ├── BulkSalaryIncreaseCommand.cs
│   │   └── Handlers/
│   │       ├── RecordCompensationChangeCommandHandler.cs
│   │       ├── UpdateBenefitsEnrollmentCommandHandler.cs
│   │       ├── EnrollInBenefitCommandHandler.cs
│   │       ├── TerminateBenefitCommandHandler.cs
│   │       └── BulkSalaryIncreaseCommandHandler.cs
│   ├── Queries/
│   │   ├── GetCompensationDetailsQuery.cs
│   │   ├── GetCompensationHistoryQuery.cs
│   │   ├── GetAvailableBenefitsQuery.cs
│   │   ├── GetEmployeeBenefitsQuery.cs
│   │   ├── GetBulkJobStatusQuery.cs
│   │   ├── GetCompensationAnalysisQuery.cs
│   │   └── Handlers/
│   │       ├── GetCompensationDetailsQueryHandler.cs
│   │       ├── GetCompensationHistoryQueryHandler.cs
│   │       ├── GetAvailableBenefitsQueryHandler.cs
│   │       ├── GetEmployeeBenefitsQueryHandler.cs
│   │       ├── GetBulkJobStatusQueryHandler.cs
│   │       └── GetCompensationAnalysisQueryHandler.cs
│   ├── DTOs/
│   │   ├── CompensationRecordDto.cs
│   │   ├── RecordCompensationChangeDto.cs
│   │   ├── SalaryHistoryDto.cs
│   │   ├── BenefitDto.cs
│   │   ├── BenefitsEnrollmentDto.cs
│   │   ├── UpdateBenefitsEnrollmentDto.cs
│   │   ├── DependentDto.cs
│   │   ├── BulkSalaryIncreaseDto.cs
│   │   ├── BulkJobStatusDto.cs
│   │   └── CompensationAnalysisDto.cs
│   ├── Interfaces/
│   │   ├── ICompensationRepository.cs
│   │   ├── IBenefitsRepository.cs
│   │   ├── ISalaryHistoryRepository.cs
│   │   ├── IBulkJobRepository.cs
│   │   └── IEncryptionService.cs
│   └── Mappers/
│       ├── CompensationMapper.cs
│       ├── BenefitsMapper.cs
│       └── SalaryHistoryMapper.cs
├── Maliev.CompensationService.Domain/
│   ├── Entities/
│   │   ├── CompensationRecord.cs
│   │   ├── SalaryHistory.cs
│   │   ├── Benefit.cs
│   │   ├── BenefitsEnrollment.cs
│   │   ├── Dependent.cs
│   │   └── BulkJob.cs
│   ├── Enums/
│   │   ├── CompensationType.cs
│   │   ├── BenefitType.cs
│   │   ├── EnrollmentStatus.cs
│   │   ├── DependentRelationship.cs
│   │   ├── BulkJobType.cs
│   │   └── BulkJobStatus.cs
│   ├── Events/
│   │   ├── CompensationChangedEvent.cs
│   │   ├── BenefitsEnrollmentUpdatedEvent.cs
│   │   ├── BulkSalaryIncreaseCompletedEvent.cs
│   │   ├── EmployeeCreatedEvent.cs
│   │   └── EmployeeTerminatedEvent.cs
│   └── Authorization/
│       └── CompensationPermissions.cs
├── Maliev.CompensationService.Infrastructure/
│   ├── Data/
│   │   ├── CompensationDbContext.cs
│   │   ├── EncryptionValueConverter.cs
│   │   └── Configurations/
│   │       ├── CompensationRecordConfiguration.cs
│   │       ├── SalaryHistoryConfiguration.cs
│   │       ├── BenefitConfiguration.cs
│   │       ├── BenefitsEnrollmentConfiguration.cs
│   │       ├── DependentConfiguration.cs
│   │       └── BulkJobConfiguration.cs
│   ├── Migrations/
│   │   └── [EF Core migrations]
│   ├── Repositories/
│   │   ├── CompensationRepository.cs
│   │   ├── BenefitsRepository.cs
│   │   ├── SalaryHistoryRepository.cs
│   │   └── BulkJobRepository.cs
│   ├── Services/
│   │   ├── EncryptionService.cs
│   │   └── CompensationIAMRegistrationService.cs
│   └── Consumers/
│       ├── EmployeeCreatedEventConsumer.cs
│       └── EmployeeTerminatedEventConsumer.cs
├── Maliev.CompensationService.Tests/
│   ├── Unit/
│   │   ├── Commands/
│   │   │   └── [Command handler unit tests]
│   │   ├── Queries/
│   │   │   └── [Query handler unit tests]
│   │   ├── Services/
│   │   │   └── EncryptionServiceTests.cs
│   │   └── Mappers/
│   │       └── [Mapper tests]
│   ├── Integration/
│   │   ├── Controllers/
│   │   │   ├── CompensationControllerTests.cs
│   │   │   ├── BenefitsControllerTests.cs
│   │   │   ├── BulkOperationsControllerTests.cs
│   │   │   └── ReportsControllerTests.cs
│   │   ├── Consumers/
│   │   │   ├── EmployeeCreatedEventConsumerTests.cs
│   │   │   └── EmployeeTerminatedEventConsumerTests.cs
│   │   ├── Repositories/
│   │   │   └── [Repository tests]
│   │   └── TestContainersFixture.cs
│   └── Contract/
│       ├── OpenApiSchemaTests.cs
│       └── EventSchemaTests.cs
├── nuget.config
├── .gitignore
├── .dockerignore
├── README.md
└── Maliev.CompensationService.sln
```

**Structure Decision**: Flat structure with 4 projects at repository root following Constitution XV. API project contains controllers and Dockerfile. Application layer implements CQRS pattern with commands/queries. Domain contains entities and events. Infrastructure handles data access, encryption, and event consumers. Tests project uses Testcontainers for real infrastructure.

## Tech Stack

| Component | Technology | Version | Rationale |
|-----------|------------|---------|-----------|
| Runtime | .NET | 10.0 | Latest LTS with performance improvements |
| Framework | ASP.NET Core | 10.0 | Built-in minimal APIs, dependency injection |
| Database | PostgreSQL | 18 | ACID compliance, encryption support, JSON support |
| ORM | Entity Framework Core | 10.x | Native .NET ORM with value converters for encryption |
| Messaging | RabbitMQ + MassTransit | Latest | Reliable event delivery, at-least-once semantics |
| Caching | Redis | 7.x | High-performance caching for benefits catalog |
| Encryption | AES-256-GCM | .NET Native | Strong encryption for financial data at rest |
| Validation | Data Annotations | .NET Native | Built-in, sufficient per Constitution XIV |
| Testing | xUnit + Testcontainers | Latest | Real infrastructure testing per Constitution IV |
| Observability | Maliev.Aspire.ServiceDefaults | Latest | Standardized logging, metrics, tracing |
| Authentication | JWT | Via ServiceDefaults | Stateless authentication |
| Authorization | Permission-based | Via ServiceDefaults | Fine-grained access control |

## Architecture Patterns

### CQRS (Command Query Responsibility Segregation)
- **Commands**: RecordCompensationChange, UpdateBenefitsEnrollment, BulkSalaryIncrease
- **Queries**: GetCompensationDetails, GetCompensationHistory, GetAvailableBenefits, GetCompensationAnalysis
- **Rationale**: Separates read/write concerns, enables independent optimization, aligns with audit requirements

### Repository Pattern
- **Repositories**: ICompensationRepository, IBenefitsRepository, ISalaryHistoryRepository, IBulkJobRepository
- **Rationale**: Abstracts data access, enables testing with Testcontainers, encapsulates encryption logic

### Event-Driven Architecture
- **Published Events**: CompensationChanged, BenefitsEnrollmentUpdated, BulkSalaryIncreaseCompleted
- **Consumed Events**: EmployeeCreated, EmployeeTerminated
- **Rationale**: Loose coupling with Employee Service, eventual consistency, audit trail

### Value Converter Pattern (EF Core)
- **EncryptionValueConverter**: Automatically encrypts/decrypts sensitive fields
- **Fields**: BaseSalary, PreviousSalary, NewSalary, NationalId
- **Rationale**: Transparent encryption/decryption, single responsibility, prevents accidental leaks

### Circuit Breaker Pattern
- **Critical Services**: Encryption Service, Authorization Service (fail-fast)
- **Non-Critical Services**: Event Bus (retry with exponential backoff)
- **Rationale**: Prevents cascading failures, aligns with 99.9% uptime requirement (SC-011)

### Optimistic Locking
- **Entity**: CompensationRecord
- **Mechanism**: Row version/timestamp column
- **Rationale**: Handles concurrent updates, prevents lost updates, aligns with clarification Q1

## Database Design

### Tables

1. **compensation_records**: Employee compensation snapshots
   - Indexed: employee_id, (employee_id, is_current)
   - Encrypted: base_salary_encrypted (TEXT)
   - Constraint: Only one is_current=true per employee_id

2. **salary_histories**: Audit trail of compensation changes
   - Indexed: employee_id, compensation_record_id
   - Encrypted: previous_salary_encrypted, new_salary_encrypted (TEXT)
   - Cascade: References compensation_records

3. **benefits**: Available benefits catalog
   - Cached in Redis (24h TTL)
   - Indexed: is_active

4. **benefits_enrollments**: Employee benefit enrollments
   - Indexed: employee_id, status
   - Cascade: Dependents deleted on enrollment deletion

5. **dependents**: Family members on benefit plans
   - Indexed: benefits_enrollment_id
   - Encrypted: national_id_encrypted (TEXT)

6. **bulk_jobs**: Async bulk operation tracking
   - Indexed: status, started_by
   - JSONB: parameters, error_details

### Encryption Strategy

**Value Converter Approach**:
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    var converter = new EncryptionValueConverter(_encryptionService);

    modelBuilder.Entity<CompensationRecord>()
        .Property(e => e.BaseSalary)
        .HasColumnName("base_salary_encrypted")
        .HasConversion(converter);

    // Similar for SalaryHistory.PreviousSalary, NewSalary, Dependent.NationalId
}
```

**Rationale**: EF Core value converters provide transparent encryption/decryption, type safety, and prevent accidental exposure of unencrypted data.

## API Design

### REST Principles
- **Base Path**: `/compensation/v1`
- **Resource-oriented**: /employees/{employeeId}/compensation, /benefits
- **HTTP Methods**: GET (read), POST (create), PUT (update), DELETE (terminate)
- **Status Codes**: 200 (OK), 201 (Created), 202 (Accepted), 400 (Bad Request), 403 (Forbidden), 404 (Not Found), 409 (Conflict)

### Async Operations (Bulk)
- **Pattern**: Submit → Job ID → Poll Status → Webhook (optional)
- **Endpoint**: POST /bulk/salary-increase → 202 Accepted + job ID
- **Status**: GET /bulk/jobs/{jobId} → polling endpoint
- **Webhook**: Optional HTTP POST on completion
- **Rationale**: Aligns with clarification Q3, prevents timeout for long operations

### Versioning
- **Strategy**: URL versioning (/v1/)
- **Rationale**: Explicit, simple, backward compatibility clear

## Security Implementation

### Encryption Service
```csharp
public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}
```

**Implementation**: AES-256-GCM with IV prepended to ciphertext, key from configuration

### Logging Filter
- **SalaryLoggingFilter**: Marks salary fields as [SENSITIVE - Not logged]
- **Audit Logger**: Separate logger for audit events (no salary amounts)
- **Correlation IDs**: Every request tagged with unique ID

### Permission Checks
```csharp
[RequirePermission(CompensationPermissions.Read,
    ResourcePathTemplate = "employee/{employeeId}",
    IsCritical = true,
    AuditPurpose = "Compensation Data Access")]
```

**Permissions**: Read, Update, Admin, Reports

## Observability Implementation

### Structured Logging
- **Format**: JSON with fields: timestamp, correlationId, userId, operation, outcome
- **Levels**: DEBUG, INFO, WARN, ERROR, CRITICAL
- **Separation**: Audit logs separate from operational logs

### Metrics (via ServiceDefaults)
- **Business**: compensation_changes_total, benefits_enrollments_total, bulk_operations_total
- **System**: http_request_duration_seconds (p50/p95/p99), http_requests_errors_total
- **External**: circuit_breaker_state, external_service_latency_seconds, event_queue_depth

### Distributed Tracing
- **Trace ID**: Propagated across all service calls
- **Spans**: External service calls, database queries, encryption operations, event publishing
- **Context**: operation, employeeId (anonymized), duration, status

### Health Checks
- **Liveness**: /compensation/liveness → 200 if running
- **Readiness**: /compensation/readiness → 200 if dependencies available

## Event Contracts

### Published Events

**CompensationChangedEvent**:
```json
{
  "employeeId": "uuid",
  "compensationRecordId": "uuid",
  "newSalary": 92000.00,
  "previousSalary": 85000.00,
  "changePercentage": 8.24,
  "effectiveDate": "2025-04-01T00:00:00Z",
  "changeReason": "Promotion to Senior Engineer"
}
```

**BenefitsEnrollmentUpdatedEvent**:
```json
{
  "employeeId": "uuid",
  "benefitId": "uuid",
  "status": "Active",
  "effectiveDate": "2025-01-01T00:00:00Z"
}
```

**BulkSalaryIncreaseCompletedEvent**:
```json
{
  "jobId": "uuid",
  "successCount": 450,
  "failureCount": 0,
  "totalBudgetImpact": 125000.00
}
```

### Consumed Events

**EmployeeCreatedEvent**:
```json
{
  "employeeId": "uuid",
  "employeeNumber": "EMP-001",
  "startDate": "2025-01-15T00:00:00Z",
  "departmentId": "uuid"
}
```

**EmployeeTerminatedEvent**:
```json
{
  "employeeId": "uuid",
  "terminationDate": "2025-12-31T00:00:00Z"
}
```

## Testing Strategy

### Unit Tests (~30 tests)
- Command handlers (compensation changes, benefits enrollment)
- Query handlers (details, history, analysis)
- Encryption service
- Mappers (entity ↔DTO)
- **Coverage Target**: 80%+ for business logic

### Integration Tests (~25 tests)
- **Testcontainers**: PostgreSQL, RabbitMQ, Redis
- Controller endpoints with real auth/authz
- Event consumers (EmployeeCreated, EmployeeTerminated)
- Repository operations with encryption
- Bulk operations end-to-end
- **Focus**: Data flow, encryption, concurrency, events

### Contract Tests (~5 tests)
- OpenAPI schema validation
- Event schema validation (published/consumed)
- **Focus**: API/event contract stability

## CI/CD Pipeline

### Build Workflow (ci-develop.yml, ci-staging.yml, ci-main.yml)
1. **Restore**: dotnet restore with GITOPS_PAT for ServiceDefaults NuGet
2. **Build**: dotnet build --no-restore with warnings as errors
3. **Test**: dotnet test with Testcontainers (PostgreSQL, RabbitMQ, Redis)
4. **Docker Build**: Using BuildKit secrets for NuGet credentials
5. **Push**: To container registry
6. **Deploy**: Via maliev-gitops repository

### Resource Limits
**Development**:
- CPU: 10m request, 25m limit
- Memory: 96Mi request, 128Mi limit

**Production**:
- CPU: 15m request, 40m limit
- Memory: 128Mi request, 192Mi limit

## Deployment Configuration

### Environment Variables
- **Encryption__Key**: Base64-encoded AES-256 key (from Secret Manager)
- **ConnectionStrings__CompensationDbContext**: PostgreSQL connection
- **Redis__ConnectionString**: Redis connection
- **RabbitMQ__Host**: RabbitMQ host
- **ExternalServices__EmployeeService__BaseUrl**: Employee Service URL

### Health Check Configuration
- **Liveness**: /compensation/liveness (every 10s)
- **Readiness**: /compensation/readiness (every 5s, initialDelay 10s)

## Data Migration Plan

### From Employee Service
1. **Export**: Encrypted data from employee service database
2. **Re-encrypt** (if needed): Decrypt with old key, re-encrypt with new key
3. **Import**: COPY to compensation_records table
4. **Verify**: Row counts, encryption, referential integrity

### Initial Seed Data
- **Benefits**: Seed common benefits (Health, Dental, Vision, Life, 401k)
- **Configuration**: Default waiting periods (90 days)

## Performance Optimization

### Caching Strategy
- **Benefits catalog**: Redis cache, 24h TTL (rarely changes)
- **NOT cached**: Employee-specific compensation (too many keys, stale data risk)

### Query Optimization
- **AsNoTracking**: For read-only queries (GetCompensationDetails, GetCompensationHistory)
- **Indexes**: employee_id, (employee_id, is_current), status
- **Projections**: Select only needed columns for reports

### Bulk Operations
- **Batch size**: 100 employees per transaction
- **Async processing**: Background job with status tracking
- **Timeout**: 10 minutes max (SC-007)

## Risk Mitigation

| Risk | Impact | Mitigation |
|------|--------|-----------|
| Encryption key compromise | HIGH | Rotate keys within 24h, re-encrypt all data |
| Concurrent updates lost | MEDIUM | Optimistic locking with conflict detection |
| Bulk operation timeout | MEDIUM | Job status tracking, webhook notifications |
| External service failure | HIGH | Circuit breaker (critical), retry (non-critical) |
| Salary data in logs | CRITICAL | SalaryLoggingFilter, audit log separation |

## Estimated Effort

- **Files**: ~70-80 C# files
- **Lines of Code**: ~12,000 LOC
- **Controllers**: 4 (Compensation, Benefits, BulkOperations, Reports)
- **Commands**: 5 (RecordCompensationChange, UpdateBenefitsEnrollment, EnrollInBenefit, TerminateBenefit, BulkSalaryIncrease)
- **Queries**: 6 (GetCompensationDetails, GetCompensationHistory, GetAvailableBenefits, GetEmployeeBenefits, GetBulkJobStatus, GetCompensationAnalysis)
- **Event Consumers**: 2 (EmployeeCreated, EmployeeTerminated)
- **Tests**: ~60 tests (30 unit, 25 integration, 5 contract)

## Next Steps

1. **Phase 0**: Research (resolve any unknowns) → research.md
2. **Phase 1**: Design (data model, contracts, quickstart) → data-model.md, /contracts/, quickstart.md
3. **Phase 2**: Tasks (generate tasks.md via /speckit.tasks)
4. **Implementation**: Test-first development, Red-Green-Refactor
5. **Deployment**: Via maliev-gitops repository
