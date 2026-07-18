# Compensation Service - Quick Start Guide

## Prerequisites

- .NET 10.0 SDK or later
- Docker Desktop (for Testcontainers)
- Git
- Your favorite IDE (Visual Studio 2025, VS Code, or Rider)

## Local Development Setup

### 1. Clone and Navigate

```bash
git clone <repository-url>
cd Maliev.CompensationService
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Configure Secrets

The service uses Google Secret Manager in production, but for local development, use User Secrets:

```bash
cd src/Maliev.CompensationService.Api
dotnet user-secrets init
dotnet user-secrets set "Encryption:Key" "your-32-byte-base64-encoded-key"
dotnet user-secrets set "ConnectionStrings:CompensationDb" "Host=localhost;Database=compensation_dev;Username=dev;Password=dev"
```

### 4. Run with Testcontainers

The service automatically starts PostgreSQL, RabbitMQ, and Redis containers when running locally:

```bash
dotnet run --project src/Maliev.CompensationService.Api
```

**Expected Output:**
```
info: Testcontainers.PostgreSqlContainer[0]
      PostgreSQL container started on port 5432
info: Testcontainers.RabbitMqContainer[0]
      RabbitMQ container started on port 5672
info: Testcontainers.RedisContainer[0]
      Redis container started on port 6379
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: http://localhost:5000
      Now listening on: https://localhost:5001
```

### 5. Verify Health

```bash
curl http://localhost:5000/health
```

**Expected Response:**
```json
{
  "status": "Healthy",
  "checks": [
    { "name": "PostgreSQL", "status": "Healthy" },
    { "name": "RabbitMQ", "status": "Healthy" },
    { "name": "Redis", "status": "Healthy" }
  ]
}
```

## API Exploration

### Access API Documentation

Navigate to: `http://localhost:5000/compensation/scalar`

The Scalar UI provides interactive API documentation with authentication support.

### Authentication

#### 1. Obtain JWT Token (Development Mode)

Obtain a development JWT from the local AuthService flow, then expose it only in the current shell:

```bash
export DEV_TOKEN="<jwt-from-local-auth-service>"
```

*(In production, obtain tokens from your identity provider)*

#### 2. Make Authenticated Requests

```bash
curl -H "Authorization: Bearer $DEV_TOKEN" \
     http://localhost:5000/compensation/v1/employees/3fa85f64-5717-4562-b3fc-2c963f66afa6/compensation
```

## Common Workflows

### Workflow 1: Record Initial Compensation

**Scenario:** New employee starts, HR sets initial salary

```bash
curl -X POST http://localhost:5000/api/v1/employees/3fa85f64-5717-4562-b3fc-2c963f66afa6/compensation \
  -H "Authorization: Bearer $DEV_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "baseSalary": 75000.00,
    "currency": "USD",
    "compensationType": "Salary",
    "effectiveDate": "2025-01-15T00:00:00Z",
    "changeReason": "Initial compensation for new hire",
    "approvedBy": "1fa85f64-5717-4562-b3fc-2c963f66afa1"
  }'
```

**Expected Response:** `201 Created`
```json
{
  "employeeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "compensationRecordId": "7fa85f64-5717-4562-b3fc-2c963f66afa7",
  "baseSalary": 75000.00,
  "effectiveDate": "2025-01-15T00:00:00Z"
}
```

**Published Event:** `CompensationChangedEvent` (check RabbitMQ Management UI at `http://localhost:15672`)

### Workflow 2: View Compensation Details

```bash
curl -H "Authorization: Bearer $DEV_TOKEN" \
     http://localhost:5000/api/v1/employees/3fa85f64-5717-4562-b3fc-2c963f66afa6/compensation
```

**Expected Response:** `200 OK`
```json
{
  "employeeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "currentCompensation": {
    "baseSalary": 75000.00,
    "compensationType": "Salary",
    "effectiveDate": "2025-01-15T00:00:00Z"
  },
  "benefits": [
    {
      "benefitId": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
      "benefitName": "Health Insurance Premium",
      "enrollmentStatus": "Active",
      "coverageLevel": "Individual"
    }
  ]
}
```

### Workflow 3: Enroll in Benefits

```bash
curl -X POST http://localhost:5000/api/v1/employees/3fa85f64-5717-4562-b3fc-2c963f66afa6/benefits/enrollments \
  -H "Authorization: Bearer $DEV_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "benefitId": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
    "effectiveDate": "2025-02-01T00:00:00Z",
    "coverageLevel": "Family",
    "dependents": [
      {
        "firstName": "Jane",
        "lastName": "Doe",
        "dateOfBirth": "1990-05-15T00:00:00Z",
        "relationship": "Spouse",
        "nationalId": "123-45-6789"
      }
    ]
  }'
```

**Expected Response:** `201 Created`
**Published Event:** `BenefitsEnrollmentUpdatedEvent` with status `Pending`

### Workflow 4: Trigger Consumed Event (Employee Terminated)

**Simulate Employee Service publishing termination event:**

```bash
# Access RabbitMQ Management UI
open http://localhost:15672  # Default credentials: guest/guest

# Navigate to Queues → compensation-service-employee-terminated
# Publish message manually:
{
  "employeeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "terminationDate": "2025-12-31T00:00:00Z",
  "terminationType": "Voluntary",
  "timestamp": "2025-12-15T14:00:00Z"
}
```

**Expected Behavior:**
- All active benefits enrollments for employee are terminated
- Pending enrollments are rejected/cancelled
- `BenefitsEnrollmentUpdatedEvent` published for each enrollment

**Verify in logs:**
```
info: Compensation.Consumers.EmployeeTerminatedConsumer[0]
      Processing termination for employee 3fa85f64-5717-4562-b3fc-2c963f66afa6
info: Compensation.Services.BenefitsService[0]
      Terminated 2 active enrollments, rejected 1 pending enrollment
```

### Workflow 5: Bulk Salary Increase

```bash
curl -X POST http://localhost:5000/api/v1/bulk/salary-increases \
  -H "Authorization: Bearer $DEV_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "departmentId": "6fa85f64-5717-4562-b3fc-2c963f66afa9",
    "increasePercentage": 3.5,
    "effectiveDate": "2025-04-01T00:00:00Z",
    "reason": "Annual cost of living adjustment",
    "startedBy": "1fa85f64-5717-4562-b3fc-2c963f66afa1"
  }'
```

**Expected Response:** `202 Accepted`
```json
{
  "jobId": "2fa85f64-5717-4562-b3fc-2c963f66afa5",
  "status": "Pending",
  "message": "Bulk salary increase job queued for processing"
}
```

**Poll for completion:**
```bash
curl -H "Authorization: Bearer $DEV_TOKEN" \
     http://localhost:5000/api/v1/bulk/jobs/2fa85f64-5717-4562-b3fc-2c963f66afa5
```

**When complete:**
```json
{
  "jobId": "2fa85f64-5717-4562-b3fc-2c963f66afa5",
  "status": "Completed",
  "successCount": 450,
  "failureCount": 0,
  "totalBudgetImpact": 157500.00
}
```

**Published Event:** `BulkSalaryIncreaseCompletedEvent`

## Testing

### Run Unit Tests

```bash
dotnet test tests/Maliev.CompensationService.UnitTests
```

### Run Integration Tests (with Testcontainers)

```bash
dotnet test tests/Maliev.CompensationService.IntegrationTests
```

**Note:** Integration tests automatically start PostgreSQL, RabbitMQ, and Redis containers. First run may take 1-2 minutes to pull images.

### Run Contract Tests

```bash
dotnet test tests/Maliev.CompensationService.ContractTests
```

## Observability

### View Logs

Structured JSON logs are written to stdout:

```bash
dotnet run --project src/Maliev.CompensationService.Api | jq
```

**Sample log entry:**
```json
{
  "timestamp": "2025-12-28T10:30:00Z",
  "level": "Information",
  "message": "Compensation changed for employee",
  "employeeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "newSalary": 92000.00,
  "previousSalary": 85000.00,
  "changePercentage": 8.24,
  "correlationId": "abc123",
  "userId": "1fa85f64-5717-4562-b3fc-2c963f66afa1"
}
```

### View Metrics

Prometheus metrics are exposed at `http://localhost:5000/metrics`:

```bash
curl http://localhost:5000/metrics
```

**Sample metrics:**
```
# HELP compensation_changes_total Total compensation changes recorded
# TYPE compensation_changes_total counter
compensation_changes_total{reason="promotion"} 45

# HELP benefits_enrollments_active Current active benefits enrollments
# TYPE benefits_enrollments_active gauge
benefits_enrollments_active{benefit_type="health"} 1234

# HELP bulk_job_duration_seconds Bulk job execution duration
# TYPE bulk_job_duration_seconds histogram
bulk_job_duration_seconds_bucket{le="10"} 5
bulk_job_duration_seconds_bucket{le="30"} 15
```

### View Distributed Traces

Distributed tracing is configured via `Maliev.Aspire.ServiceDefaults`. Traces are exported to your configured APM backend (e.g., Application Insights, Jaeger).

## Troubleshooting

### Issue: PostgreSQL container won't start

**Symptom:** `Docker.DotNet.DockerApiException: No such container`

**Solution:**
```bash
docker ps -a  # Check for existing containers
docker rm -f $(docker ps -aq)  # Remove all stopped containers
dotnet run  # Retry
```

### Issue: Encryption key error

**Symptom:** `InvalidOperationException: Encryption key not configured`

**Solution:**
```bash
# Generate a valid AES-256 key
openssl rand -base64 32

# Set user secret
dotnet user-secrets set "Encryption:Key" "<generated-key>"
```

### Issue: RabbitMQ connection refused

**Symptom:** `RabbitMQ.Client.Exceptions.BrokerUnreachableException`

**Solution:**
```bash
# Check RabbitMQ container status
docker ps | grep rabbitmq

# Restart service (Testcontainers will recreate container)
dotnet run
```

### Issue: Optimistic concurrency conflict

**Symptom:** `DbUpdateConcurrencyException: Database operation expected to affect 1 row(s) but actually affected 0`

**Solution:** This is expected behavior when two users update the same compensation record simultaneously. The client should:
1. Fetch the latest data: `GET /employees/{id}/compensation`
2. Display conflict to user: "Another user modified this record. Please review and resubmit."
3. User re-enters changes with fresh data
4. Retry: `POST /employees/{id}/compensation`

## Next Steps

1. **Explore the codebase**: Start with `src/Maliev.CompensationService.Api/Program.cs`
2. **Review data model**: See `specs/001-compensation-service/data-model.md`
3. **Check API contracts**: See `specs/001-compensation-service/contracts/openapi.yaml`
4. **Understand events**: See `specs/001-compensation-service/contracts/events/`
5. **Read implementation plan**: See `specs/001-compensation-service/plan.md`

## Additional Resources

- **Specification**: `specs/001-compensation-service/spec.md`
- **Architecture Decisions**: `specs/001-compensation-service/research.md`
- **OpenAPI Documentation**: `http://localhost:5000/scalar/v1` (when running)
- **RabbitMQ Management**: `http://localhost:15672` (guest/guest)
- **Health Checks**: `http://localhost:5000/health`

## Support

For questions or issues:
1. Check existing documentation in `specs/001-compensation-service/`
2. Review logs with `dotnet run | jq`
3. Contact the development team
