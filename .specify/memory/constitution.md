# MALIEV Microservices Constitution (v2.0.0)

This is the foundational law of the MALIEV platform. All services, developers, and agents MUST adhere to these rules.

---

## 🏗️ 1. Technical Architecture & Frameworks

### I. Framework Standards
*   **Target Framework**: All C# services MUST target **.NET 10** (and EF Core 10).
*   **Infrastructure**: Standard stack includes **PostgreSQL** (via CloudNativePG), **RabbitMQ** (via MassTransit), and **Redis** (caching).
*   **Optimization**: Codebase must be optimized for **minimal resource usage** (e.g., memory-efficient pooling, trimmed images).

### II. ServiceDefaults & Aspire
*   **Shared Infrastructure**: Every service MUST fully utilize `Maliev.Aspire.ServiceDefaults`. Manual configuration of OpenTelemetry, health checks, or resilience is prohibited.
*   **Local Orchestration**: Services must be integrated into the `Maliev.Aspire` project for local development.

### III. Solutions & Projects
*   **Solution Format**: Prefer **.slnx** over the traditional `.sln`.
*   **No Boilerplate**: Absolute prohibition of default template code (e.g., `Class1.cs`, `WeatherForecast.cs`, `UnitTest1.cs`). Delete them immediately.

---

## 📡 2. API & Communication

### IV. Routing & Versioning
*   **Service Prefix**: All routes MUST be prefixed with the service domain (e.g., `/auth`, `/customer`, `/job`).
*   **Versioning**: All API routes MUST be versioned (e.g., `/auth/v1/...`).
*   **launchSettings**: `launchSettings.json` MUST be configured to automatically launch the **Scalar** documentation page.

### V. Documentation
*   **Scalar UI**: Use **Scalar** instead of Swagger for API documentation. Swagger/Swashbuckle packages are BANNED.
*   **XML Documentation**: Mandatory XML comments on all public methods, properties, and classes.

### VI. Messaging
*   **Centralized Contracts**: All inter-service events MUST reside in `Maliev.MessagingContracts`. Local events are prohibited for inter-service communication.

---

## 🔒 3. Security & Authorization

### VII. Permission System
*   **GCP-Style Permissions**: Use resource-scoped permissions (e.g., `auth.tokens.revoke`, `job.jobs.read`).
*   **Implementation**: Permissions must be defined as `static const string` and enforced using the custom `[RequirePermission]` attribute (not plain `[Authorize]`).

### VIII. Secrets & Credentials
*   **Zero Secrets**: Absolute prohibition of secrets in the entire codebase. No hardcoded keys, passwords, or connection strings.
*   **Environment Injection**: Use Google Cloud's **External Secrets Operator (ESO)** for production secrets.
*   **NuGet Feed**: Connection to the private NuGet feed (for ServiceDefaults/Contracts) MUST use environment-variable-based credentials.

---

## 🛠️ 4. Development & Workflow

### IX. Version Control
*   **Feature Branches**: Use a feature-branch-oriented workflow. Development should happen on feature branches, issued into a PR for merge into `develop`.
*   **LLM Metadata**: Committing `.claude`, `.gemini`, `.opencode`, `.specify`, etc., is allowed and encouraged.
*   **Exclusions**: Never commit developer-specific settings (e.g., `*.local.json`, `.env`).

### X. Code Quality
*   **TreatWarningsAsErrors**: Mandatory. All warnings must be resolved correctly; suppression is prohibited.
*   **Banned Libraries**: Strictly no **Swagger/Swashbuckle**, **FluentAssertions**, or **FluentValidation**.
*   **Pre-commit**: Use `.pre-commit-config.yaml` for local linting and verification.

### XI. GitHub Configuration
*   **CODEOWNERS**: Mandatory at `.github/CODEOWNERS`.
*   **Dependabot**: `dependabot.yml` is required in all services.
*   **Workflows**: `.github/workflows` MUST include PR validation, Gemini workflows, and CI workflows for `dev`/`staging`/`main`.

---

## 🧪 5. Testing & Validation

### XII. Testing Mandates
*   **Code Coverage**: Minimum **80% coverage** required for all services.
*   **Real Infrastructure**: Use **Testcontainers** for all integration tests. In-memory databases are prohibited.

---

## 📦 6. Containerization & Deployment

### XIII. Docker Standards
*   **Location**: `Dockerfile` MUST be located in the API project folder.
*   **Structure**: Use optimized, multi-stage builds.
*   **Best Practices**: Use the built-in `app` user; ensure correct file ownership.

### XIV. GitOps & GKE
*   **Deployments**: Managed via `maliev-gitops` using **ArgoCD**.
*   **Infrastructure**: Standardized resource definitions for Redis, RabbitMQ, and Postgres across environments.

---

**Version:** 2.0.0 | **Ratified:** 2026-02-22 | **Last Amended:** 2026-02-22
