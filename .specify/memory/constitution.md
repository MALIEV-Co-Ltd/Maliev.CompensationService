# MALIEV Microservices Constitution

## Core Principles

### I. Service Autonomy (NON-NEGOTIABLE)

Each microservice must be **self-contained**:

* Own database and schema
* Own domain logic
* Interact with others only via APIs or events
* No direct database access to another service

**Rationale:** Enables independent deployment, scaling, and ownership.

---

### II. Explicit Contracts

* All APIs documented via **OpenAPI/Scalar**
* Data contracts versioned (MAJOR.MINOR)
* Backward-compatible migrations mandatory

**Rationale:** Prevents breaking changes and preserves consumer stability. Scalar provides a more modern and performant API documentation experience.

---

### III. Test-First Development (NON-NEGOTIABLE)

* Tests authored **immediately after specification approval**, before implementation
* Code must **fail tests first** (Red–Green–Refactor)
* Unit, integration, and contract tests mandatory
* Minimum 80 % coverage for business-critical logic
* Test code reviewed equally with production code

**Rationale:** Ensures correctness before coding and keeps system behavior verifiable.

---

### IV. Real Infrastructure Testing (NON-NEGOTIABLE)

* **ALL tests MUST use real infrastructure dependencies** via Testcontainers - no in-memory substitutes allowed
* **PostgreSQL**: Real PostgreSQL instances required (no EF Core InMemoryDatabase provider permitted)
* **RabbitMQ**: Real RabbitMQ instances required for message queue testing (no in-memory message buses)
* **Redis**: Real Redis instances required for caching and distributed locking tests (no in-memory cache providers)
* Integration tests MUST use Docker containers for all infrastructure (local/CI)
* Test isolation achieved through database transactions, queue purging, or cleanup scripts
* Test infrastructure must mirror production configuration exactly (same versions, same settings)

**Rationale:** In-memory substitutes have different behavior, concurrency handling, and constraints than real infrastructure. Testing against production-like infrastructure catches real-world issues early (distributed locking race conditions, message serialization, connection pooling, transaction isolation) and eliminates false positives from in-memory quirks. This ensures test fidelity and production confidence across all infrastructure layers.

---

### V. Auditability & Observability

* Structured JSON logging with traceable user/action IDs
* Immutable audit logs retained per policy
* Health checks for liveness/readiness
* **Mandatory Log Level Configuration**: `appsettings.json` MUST use the following LogLevel configuration to reduce noise:
  ```json
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning",
    "Microsoft.EntityFrameworkCore": "Warning",
    "Microsoft.AspNetCore.Watch.BrowserRefresh": "None",
    "Microsoft.Hosting.Lifetime": "Information",
    "Microsoft.AspNetCore.Watch": "Warning",
    "System": "Warning"
  }
  ```

**Rationale:** Enables compliance, diagnostics, and operational insight.

---

### VI. Security & Compliance

* JWT authentication, role-based authorization
* Sensitive data encrypted at rest and in transit
* Compliance with GDPR, Thai tax law, and all relevant regulations

---

### VII. Secrets Management & Configuration Security (NON-NEGOTIABLE)

* No secrets in source code
* Secrets injected from **Google Secret Manager**
* Public repositories sanitized of real endpoints
* Commits scanned for secrets before merge

**Rationale:** Prevents leaks and targeted attacks.

---

### VIII. Zero Warnings Policy (NON-NEGOTIABLE)

* Builds must emit zero warnings
* Warnings treated as build failures

**Rationale:** Eliminates technical debt and instability.

---

### IX. Clean Project Artifacts (NON-NEGOTIABLE)

* Remove unused files, outdated docs, and generated artifacts
* `.gitignore` must exclude temporary files
* `.dockerignore` must exclude build artifacts, specs, and IDE files
* Cleanup enforced pre-release
* **NO additional markdown documents** in the repository root (e.g., `COMPLIANCE_REVIEW.md` is forbidden). Only `README.md`, `LICENSE` are allowed.
* **CODEOWNERS** file is MANDATORY at `.github/CODEOWNERS` with content: `* @MALIEV-Co-Ltd/core-developers`

---

### X. Docker Best Practices (NON-NEGOTIABLE)

* **The Dockerfile MUST be located in the API project folder** (e.g., `Maliev.UploadService.Api/Dockerfile`), NOT at the repository root.
* **ALL services MUST use the built-in `app` user** from Microsoft's ASP.NET runtime images
* **NO custom user creation** with `useradd`, `adduser`, or `addgroup` commands
* Multi-stage builds mandatory: SDK for build, ASP.NET runtime for final image
* Use .NET 10 base images: `mcr.microsoft.com/dotnet/sdk:10.0` and `mcr.microsoft.com/dotnet/aspnet:10.0`
* Set ownership with `chown -R app:app /app` **BEFORE** the `USER app` directive, then COPY files as `app` user
* Use `.dockerignore` to exclude build outputs, IDE files, specs, CI/CD files, **and Test projects**
* BuildKit secrets mandatory for NuGet credentials: `--mount=type=secret,id=nuget_username`
* Health checks must validate service liveness endpoint: `HEALTHCHECK CMD curl -f http://localhost:8080/[service-name]/liveness || exit 1`
* Optimize layer caching by copying project files before source code
* Expose port 8080: `EXPOSE 8080` and `ENV ASPNETCORE_URLS=http://+:8080`

**Standard Dockerfile Pattern:**

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /
COPY nuget.config ./
COPY ["Maliev.Service.Api/Maliev.Service.Api.csproj", "Maliev.Service.Api/"]
RUN --mount=type=secret,id=nuget_username \
    --mount=type=secret,id=nuget_password \
    NUGET_USERNAME=$(cat /run/secrets/nuget_username) \
    NUGET_PASSWORD=$(cat /run/secrets/nuget_password) \
    dotnet restore "Maliev.Service.Api/Maliev.Service.Api.csproj"
COPY . .
WORKDIR "/Maliev.Service.Api"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
RUN chown -R app:app /app
USER app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
HEALTHCHECK CMD curl -f http://localhost:8080/[service-name]/liveness || exit 1
ENTRYPOINT ["dotnet", "Maliev.Service.Api.dll"]
```

**Rationale:** Microsoft's built-in `app` user provides security without complexity. BuildKit secrets prevent credential exposure in Docker image layers. Following Docker best practices ensures consistent, secure, and efficient container images across all services.

---

### XI. Simplicity & Maintainability

* Apply YAGNI
* Favor readable, stateless design
* Shared libraries must be versioned and documented

---

### XII. Business Metrics & Analytics (NON-NEGOTIABLE)

* Every service must expose **business-relevant metrics and analytics endpoints** for use by the company's telemetry pipeline.
* Metrics must quantify both **system health** and **business outcomes**, including (where applicable):

  * Number of processed jobs, quotes, or transactions
  * Active users, conversion rates, and session durations
  * Production throughput, revenue per feature, or machine utilization
* Metrics must use **structured formats** compatible with Prometheus, OpenTelemetry, or other standard collectors.
* Services must tag metrics with:

  * `service_name`
  * `version`
  * `region`
  * `environment` (dev/staging/prod)
* Each release must define a clear mapping between **business objectives** and the metrics implemented.
* Tests must validate the **presence and format** of required metrics endpoints.
* Metrics must not expose confidential or personally identifiable information.

**Rationale:** Analytics convert operational data into measurable business intelligence. This enables data-driven decisions for product strategy, cost optimization, and growth.

---

### XIII. .NET Aspire Integration (NON-NEGOTIABLE)

* **ServiceDefaults as NuGet Package**: All microservices MUST consume `Maliev.Aspire.ServiceDefaults` as a NuGet package from GitHub Packages, NOT as a project reference
* **Package Source Configuration**: Each repository MUST have a `nuget.config` file pointing to GitHub Packages with credential placeholders
* **CI/CD Authentication**: Workflows MUST use `GITOPS_PAT` (with `read:packages` scope) for NuGet authentication - `GITHUB_TOKEN` is insufficient for cross-repo packages
* **Docker BuildKit Secrets**: Dockerfiles MUST use BuildKit secrets (`--mount=type=secret`) for NuGet credentials - using `ARG` for credentials is FORBIDDEN (exposes in image layers)
* **Program.cs Integration**: All services MUST call `builder.AddServiceDefaults()` and `app.MapDefaultEndpoints()`
* **nuget.config Mandatory**: Repository root MUST contain `nuget.config` with GitHub Packages source and `%NUGET_USERNAME%`/`%NUGET_PASSWORD%` placeholders

**Rationale:** Each microservice has its own Git repository. Project references (`../../Maliev.Aspire/...`) fail in CI because the Aspire repository is not present in the microservice's checkout context. Using a NuGet package from GitHub Packages enables independent CI/CD pipelines while maintaining shared observability standards. BuildKit secrets prevent credential exposure in Docker image layers.

---

### XIV. Code Quality & Library Standards (NON-NEGOTIABLE)

* **NO AutoMapper**: Explicit mapping only.
  * **Rationale**: AutoMapper hides references, makes refactoring difficult, and introduces runtime errors that should be compile-time errors. Explicit mapping is explicit, searchable, and performant.
* **NO FluentValidation**: Use standard .NET DataAnnotations or manual validation logic.
  * **Rationale**: FluentValidation adds unnecessary complexity and abstraction. Standard validation is built-in, sufficient, and reduces dependency bloat.
* **NO FluentAssertions**: Use standard xUnit `Assert`.
  * **Rationale**: FluentAssertions adds a large dependency and encourages readable but sometimes ambiguous assertions. Standard `Assert` is part of the test framework, faster, and sufficient.

---

### XV. Project Structure & Naming (NON-NEGOTIABLE)

* **Flat Structure**: For .NET applications, create the API, Data, and Tests projects **directly at the root of the repository**.
  * ❌ NO `/src` folder
  * ❌ NO `/tests` folder
* **Naming Convention**: Projects MUST be named with the full company prefix.
  * ✅ `Maliev.UploadService.Api`
  * ❌ `UploadService.Api`
* **Dockerfile Placement**: Must be inside the API project folder, NOT at the root.

---

### XVI. CI/CD Standards (NON-NEGOTIABLE)

* **Workflow Filenames**: GitHub Actions workflows MUST be named explicitly:
  * `ci-develop.yml`
  * `ci-staging.yml`
  * `ci-main.yml`
* **No Docker Compose**: Use **Testcontainers** for all integration tests and local development verification. `docker-compose.yml` is NOT required or recommended.

---

## Deployment & Operations Standards

* All services containerized via Docker
* Configurable solely by environment variables
* Rate limiting and recovery mechanisms mandatory
* Services must emit metrics consumable by the central telemetry gateway
* Metrics availability verified during deployment pipeline

---

## Development Workflow

**Mandatory sequence:**

1. Specification
2. **Test Definition (includes metrics tests)**
3. Implementation
4. Validation (tests, coverage, analytics endpoints)
5. Refactor

* Pull requests without analytics instrumentation will be rejected.
* CI/CD must verify both functional tests and metrics schema compliance.

---

## Security Compliance & Audit Requirements

* Pre-commit scans for secrets and sensitive endpoints
* Compromised credentials rotated within 24 hours
* Quarterly audits of metrics exposure to ensure no PII leakage

---

## Governance

* Constitution supersedes developer preference.
* All PRs validated for constitutional and analytics compliance.
* Amendments require leadership approval and documented migration plan.
* Violations block merge or deployment.

---

**Version:** 1.7.0 | **Ratified:** 2025-12-04 | **Last Amended:** 2025-12-04
