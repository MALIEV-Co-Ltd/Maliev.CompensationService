# Research & Technical Decisions: Employee Compensation Management Service

**Date**: 2025-12-28
**Feature**: Employee Compensation Management Service
**Related**: [plan.md](./plan.md) | [spec.md](./spec.md)

## Overview

This document captures the research and technical decisions made for the Compensation Service implementation. All decisions align with the MALIEV Microservices Constitution and clarifications from the specification phase.

## Key Technical Decisions

### 1. Encryption Strategy

**Decision**: EF Core Value Converters for transparent encryption/decryption

**Rationale**:
- **Type Safety**: Compile-time checking ensures encrypted fields are handled correctly
- **Transparency**: Encryption/decryption happens automatically at the ORM layer
- **Single Responsibility**: Encryption logic encapsulated in one place (EncryptionValueConverter)
- **Prevention**: Impossible to accidentally expose unencrypted salary data in queries or logs

**Alternatives Considered**:
- **Manual encryption in repositories**: Rejected - error-prone, scattered logic, easy to forget
- **Database-level encryption (TDE)**: Rejected - doesn't prevent in-memory exposure, all-or-nothing approach
- **Application-level before save**: Rejected - requires discipline at every save point, easy to miss

**Implementation**:
```csharp
public class EncryptionValueConverter : ValueConverter<decimal, string>
{
    public EncryptionValueConverter(IEncryptionService encryptionService)
        : base(
            v => encryptionService.Encrypt(v.ToString()),
            v => decimal.Parse(encryptionService.Decrypt(v)))
    { }
}
```

**References**:
- EF Core Value Converters: https://learn.microsoft.com/en-us/ef/core/modeling/value-converters
- Constitution VI: Security & Compliance

---

### 2. Concurrent Update Handling

**Decision**: Optimistic locking with row version

**Rationale**:
- **Performance**: No locks held during user think time
- **Scalability**: Supports 100 concurrent users (SC-004)
- **User Experience**: Only conflicts get errors, successful updates are fast
- **Industry Standard**: Common pattern for financial systems

**Alternatives Considered**:
- **Pessimistic locking**: Rejected - poor UX (users must wait), doesn't scale, deadlock risk
- **Last write wins**: Rejected - data loss, unacceptable for financial data
- **Queue-based**: Rejected - unnecessary complexity for rare conflicts

**Implementation**:
```csharp
public class CompensationRecord
{
    [Timestamp]
    public byte[] RowVersion { get; set; }
    // ...
}
```

**References**:
- Clarification Q1: Optimistic Locking chosen
- EF Core Concurrency Tokens: https://learn.microsoft.com/en-us/ef/core/saving/concurrency

---

### 3. External Service Failure Handling

**Decision**: Circuit Breaker for critical services, Retry for non-critical

**Rationale**:
- **Prevents Cascading Failures**: Circuit breaker opens after consecutive failures
- **Fast Failure**: Critical operations (encryption, auth) fail immediately when unavailable
- **Eventual Consistency**: Non-critical operations (events) retry with backoff
- **99.9% Uptime**: Aligns with SC-011 availability requirement

**Alternatives Considered**:
- **Fail fast for all**: Rejected - loses events, breaks eventual consistency
- **Retry all indefinitely**: Rejected - cascading failures, resource exhaustion
- **Cache stale data**: Rejected - unacceptable for financial data accuracy

**Implementation**:
- **Critical Services**: Encryption, Authorization → AddStandardResilienceHandler (circuit breaker)
- **Non-Critical Services**: Event Bus → Retry with exponential backoff (2s, 4s, 8s, 16s, max 30s)
- **Queued Events**: Failed event publications stored locally, retried when bus available

**References**:
- Clarification Q2: Graceful Degradation with Circuit Breaker
- Polly Resilience: https://www.pollydocs.org/strategies/circuit-breaker

---

### 4. Bulk Operation Notification

**Decision**: Polling with optional webhook

**Rationale**:
- **Simplicity**: No real-time infrastructure required initially
- **Flexibility**: Users check status when convenient
- **Automation**: Optional webhook for advanced users/integrations
- **Scalability**: Webhook reduces polling load

**Alternatives Considered**:
- **Real-time push (SignalR)**: Rejected - complex infrastructure, overkill for 10min operations
- **Email only**: Rejected - not programmatic, requires manual monitoring
- **In-app notification queue**: Rejected - requires user to be logged in

**Implementation**:
- **Submit**: POST /bulk/salary-increase → 202 Accepted + {jobId, status, affectedEmployees}
- **Poll**: GET /bulk/jobs/{jobId} → {jobId, status, successCount, failureCount, completedAt}
- **Webhook**: POST to configured URL with {jobId, status, successCount, failureCount, timestamp}

**References**:
- Clarification Q3: Polling with Optional Webhook
- SC-007: Bulk operations complete within 10 minutes

---

### 5. Observability Stack

**Decision**: Structured Logging + Key Metrics + Distributed Tracing

**Rationale**:
- **Comprehensive**: Covers logs, metrics, traces for full observability
- **Industry Standard**: OpenTelemetry-compatible, Prometheus metrics
- **Debugging**: Distributed traces enable root cause analysis across services
- **SLA Support**: Metrics enable proactive monitoring for 99.9% uptime (SC-011)

**Alternatives Considered**:
- **Basic logging only**: Rejected - insufficient for SLA monitoring, no metrics
- **Metrics-focused**: Rejected - missing context for debugging failures
- **Full APM with anomaly detection**: Deferred - can add later, not MVP requirement

**Implementation**:
- **Logs**: JSON format via Maliev.Aspire.ServiceDefaults
- **Metrics**: compensation_changes_total, http_request_duration_seconds (p50/p95/p99)
- **Traces**: Trace ID per request, spans for DB/external calls

**References**:
- Clarification Q4: Structured Logging + Key Metrics + Distributed Tracing
- Constitution V: Auditability & Observability
- Constitution XII: Business Metrics & Analytics

---

### 6. Terminated Employee with Pending Benefits

**Decision**: Auto-reject pending enrollments, terminate active enrollments

**Rationale**:
- **Business Logic**: Terminated employees don't receive new benefits
- **Clean State**: No orphaned pending records
- **Consistency**: Clear separation between active (terminate) and pending (reject)

**Alternatives Considered**:
- **Terminate all**: Rejected - conflates two different states (active vs pending)
- **Grace period**: Rejected - complex business logic, edge cases
- **Manual review**: Rejected - blocks termination workflow, requires HR intervention

**Implementation**:
```csharp
// EmployeeTerminatedEventConsumer
await _benefitsRepository.TerminateActiveEnrollments(employeeId, terminationDate);
await _benefitsRepository.RejectPendingEnrollments(employeeId, "Employee terminated");
```

**References**:
- Clarification Q5: Auto-Reject Pending, Terminate Active
- FR-048, FR-049: Automatic rejection/termination requirements

---

### 7. CQRS Pattern

**Decision**: Implement CQRS with separate command/query handlers

**Rationale**:
- **Separation of Concerns**: Read logic (queries) vs write logic (commands)
- **Optimization**: Read queries optimized with AsNoTracking, projections
- **Audit Trail**: Commands naturally enforce audit logging
- **Testability**: Handlers testable independently

**Alternatives Considered**:
- **Traditional repository CRUD**: Rejected - mixes read/write concerns
- **MediatR library**: Rejected per Constitution XIV (avoid unnecessary libraries)

**Implementation**:
- **Commands**: RecordCompensationChangeCommand, UpdateBenefitsEnrollmentCommand, BulkSalaryIncreaseCommand
- **Queries**: GetCompensationDetailsQuery, GetCompensationHistoryQuery, GetCompensationAnalysisQuery
- **Handlers**: One handler per command/query, registered as scoped services

**References**:
- CQRS Pattern: https://martinfowler.com/bliki/CQRS.html
- Constitution XI: Simplicity & Maintainability

---

### 8. Repository Pattern

**Decision**: Use repository interfaces for data access abstraction

**Rationale**:
- **Testability**: Enables Testcontainers for integration tests
- **Encapsulation**: Encryption logic hidden behind repository
- **Flexibility**: Can swap implementations (e.g., caching layer)

**Complexity Justification**: Repository pattern adds one abstraction layer but is justified because:
- **Testcontainers Requirement**: Constitution IV mandates real infrastructure tests; repositories enable this
- **Encryption Encapsulation**: Prevents accidental exposure of unencrypted data
- **Not Over-Engineering**: Only 4 repositories (Compensation, Benefits, SalaryHistory, BulkJob)

**Implementation**:
```csharp
public interface ICompensationRepository
{
    Task<CompensationRecord?> GetCurrentAsync(Guid employeeId, CancellationToken ct);
    Task<List<CompensationRecord>> GetHistoryAsync(Guid employeeId, CancellationToken ct);
    Task<CompensationRecord> CreateAsync(CompensationRecord record, CancellationToken ct);
    Task UpdateAsync(CompensationRecord record, CancellationToken ct);
}
```

**References**:
- Constitution IV: Real Infrastructure Testing
- Repository Pattern: https://martinfowler.com/eaaCatalog/repository.html

---

### 9. Data Validation Strategy

**Decision**: Data Annotations for DTO validation

**Rationale**:
- **Constitution Mandate**: Constitution XIV prohibits FluentValidation
- **Sufficient**: Data Annotations handle all validation requirements
- **Built-in**: No external dependencies
- **Clear**: Validation rules visible on DTO properties

**Implementation**:
```csharp
public class RecordCompensationChangeDto
{
    [Required]
    public DateTime EffectiveDate { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Base salary must be greater than 0")]
    public decimal BaseSalary { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be 3-letter ISO code")]
    public string Currency { get; set; }

    [Range(0, 100)]
    public decimal? BonusPercentage { get; set; }

    [Required]
    [StringLength(500)]
    public string ChangeReason { get; set; }
}
```

**References**:
- Constitution XIV: NO FluentValidation
- FR-031, FR-032, FR-033: Validation requirements

---

### 10. Event-Driven Integration

**Decision**: MassTransit with RabbitMQ for event-driven architecture

**Rationale**:
- **Loose Coupling**: Compensation Service decoupled from Employee Service
- **Eventual Consistency**: Events processed asynchronously
- **Reliability**: At-least-once delivery semantics
- **Audit Trail**: All events logged for compliance

**Message Patterns**:
- **Published**: CompensationChanged, BenefitsEnrollmentUpdated, BulkSalaryIncreaseCompleted
- **Consumed**: EmployeeCreated, EmployeeTerminated
- **Idempotency**: Consumer handlers check for duplicates (e.g., compensation already exists)

**Implementation**:
```csharp
builder.AddMassTransitWithRabbitMq(configure: x =>
{
    x.AddConsumer<EmployeeCreatedEventConsumer>();
    x.AddConsumer<EmployeeTerminatedEventConsumer>();
});
```

**References**:
- Constitution I: Service Autonomy
- MassTransit Documentation: https://masstransit.io

---

## Technology Stack Summary

| Component | Technology | Justification |
|-----------|-----------|---------------|
| Runtime | .NET 10.0 | Latest LTS, performance improvements, Constitution mandate |
| Database | PostgreSQL 18 | ACID compliance, JSON support, encryption support |
| ORM | EF Core 10.x | Native .NET, value converters for encryption |
| Messaging | RabbitMQ + MassTransit | Reliable delivery, at-least-once semantics |
| Caching | Redis 7.x | High-performance, distributed caching |
| Encryption | AES-256-GCM | Strong encryption, NIST approved |
| Validation | Data Annotations | Constitution XIV mandate, built-in |
| Testing | xUnit + Testcontainers | Constitution IV mandate, real infrastructure |

## Performance Considerations

### Caching Strategy
- **What to cache**: Benefits catalog (rarely changes, shared across employees)
- **What NOT to cache**: Employee compensation (too many keys, stale data risk)
- **TTL**: 24 hours for benefits catalog

### Query Optimization
- **AsNoTracking**: For read-only queries (20-30% performance improvement)
- **Indexes**: employee_id, (employee_id, is_current), status
- **Projections**: Select only needed columns for reports

### Bulk Operations
- **Batch Size**: 100 employees per transaction (balance between performance and rollback size)
- **Parallelism**: Process batches sequentially (avoid deadlocks)
- **Timeout**: 10 minutes max per SC-007

## Security Considerations

### Encryption Key Management
- **Storage**: Google Secret Manager
- **Rotation**: Support key rotation without downtime
- **Compromise Response**: Rotate within 24h, re-encrypt all data

### Audit Logging
- **Separation**: Audit logs separate from operational logs
- **Retention**: Indefinite per compliance requirements
- **No Salary Data**: Audit logs contain user, action, timestamp, resource - NOT salary amounts

### Permission Model
- **Read**: View compensation, benefits
- **Update**: Modify compensation, benefits
- **Admin**: Bulk operations
- **Reports**: Generate analysis reports

## Open Questions (None)

All technical decisions have been made. No open questions remain.

## References

- [Feature Specification](./spec.md)
- [Implementation Plan](./plan.md)
- [MALIEV Microservices Constitution](../../.specify/memory/constitution.md)
- EF Core Documentation: https://learn.microsoft.com/en-us/ef/core/
- MassTransit Documentation: https://masstransit.io
- Polly Resilience: https://www.pollydocs.org/

---

**Next Phase**: Phase 1 - Design (data-model.md, contracts/, quickstart.md)
