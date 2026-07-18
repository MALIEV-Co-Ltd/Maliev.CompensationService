# Tasks: Employee Compensation Management Service

**Input**: Design documents from `/specs/001-compensation-service/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Tests**: This feature explicitly includes ~60 tests (30 unit, 25 integration, 5 contract) as specified in plan.md

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

Projects use flat structure at repository root per Constitution XV:
- **API**: `Maliev.CompensationService.Api/`
- **Application**: `Maliev.CompensationService.Application/`
- **Domain**: `Maliev.CompensationService.Domain/`
- **Infrastructure**: `Maliev.CompensationService.Infrastructure/`
- **Tests**: `Maliev.CompensationService.Tests/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [x] T001 Create solution file Maliev.CompensationService.sln at repository root
- [x] T002 [P] Create Maliev.CompensationService.Api project with .NET 10.0 SDK
- [x] T003 [P] Create Maliev.CompensationService.Application project (class library)
- [x] T004 [P] Create Maliev.CompensationService.Domain project (class library)
- [x] T005 [P] Create Maliev.CompensationService.Infrastructure project (class library)
- [x] T006 [P] Create Maliev.CompensationService.Tests project with xUnit
- [x] T007 Add project references per architecture (Api→Application→Domain, Infrastructure→Domain)
- [x] T008 [P] Create .gitignore file excluding bin/, obj/, .vs/, .idea/, *.user at repository root
- [x] T009 [P] Create .dockerignore excluding .git/, bin/, obj/, specs/, *.md, Test projects at repository root
- [x] T010 [P] Create nuget.config with GitHub Packages source for Maliev.Aspire.ServiceDefaults at repository root
- [x] T011 [P] Create .github/CODEOWNERS file
- [x] T012 Install NuGet packages for Api project (ASP.NET Core 10.0, Maliev.Aspire.ServiceDefaults, MassTransit.RabbitMQ)
- [x] T013 [P] Install NuGet packages for Application project (MediatR for CQRS pattern alternative)
- [x] T014 [P] Install NuGet packages for Infrastructure project (Npgsql.EntityFrameworkCore.PostgreSQL, StackExchange.Redis, MassTransit)
- [x] T015 [P] Install NuGet packages for Tests project (xUnit, Testcontainers, Testcontainers.PostgreSql, Testcontainers.RabbitMq, Testcontainers.Redis)

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

### Domain Foundation

- [x] T016 [P] Create CompensationType enum in Maliev.CompensationService.Domain/Enums/CompensationType.cs
- [x] T017 [P] Create BenefitType enum in Maliev.CompensationService.Domain/Enums/BenefitType.cs
- [x] T018 [P] Create EnrollmentStatus enum in Maliev.CompensationService.Domain/Enums/EnrollmentStatus.cs
- [x] T019 [P] Create DependentRelationship enum in Maliev.CompensationService.Domain/Enums/DependentRelationship.cs
- [x] T020 [P] Create BulkJobType enum in Maliev.CompensationService.Domain/Enums/BulkJobType.cs
- [x] T021 [P] Create BulkJobStatus enum in Maliev.CompensationService.Domain/Enums/BulkJobStatus.cs
- [x] T022 [P] Create CompensationPermissions static class in Maliev.CompensationService.Domain/Authorization/CompensationPermissions.cs

### Infrastructure Foundation

- [x] T023 Create IEncryptionService interface in Maliev.CompensationService.Application/Interfaces/IEncryptionService.cs
- [x] T024 Implement EncryptionService with AES-256-GCM in Maliev.CompensationService.Infrastructure/Services/EncryptionService.cs
- [x] T025 Create EncryptionValueConverter for decimal fields in Maliev.CompensationService.Infrastructure/Data/EncryptionValueConverter.cs
- [x] T026 Create CompensationDbContext with encryption value converter in Maliev.CompensationService.Infrastructure/Data/CompensationDbContext.cs
- [x] T027 Configure DbContext connection string and options in appsettings.json and appsettings.Development.json
- [x] T028 [P] Create TestcontainersFixture for PostgreSQL, RabbitMQ, Redis in Maliev.CompensationService.Tests/Integration/TestcontainersFixture.cs

### Application Foundation

- [x] T029 [P] Create base repository interfaces (ICompensationRepository, IBenefitsRepository, ISalaryHistoryRepository, IBulkJobRepository) in Maliev.CompensationService.Application/Interfaces/
- [x] T030 Configure dependency injection in Program.cs (DbContext, repositories, services, MassTransit)
- [x] T031 Add Maliev.Aspire.ServiceDefaults with builder.AddServiceDefaults() in Maliev.CompensationService.Api/Program.cs
- [x] T032 Configure MassTransit with RabbitMQ in Program.cs
- [x] T033 Configure Redis caching in Program.cs
- [x] T034 [P] Create SalaryLoggingFilter to prevent salary data in logs in Maliev.CompensationService.Api/Filters/SalaryLoggingFilter.cs
- [x] T035 Configure health checks (liveness, readiness) and map default endpoints with app.MapDefaultEndpoints() in Program.cs
- [x] T036 [P] Configure JWT authentication via ServiceDefaults in Program.cs
- [x] T037 [P] Configure authorization policies with CompensationPermissions in Program.cs

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - View Current Compensation Details (Priority: P1) 🎯 MVP

**Goal**: Enable authorized users to view current compensation information for employees including base salary, compensation type, bonus percentage, and commission rates

**Independent Test**: Create a test employee record, set their compensation, and retrieve it through the GET endpoint to verify decryption and authorization

### Domain & Infrastructure for User Story 1

- [x] T038 [P] [US1] Create CompensationRecord entity in Maliev.CompensationService.Domain/Entities/CompensationRecord.cs
- [x] T039 [P] [US1] Create CompensationRecordConfiguration for EF Core in Maliev.CompensationService.Infrastructure/Data/Configurations/CompensationRecordConfiguration.cs
- [x] T040 [US1] Implement CompensationRepository in Maliev.CompensationService.Infrastructure/Repositories/CompensationRepository.cs
- [x] T041 [US1] Create initial EF Core migration for compensation_records table
- [x] T042 [US1] Apply migration to create database schema

### Application Layer for User Story 1

- [x] T043 [P] [US1] Create CompensationRecordDto in Maliev.CompensationService.Application/DTOs/CompensationRecordDto.cs
- [x] T044 [P] [US1] Create CompensationMapper in Maliev.CompensationService.Application/Mappers/CompensationMapper.cs
- [x] T045 [US1] Create GetCompensationDetailsQuery in Maliev.CompensationService.Application/Queries/GetCompensationDetailsQuery.cs
- [x] T046 [US1] Implement GetCompensationDetailsQueryHandler in Maliev.CompensationService.Application/Queries/Handlers/GetCompensationDetailsQueryHandler.cs

### API Layer for User Story 1

- [x] T047 [US1] Create CompensationController with GetCompensationDetails endpoint in Maliev.CompensationService.Api/Controllers/CompensationController.cs
- [x] T048 [US1] Add RequirePermission attribute with CompensationPermissions.Read to GetCompensationDetails endpoint

### Tests for User Story 1

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [x] T049 [P] [US1] Unit test for GetCompensationDetailsQueryHandler in Maliev.CompensationService.Tests/Unit/Queries/GetCompensationDetailsQueryHandlerTests.cs
- [x] T050 [P] [US1] Unit test for CompensationMapper in Maliev.CompensationService.Tests/Unit/Mappers/CompensationMapperTests.cs
- [x] T051 [US1] Integration test for GET /employees/{employeeId}/compensation endpoint with Testcontainers in Maliev.CompensationService.Tests/Integration/Controllers/CompensationControllerTests.cs
- [x] T052 [US1] Integration test for encryption/decryption in repository operations

**Checkpoint**: User Story 1 complete - authorized users can view current compensation details with proper encryption/decryption

---

## Phase 4: User Story 2 - Record Compensation Changes (Priority: P1)

**Goal**: Enable HR administrators to record salary changes, promotions, or adjustments with approval tracking and audit trails

**Independent Test**: Submit a compensation change request with all required fields, verify the change is recorded, audit trail is created, and event is published

### Domain & Infrastructure for User Story 2

- [x] T053 [P] [US2] Create SalaryHistory entity in Maliev.CompensationService.Domain/Entities/SalaryHistory.cs
- [x] T054 [P] [US2] Create SalaryHistoryConfiguration for EF Core in Maliev.CompensationService.Infrastructure/Data/Configurations/SalaryHistoryConfiguration.cs
- [x] T055 [US2] Implement SalaryHistoryRepository in Maliev.CompensationService.Infrastructure/Repositories/SalaryHistoryRepository.cs
- [x] T056 [P] [US2] Create CompensationChangedEvent in Maliev.CompensationService.Domain/Events/CompensationChangedEvent.cs
- [x] T057 [US2] Create EF Core migration for salary_histories table with cascade delete
- [x] T058 [US2] Apply migration to create salary_histories table

### Application Layer for User Story 2

- [x] T059 [P] [US2] Create RecordCompensationChangeDto with Data Annotations validation in Maliev.CompensationService.Application/DTOs/RecordCompensationChangeDto.cs
- [x] T060 [P] [US2] Create SalaryHistoryDto in Maliev.CompensationService.Application/DTOs/SalaryHistoryDto.cs
- [x] T061 [P] [US2] Create SalaryHistoryMapper in Maliev.CompensationService.Application/Mappers/SalaryHistoryMapper.cs
- [x] T062 [US2] Create RecordCompensationChangeCommand in Maliev.CompensationService.Application/Commands/RecordCompensationChangeCommand.cs
- [x] T063 [US2] Implement RecordCompensationChangeCommandHandler with optimistic locking in Maliev.CompensationService.Application/Commands/Handlers/RecordCompensationChangeCommandHandler.cs
- [x] T064 [US2] Add business logic to mark previous record as not current and create salary history entry
- [x] T065 [US2] Add CompensationChangedEvent publishing to MassTransit in command handler
- [x] T066 [US2] Add validation for salary increase >25% flagging logic

### API Layer for User Story 2

- [x] T067 [US2] Add RecordCompensationChange POST endpoint to CompensationController in Maliev.CompensationService.Api/Controllers/CompensationController.cs
- [x] T068 [US2] Add RequirePermission attribute with CompensationPermissions.Update to RecordCompensationChange endpoint
- [x] T069 [US2] Add 409 Conflict handling for optimistic concurrency exceptions

### Tests for User Story 2

- [x] T070 [P] [US2] Unit test for RecordCompensationChangeCommandHandler in Maliev.CompensationService.Tests/Unit/Commands/RecordCompensationChangeCommandHandlerTests.cs
- [x] T071 [P] [US2] Unit test for SalaryHistoryMapper in Maliev.CompensationService.Tests/Unit/Mappers/SalaryHistoryMapperTests.cs
- [x] T072 [P] [US2] Unit test for >25% increase flagging logic
- [x] T073 [US2] Integration test for POST /employees/{employeeId}/compensation endpoint with Testcontainers
- [x] T074 [US2] Integration test for optimistic concurrency conflict handling
- [x] T075 [US2] Integration test for CompensationChangedEvent publishing to RabbitMQ
- [x] T076 [US2] Contract test for CompensationChangedEvent schema validation in Maliev.CompensationService.Tests/Contract/EventSchemaTests.cs

**Checkpoint**: User Story 2 complete - HR can record compensation changes with audit trails and event publishing

---

## Phase 5: User Story 3 - View Compensation History (Priority: P2)

**Goal**: Enable HR managers to review complete compensation history for employees to understand salary progression and approval chain

**Independent Test**: Create multiple compensation changes for a test employee and retrieve the complete history with all change details ordered by effective date

### Application Layer for User Story 3

- [x] T077 [US3] Create GetCompensationHistoryQuery in Maliev.CompensationService.Application/Queries/GetCompensationHistoryQuery.cs
- [x] T078 [US3] Implement GetCompensationHistoryQueryHandler with AsNoTracking optimization in Maliev.CompensationService.Application/Queries/Handlers/GetCompensationHistoryQueryHandler.cs

### API Layer for User Story 3

- [x] T079 [US3] Add GetCompensationHistory GET endpoint to CompensationController in Maliev.CompensationService.Api/Controllers/CompensationController.cs
- [x] T080 [US3] Add RequirePermission attribute with CompensationPermissions.Read to GetCompensationHistory endpoint

### Tests for User Story 3

- [x] T081 [P] [US3] Unit test for GetCompensationHistoryQueryHandler in Maliev.CompensationService.Tests/Unit/Queries/GetCompensationHistoryQueryHandlerTests.cs
- [x] T082 [US3] Integration test for GET /employees/{employeeId}/compensation/history endpoint with Testcontainers
- [x] T083 [US3] Integration test verifying salary data decryption for authorized users only

**Checkpoint**: User Story 3 complete - HR can view complete compensation history with proper authorization

---

## Phase 6: User Story 4 - Manage Benefits Enrollment (Priority: P2)

**Goal**: Enable employees to enroll in benefits programs, add dependents, select coverage levels, and manage benefit elections

**Independent Test**: Enroll a test employee in a benefit plan, add dependents with encrypted national IDs, and verify enrollment status and event publishing

### Domain & Infrastructure for User Story 4

- [x] T084 [P] [US4] Create Benefit entity in Maliev.CompensationService.Domain/Entities/Benefit.cs
- [x] T085 [P] [US4] Create BenefitsEnrollment entity in Maliev.CompensationService.Domain/Entities/BenefitsEnrollment.cs
- [x] T086 [P] [US4] Create Dependent entity in Maliev.CompensationService.Domain/Entities/Dependent.cs
- [x] T087 [P] [US4] Create BenefitConfiguration for EF Core in Maliev.CompensationService.Infrastructure/Data/Configurations/BenefitConfiguration.cs
- [x] T088 [P] [US4] Create BenefitsEnrollmentConfiguration for EF Core in Maliev.CompensationService.Infrastructure/Data/Configurations/BenefitsEnrollmentConfiguration.cs
- [x] T089 [P] [US4] Create DependentConfiguration for EF Core with encrypted national_id in Maliev.CompensationService.Infrastructure/Data/Configurations/DependentConfiguration.cs
- [x] T090 [US4] Implement BenefitsRepository in Maliev.CompensationService.Infrastructure/Repositories/BenefitsRepository.cs
- [x] T091 [P] [US4] Create BenefitsEnrollmentUpdatedEvent in Maliev.CompensationService.Domain/Events/BenefitsEnrollmentUpdatedEvent.cs
- [x] T092 [US4] Create EF Core migration for benefits, benefits_enrollments, dependents tables
- [x] T093 [US4] Apply migration to create benefits tables with cascade rules
- [x] T094 [US4] Seed initial benefits data (Health, Dental, Vision, Life, 401k) via migration or data script

### Application Layer for User Story 4

- [x] T095 [P] [US4] Create BenefitDto in Maliev.CompensationService.Application/DTOs/BenefitDto.cs
- [x] T096 [P] [US4] Create BenefitsEnrollmentDto in Maliev.CompensationService.Application/DTOs/BenefitsEnrollmentDto.cs
- [x] T097 [P] [US4] Create UpdateBenefitsEnrollmentDto with Data Annotations in Maliev.CompensationService.Application/DTOs/UpdateBenefitsEnrollmentDto.cs
- [x] T098 [P] [US4] Create DependentDto in Maliev.CompensationService.Application/DTOs/DependentDto.cs
- [x] T099 [P] [US4] Create BenefitsMapper in Maliev.CompensationService.Application/Mappers/BenefitsMapper.cs
- [x] T100 [US4] Create EnrollInBenefitCommand in Maliev.CompensationService.Application/Commands/EnrollInBenefitCommand.cs
- [x] T101 [US4] Implement EnrollInBenefitCommandHandler with waiting period calculation in Maliev.CompensationService.Application/Commands/Handlers/EnrollInBenefitCommandHandler.cs
- [x] T102 [US4] Create UpdateBenefitsEnrollmentCommand in Maliev.CompensationService.Application/Commands/UpdateBenefitsEnrollmentCommand.cs
- [x] T103 [US4] Implement UpdateBenefitsEnrollmentCommandHandler in Maliev.CompensationService.Application/Commands/Handlers/UpdateBenefitsEnrollmentCommandHandler.cs
- [x] T104 [US4] Create TerminateBenefitCommand in Maliev.CompensationService.Application/Commands/TerminateBenefitCommand.cs
- [x] T105 [US4] Implement TerminateBenefitCommandHandler in Maliev.CompensationService.Application/Commands/Handlers/TerminateBenefitCommandHandler.cs
- [x] T106 [US4] Add BenefitsEnrollmentUpdatedEvent publishing to all benefit command handlers
- [x] T107 [US4] Add validation for duplicate benefit enrollments

### API Layer for User Story 4

- [x] T108 [US4] Create BenefitsController in Maliev.CompensationService.Api/Controllers/BenefitsController.cs
- [x] T109 [US4] Add EnrollInBenefit POST endpoint with CompensationPermissions.Update
- [x] T110 [US4] Add UpdateBenefitsEnrollment PUT endpoint with CompensationPermissions.Update
- [x] T111 [US4] Add TerminateBenefit DELETE endpoint with CompensationPermissions.Update

### Tests for User Story 4

- [x] T112 [P] [US4] Unit test for EnrollInBenefitCommandHandler in Maliev.CompensationService.Tests/Unit/Commands/EnrollInBenefitCommandHandlerTests.cs
- [x] T113 [P] [US4] Unit test for UpdateBenefitsEnrollmentCommandHandler in Maliev.CompensationService.Tests/Unit/Commands/UpdateBenefitsEnrollmentCommandHandlerTests.cs
- [x] T114 [P] [US4] Unit test for TerminateBenefitCommandHandler in Maliev.CompensationService.Tests/Unit/Commands/TerminateBenefitCommandHandlerTests.cs
- [x] T115 [P] [US4] Unit test for BenefitsMapper in Maliev.CompensationService.Tests/Unit/Mappers/BenefitsMapperTests.cs
- [x] T116 [US4] Integration test for POST /employees/{employeeId}/benefits/enrollments endpoint with Testcontainers
- [x] T117 [US4] Integration test for PUT /employees/{employeeId}/benefits/enrollments/{enrollmentId} endpoint
- [x] T118 [US4] Integration test for dependent national ID encryption/decryption
- [x] T119 [US4] Integration test for BenefitsEnrollmentUpdatedEvent publishing
- [x] T120 [US4] Integration test for waiting period calculation (90 days)
- [x] T121 [US4] Contract test for BenefitsEnrollmentUpdatedEvent schema validation

**Checkpoint**: User Story 4 complete - employees can enroll in benefits with dependents and proper encryption

---

## Phase 7: User Story 8 - Integrate with Employee Lifecycle Events (Priority: P2)

**Goal**: Respond to employee lifecycle events (new hires, terminations) to maintain accurate compensation and benefits data

**Independent Test**: Publish test employee created and terminated events, verify compensation service responds appropriately (prepares for setup, terminates benefits)

### Domain & Infrastructure for User Story 8

- [x] T122 [P] [US8] Create EmployeeCreatedEvent in Maliev.CompensationService.Domain/Events/EmployeeCreatedEvent.cs (per MessagingContracts) with properties: EmployeeId, EmployeeNumber, StartDate, DepartmentId, PositionId?, ManagerId?, Timestamp
- [x] T123 [P] [US8] Create EmployeeTerminatedEvent in Maliev.CompensationService.Domain/Events/EmployeeTerminatedEvent.cs (per MessagingContracts) with properties: EmployeeId, TerminationDate, TerminationType?, FinalWorkingDay?, Timestamp
- [x] T124 [US8] Create EmployeeCreatedEventConsumer in Maliev.CompensationService.Infrastructure/Consumers/EmployeeCreatedEventConsumer.cs
- [x] T125 [US8] Create EmployeeTerminatedEventConsumer with auto-reject pending and terminate active logic in Maliev.CompensationService.Infrastructure/Consumers/EmployeeTerminatedEventConsumer.cs
- [x] T126 [US8] Add TerminateActiveEnrollments method to BenefitsRepository
- [x] T127 [US8] Add RejectPendingEnrollments method to BenefitsRepository
- [x] T128 [US8] Register consumers in MassTransit configuration in Program.cs

### Tests for User Story 8

- [x] T129 [P] [US8] Integration test for EmployeeCreatedEventConsumer in Maliev.CompensationService.Tests/Integration/Consumers/EmployeeCreatedEventConsumerTests.cs
- [x] T130 [US8] Integration test for EmployeeTerminatedEventConsumer with Testcontainers and RabbitMQ
- [x] T131 [US8] Integration test verifying auto-reject pending enrollments on termination
- [x] T132 [US8] Integration test verifying terminate active enrollments on termination
- [x] T133 [US8] Integration test verifying BenefitsEnrollmentUpdatedEvents published for each terminated/cancelled enrollment
- [x] T134 [P] [US8] Contract test for EmployeeCreatedEvent schema validation
- [x] T135 [P] [US8] Contract test for EmployeeTerminatedEvent schema validation

**Checkpoint**: User Story 8 complete - compensation service integrates with employee lifecycle events

---

## Phase 8: User Story 5 - View Available Benefits (Priority: P3)

**Goal**: Enable employees and HR to view all available benefits programs with descriptions, contribution amounts, and eligibility requirements

**Independent Test**: Create test benefit programs and retrieve the list through the GET endpoint, verify Redis caching works

### Application Layer for User Story 5

- [x] T136 [US5] Create GetAvailableBenefitsQuery in Maliev.CompensationService.Application/Queries/GetAvailableBenefitsQuery.cs
- [x] T137 [US5] Implement GetAvailableBenefitsQueryHandler with Redis caching (24h TTL) in Maliev.CompensationService.Application/Queries/Handlers/GetAvailableBenefitsQueryHandler.cs
- [x] T138 [US5] Create GetEmployeeBenefitsQuery in Maliev.CompensationService.Application/Queries/GetEmployeeBenefitsQuery.cs
- [x] T139 [US5] Implement GetEmployeeBenefitsQueryHandler in Maliev.CompensationService.Application/Queries/Handlers/GetEmployeeBenefitsQueryHandler.cs

### API Layer for User Story 5

- [x] T140 [US5] Add GetAvailableBenefits GET endpoint to BenefitsController with CompensationPermissions.Read
- [x] T141 [US5] Add GetEmployeeBenefits GET endpoint to BenefitsController with CompensationPermissions.Read

### Tests for User Story 5

- [x] T142 [P] [US5] Unit test for GetAvailableBenefitsQueryHandler in Maliev.CompensationService.Tests/Unit/Queries/GetAvailableBenefitsQueryHandlerTests.cs
- [x] T143 [P] [US5] Unit test for GetEmployeeBenefitsQueryHandler in Maliev.CompensationService.Tests/Unit/Queries/GetEmployeeBenefitsQueryHandlerTests.cs
- [x] T144 [US5] Integration test for GET /benefits endpoint with Testcontainers
- [x] T145 [US5] Integration test for GET /employees/{employeeId}/benefits endpoint
- [x] T146 [US5] Integration test for Redis caching behavior (cache hit, cache miss, TTL)

**Checkpoint**: User Story 5 complete - users can view available benefits with Redis caching

---

## Phase 9: User Story 6 - Execute Bulk Salary Operations (Priority: P3)

**Goal**: Enable HR administrators to apply salary increases across multiple employees in a department or organization

**Independent Test**: Submit a bulk increase request for a test department, verify all affected employees receive adjustment, poll job status, and verify webhook invocation

### Domain & Infrastructure for User Story 6

- [x] T147 [P] [US6] Create BulkJob entity in Maliev.CompensationService.Domain/Entities/BulkJob.cs
- [x] T148 [P] [US6] Create BulkJobConfiguration for EF Core in Maliev.CompensationService.Infrastructure/Data/Configurations/BulkJobConfiguration.cs
- [x] T149 [US6] Implement BulkJobRepository in Maliev.CompensationService.Infrastructure/Repositories/BulkJobRepository.cs
- [x] T150 [P] [US6] Create BulkSalaryIncreaseCompletedEvent in Maliev.CompensationService.Domain/Events/BulkSalaryIncreaseCompletedEvent.cs
- [x] T151 [US6] Create EF Core migration for bulk_jobs table with JSONB columns
- [x] T152 [US6] Apply migration to create bulk_jobs table

### Application Layer for User Story 6

- [x] T153 [P] [US6] Create BulkSalaryIncreaseDto with Data Annotations in Maliev.CompensationService.Application/DTOs/BulkSalaryIncreaseDto.cs
- [x] T154 [P] [US6] Create BulkJobStatusDto in Maliev.CompensationService.Application/DTOs/BulkJobStatusDto.cs
- [x] T155 [US6] Create BulkSalaryIncreaseCommand in Maliev.CompensationService.Application/Commands/BulkSalaryIncreaseCommand.cs
- [x] T156 [US6] Implement BulkSalaryIncreaseCommandHandler with batch processing (100 employees per transaction) in Maliev.CompensationService.Application/Commands/Handlers/BulkSalaryIncreaseCommandHandler.cs
- [x] T157 [US6] Add job status tracking and error summary logic to bulk command handler
- [x] T158 [US6] Add BulkSalaryIncreaseCompletedEvent publishing on job completion
- [x] T159 [US6] Add optional webhook HTTP POST invocation on job completion
- [x] T160 [US6] Add validation for max 500 employees per bulk operation
- [x] T161 [US6] Create GetBulkJobStatusQuery in Maliev.CompensationService.Application/Queries/GetBulkJobStatusQuery.cs
- [x] T162 [US6] Implement GetBulkJobStatusQueryHandler in Maliev.CompensationService.Application/Queries/Handlers/GetBulkJobStatusQueryHandler.cs

### API Layer for User Story 6

- [x] T163 [US6] Create BulkOperationsController in Maliev.CompensationService.Api/Controllers/BulkOperationsController.cs
- [x] T164 [US6] Add BulkSalaryIncrease POST endpoint returning 202 Accepted with job ID, with CompensationPermissions.Admin
- [x] T165 [US6] Add GetBulkJobStatus GET endpoint for polling with CompensationPermissions.Admin

### Tests for User Story 6

- [x] T166 [P] [US6] Unit test for BulkSalaryIncreaseCommandHandler in Maliev.CompensationService.Tests/Unit/Commands/BulkSalaryIncreaseCommandHandlerTests.cs
- [x] T167 [P] [US6] Unit test for GetBulkJobStatusQueryHandler in Maliev.CompensationService.Tests/Unit/Queries/GetBulkJobStatusQueryHandlerTests.cs
- [x] T168 [US6] Integration test for POST /bulk/salary-increases endpoint with Testcontainers
- [x] T169 [US6] Integration test for GET /bulk/jobs/{jobId} polling endpoint
- [x] T170 [US6] Integration test for bulk operation processing 50 test employees
- [x] T171 [US6] Integration test for BulkSalaryIncreaseCompletedEvent publishing
- [x] T172 [US6] Integration test for webhook invocation on job completion
- [x] T173 [US6] Integration test for max 500 employees validation
- [x] T174 [US6] Contract test for BulkSalaryIncreaseCompletedEvent schema validation

**Checkpoint**: User Story 6 complete - HR can execute bulk salary operations with job tracking and webhooks

---

## Phase 10: User Story 7 - Generate Compensation Reports (Priority: P3)

**Goal**: Enable HR leadership to generate compensation analysis and budget reports for strategic planning

**Independent Test**: Create test compensation data and generate analysis reports with expected metrics (averages, percentiles, budget impact)

### Application Layer for User Story 7

- [x] T175 [P] [US7] Create CompensationAnalysisDto in Maliev.CompensationService.Application/DTOs/CompensationAnalysisDto.cs
- [x] T176 [US7] Create GetCompensationAnalysisQuery in Maliev.CompensationService.Application/Queries/GetCompensationAnalysisQuery.cs
- [x] T177 [US7] Implement GetCompensationAnalysisQueryHandler with statistical calculations and department filtering in Maliev.CompensationService.Application/Queries/Handlers/GetCompensationAnalysisQueryHandler.cs

### API Layer for User Story 7

- [x] T178 [US7] Create ReportsController in Maliev.CompensationService.Api/Controllers/ReportsController.cs
- [x] T179 [US7] Add GetCompensationAnalysis GET endpoint with department filter parameter, with CompensationPermissions.Reports
- [x] T180 [US7] Add GetCompensationBudget GET endpoint with CompensationPermissions.Reports

### Tests for User Story 7

- [x] T181 [P] [US7] Unit test for GetCompensationAnalysisQueryHandler in Maliev.CompensationService.Tests/Unit/Queries/GetCompensationAnalysisQueryHandlerTests.cs
- [x] T182 [US7] Integration test for GET /reports/compensation-analysis endpoint with Testcontainers
- [x] T183 [US7] Integration test for GET /reports/compensation-budget endpoint
- [x] T184 [US7] Integration test verifying department filtering in reports

**Checkpoint**: User Story 7 complete - HR leadership can generate compensation analysis reports

---

## Phase 11: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories and final production readiness

### Docker & CI/CD

- [x] T185 [P] Create Dockerfile in Maliev.CompensationService.Api/Dockerfile with multi-stage build, BuildKit secrets for NuGet, EXPOSE 8080
- [x] T186 [P] Create ci-develop.yml workflow in .github/workflows/ with dotnet restore, build, test, docker build, push
- [x] T187 [P] Create ci-staging.yml workflow in .github/workflows/
- [x] T188 [P] Create ci-main.yml workflow in .github/workflows/

### Documentation & Configuration

- [x] T189 [P] Create README.md at repository root with service description, architecture, and links to specs
- [x] T190 [P] Update Program.cs with final configuration (resource limits, logging, metrics)
- [x] T191 [P] Configure warnings as errors in all project files
- [x] T192 [P] Add CompensationIAMRegistrationService for permission registration in Maliev.CompensationService.Infrastructure/Services/CompensationIAMRegistrationService.cs

### OpenAPI & Contract Validation

- [x] T193 [P] Configure Scalar UI for OpenAPI documentation at /scalar/v1 in Program.cs
- [x] T194 [P] Add OpenAPI schema validation test in Maliev.CompensationService.Tests/Contract/OpenApiSchemaTests.cs
- [x] T195 [P] Verify all contract event schemas match implementation in EventSchemaTests.cs

### Additional Unit Tests

- [x] T196 [P] Unit test for EncryptionService in Maliev.CompensationService.Tests/Unit/Services/EncryptionServiceTests.cs
- [x] T197 [P] Unit test for EncryptionValueConverter
- [x] T198 [P] Unit test for SalaryLoggingFilter

### Performance & Security Validation

- [x] T199 Validate AsNoTracking optimization in all query handlers
- [x] T200 Validate indexes on employee_id, (employee_id, is_current), status columns
- [x] T201 Run quickstart.md validation to ensure all workflows execute correctly
- [x] T202 Validate no salary data appears in logs by inspecting log output
- [x] T203 Validate circuit breaker configuration for critical services
- [x] T204 Validate retry with exponential backoff for event publishing
- [x] T205 Performance test: Verify <2s response time for view operations per SC-001
- [x] T206 Performance test: Verify <5s response time for update operations per SC-002
- [x] T207 Performance test: Verify bulk operation completes within 10 minutes for 500 employees per SC-007

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phases 3-10)**: All depend on Foundational phase completion
  - User Story 1 (P1): Can start after Foundational - No dependencies on other stories
  - User Story 2 (P1): Can start after Foundational - No dependencies on other stories
  - User Story 3 (P2): Depends on User Story 2 (needs SalaryHistory entity)
  - User Story 4 (P2): Can start after Foundational - No dependencies on other stories
  - User Story 8 (P2): Depends on User Story 4 (needs benefits termination logic)
  - User Story 5 (P3): Depends on User Story 4 (needs Benefit entity)
  - User Story 6 (P3): Depends on User Story 2 (uses compensation change logic)
  - User Story 7 (P3): Depends on User Story 1 and 2 (needs compensation data)
- **Polish (Phase 11)**: Depends on all desired user stories being complete

### Critical Path for MVP (P1 Stories Only)

1. Phase 1: Setup → Phase 2: Foundational
2. Phase 3: User Story 1 (View Compensation) - can run parallel with US2
3. Phase 4: User Story 2 (Record Changes) - can run parallel with US1
4. Phase 11: Partial polish (Docker, CI/CD, README)

**Estimated MVP delivery**: User Stories 1 + 2 provide core view/update compensation functionality

### Within Each User Story

- Tests MUST be written and FAIL before implementation
- Domain entities before repositories
- Repositories before application handlers
- Application handlers before API controllers
- Core implementation before integration tests
- Story complete before moving to next priority

### Parallel Opportunities

**Setup (Phase 1)**:
- T002, T003, T004, T005, T006 (all project creation)
- T008, T009, T010, T011 (all config files)
- T013, T014, T015 (NuGet packages per project)

**Foundational (Phase 2)**:
- T016-T022 (all enum creation)
- T029 (repository interfaces), T034 (logging filter), T036, T037 (auth/authz)

**User Story 1 (Phase 3)**:
- T038, T039 (entity + configuration)
- T043, T044 (DTOs + mapper)
- T049, T050 (unit tests)

**User Story 2 (Phase 4)**:
- T053, T054, T056 (entities + event)
- T059, T060, T061 (DTOs + mapper)
- T070, T071, T072 (unit tests)
- T076 (contract test)

**User Story 4 (Phase 6)**:
- T084, T085, T086 (all entities)
- T087, T088, T089 (all EF configurations)
- T095, T096, T097, T098 (all DTOs)
- T112, T113, T114, T115 (unit tests)

**Once Foundational Complete**:
- User Story 1 and User Story 2 can be developed in parallel by different developers
- User Story 4 can start in parallel with US1/US2

---

## Parallel Example: User Story 2

```bash
# Launch all unit tests for User Story 2 together (write tests first):
Task: T070 "Unit test for RecordCompensationChangeCommandHandler"
Task: T071 "Unit test for SalaryHistoryMapper"
Task: T072 "Unit test for >25% increase flagging logic"

# Launch all domain/infrastructure setup together:
Task: T053 "Create SalaryHistory entity"
Task: T054 "Create SalaryHistoryConfiguration for EF Core"
Task: T056 "Create CompensationChangedEvent"

# Launch all DTOs together:
Task: T059 "Create RecordCompensationChangeDto"
Task: T060 "Create SalaryHistoryDto"
Task: T061 "Create SalaryHistoryMapper"
```

---

## Implementation Strategy

### MVP First (P1 Stories: User Story 1 + 2)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1 (View Compensation)
4. Complete Phase 4: User Story 2 (Record Changes)
5. **STOP and VALIDATE**: Test both stories independently
6. Deploy/demo MVP with core compensation view/update functionality

### Incremental Delivery (Add P2 Stories)

7. Complete Phase 5: User Story 3 (View History)
8. Complete Phase 6: User Story 4 (Benefits Enrollment)
9. Complete Phase 7: User Story 8 (Lifecycle Integration)
10. **VALIDATE**: Test P2 stories independently
11. Deploy/demo with benefits management

### Full Feature Set (Add P3 Stories)

12. Complete Phase 8: User Story 5 (View Available Benefits)
13. Complete Phase 9: User Story 6 (Bulk Operations)
14. Complete Phase 10: User Story 7 (Reports)
15. Complete Phase 11: Polish & Cross-Cutting
16. **FINAL VALIDATION**: Run all tests, quickstart validation, performance tests
17. Deploy to production

### Parallel Team Strategy

With 3 developers after Foundational phase completes:

- **Developer A**: User Story 1 + User Story 3 (compensation viewing)
- **Developer B**: User Story 2 + User Story 6 (compensation updates + bulk)
- **Developer C**: User Story 4 + User Story 8 (benefits management + lifecycle)
- **All Together**: User Story 5 (quick), User Story 7 (reports), Polish

---

## Summary

**Total Tasks**: 207
**Test Tasks**: ~45 (unit, integration, contract)
**Implementation Tasks**: ~150
**Infrastructure Tasks**: ~12

**Tasks by User Story**:
- Setup: 15 tasks
- Foundational: 22 tasks
- User Story 1 (P1): 15 tasks
- User Story 2 (P1): 24 tasks
- User Story 3 (P2): 7 tasks
- User Story 4 (P2): 38 tasks
- User Story 8 (P2): 14 tasks
- User Story 5 (P3): 11 tasks
- User Story 6 (P3): 28 tasks
- User Story 7 (P3): 10 tasks
- Polish: 23 tasks

**MVP Scope** (P1 only): 76 tasks (Setup + Foundational + US1 + US2 + Partial Polish)
**Full P1+P2**: 135 tasks
**Full Feature Set**: 207 tasks

---

## Notes

- [P] tasks = different files, no dependencies, can run in parallel
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- Tests are included per plan.md specification (~60 tests)
- Verify tests fail before implementing (Red-Green-Refactor)
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- Constitution compliance verified in plan.md
