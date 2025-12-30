# Compensation Service

Dedicated microservice for managing employee compensation, salary history, and benefits for Maliev Co. Ltd.

## Overview

The Compensation Service handles all financial aspects of the employee relationship, including:

- **Compensation Management** - Recording and updating base salary, bonus structures, and commission rates.
- **Salary History** - Maintaining a complete audit trail of all compensation changes over time.
- **Benefits Enrollment** - Managing employee enrollment in various benefit programs (health, dental, etc.).
- **Bulk Operations** - Applying mass salary increases across departments or the entire organization.

## Architecture

- **Framework**: ASP.NET Core 10.0
- **Database**: PostgreSQL 18 with Entity Framework Core
- **Messaging**: RabbitMQ via MassTransit
- **Security**: AES-256 encryption for sensitive salary fields

## Getting Started

### Prerequisites

- .NET 10.0 SDK
- PostgreSQL 18
- Docker (optional, for Redis and RabbitMQ)

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/MALIEV-Co-Ltd/Maliev.CompensationService.git
   ```

2. **Run database migrations**
   ```bash
   dotnet ef database update --project Maliev.CompensationService.Infrastructure --startup-project Maliev.CompensationService.Api
   ```

3. **Run the service**
   ```bash
   dotnet run --project Maliev.CompensationService.Api
   ```

   The service will be available at `https://localhost:7079` or `http://localhost:5032`.

## API Endpoints

### Compensation

```
GET  /compensation/v1/employees/{employeeId}/compensation - Get current compensation
POST /compensation/v1/employees/{employeeId}/compensation - Record compensation change
GET  /compensation/v1/employees/{employeeId}/compensation/history - Get salary history
POST /compensation/v1/employees/bulk/salary-increase - Apply mass salary increase
```

### Benefits

```
GET  /compensation/v1/employees/{employeeId}/benefits - Get benefit enrollments
POST /compensation/v1/employees/{employeeId}/benefits/{benefitId}/enroll - Enroll in a benefit
PUT  /compensation/v1/employees/{employeeId}/benefits/{enrollmentId} - Update enrollment
POST /compensation/v1/employees/{employeeId}/benefits/{enrollmentId}/dependents - Add dependent
```

## Integration Events Published

- `CompensationChangedEvent` - Notifies other services of salary changes.

## Integration Events Consumed

- `EmployeeCreatedIntegrationEvent` - Prepares compensation context for new hires.
- `EmployeeTerminatedIntegrationEvent` - Deactivates active benefits for departing employees.

## License

Copyright © 2025 Maliev Co. Ltd. All rights reserved.