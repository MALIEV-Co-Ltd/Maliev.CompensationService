# Maliev Compensation Service

[![Build Status](https://img.shields.io/badge/Build-Passing-success)](https://github.com/ORGANIZATION/Maliev.CompensationService)
[![.NET Version](https://img.shields.io/badge/.NET-10.0-blue)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![Database](https://img.shields.io/badge/Database-PostgreSQL%2018-blue)](https://www.postgresql.org/)

Dedicated microservice for managing employee compensation, salary history, and benefit enrollments.

**Role in MALIEV Architecture**: Handles all financial aspects of the employee relationship. It maintains sensitive salary data, manages benefit programs, and publishes compensation events used by Payroll and Accounting services.

---

## 🏗️ Architecture & Tech Stack

- **Framework**: ASP.NET Core 10.0 (C# 13)
- **Database**: PostgreSQL 18 with Entity Framework Core 10.x
- **Distributed Cache**: Redis 7.x (High-speed salary history retrieval)
- **Messaging**: RabbitMQ via MassTransit
- **Security**: AES-256 field-level encryption for sensitive metrics
- **API Documentation**: OpenAPI 3.1 + Scalar UI

---

## ⚖️ Constitution Rules

This service strictly adheres to the platform development mandates:

### Banned Libraries
To maintain high performance and low complexity, the following are **NOT** used:
- ❌ **AutoMapper**: Explicit manual mapping only.
- ❌ **FluentValidation**: Standard Data Annotations (`[Required]`, `[EmailAddress]`) only.
- ❌ **FluentAssertions**: Standard xUnit `Assert` methods only.
- ❌ **In-memory Test DB**: All integration tests use **Testcontainers** with real PostgreSQL 18.

### Mandatory Practices
- ✅ **TreatWarningsAsErrors**: Enabled in all `.csproj` files.
- ✅ **XML Documentation**: Required on all public methods and properties.
- ✅ **No Secrets in Code**: All sensitive configuration injected via environment variables.
- ✅ **No Test Config in Program.cs**: Test configuration in test fixtures only.
- ✅ **IAM Integration**: Self-registers permissions with the IAM Service using GCP-style naming: `{service}.{resource}.{action}`.

---

## ✨ Key Features

- **Compensation Lifecycle**: Management of base salaries, bonuses, and commission structures.
- **Immutable Salary History**: Complete audit trail of all compensation changes over the employee's tenure.
- **Benefits Administration**: Management of health, dental, and insurance benefit enrollments and dependents.
- **Bulk Adjustments**: High-performance engine for organizational or departmental mass salary increases.
- **Encrypted Storage**: Secure handling of sensitive financial data using industry-standard encryption.

---

## 🚀 Quick Start

### Prerequisites
- .NET 10.0 SDK
- Docker Desktop (for infrastructure)
- PostgreSQL 18 (Alpine)

### Local Development Setup

1. **Clone the repository**
```bash
git clone https://github.com/ORGANIZATION/Maliev.CompensationService.git
cd Maliev.CompensationService
```

2. **Spin up Infrastructure**
```bash
docker run --name compensation-db -e POSTGRES_PASSWORD=YOUR_PASSWORD -p 5432:5432 -d postgres:18-alpine
docker run --name compensation-redis -p 6379:6379 -d redis:7-alpine
```

3. **Configure Environment**
```powershell
# Windows PowerShell
$env:ConnectionStrings__CompensationDbContext="YOUR_POSTGRES_CONNECTION_STRING"
$env:ConnectionStrings__Cache="YOUR_REDIS_CONNECTION_STRING"
```

4. **Apply Migrations & Run**
```bash
dotnet ef database update --project Maliev.CompensationService.Api
dotnet run --project Maliev.CompensationService.Api
```

The service will be available at `http://localhost:5000/compensation`. Access the interactive documentation at `http://localhost:5000/compensation/scalar`.

---

## 📡 API Endpoints

All endpoints are prefixed with `/compensation/v1/`.

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/employees/{id}/compensation` | Get current employee compensation |
| POST | `/employees/{id}/compensation` | Record a salary change |
| GET | `/employees/{id}/benefits` | List benefit enrollments |
| POST | `/employees/bulk/salary-increase` | Apply organizational mass increase |

---

## 🏥 Health & Monitoring

Standardized health probes for Kubernetes orchestration:
- **Liveness**: `GET /compensation/liveness`
- **Readiness**: `GET /compensation/readiness` (Checks DB and Redis connectivity)
- **Metrics**: `GET /compensation/metrics` (Prometheus format)

---

## 🧪 Testing

We prioritize reliable tests over mock-heavy unit tests.

```bash
# Run all tests using Testcontainers
dotnet test --verbosity normal
```

- **Integration Tests**: Use real PostgreSQL 18 containers.
- **Contract Tests**: Ensure API stability for consumers.

---

## 📦 Deployment

Infrastructure management is handled via GitOps patterns.

- **Docker Image**: `REGION-docker.pkg.dev/PROJECT_ID/REPOSITORY/maliev-compensation-service:{sha}`
- **Environments**: Development, Staging, Production

---

## 📄 License

Proprietary - © 2025 MALIEV Co., Ltd. All rights reserved.