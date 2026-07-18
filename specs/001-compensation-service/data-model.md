# Data Model: Employee Compensation Management Service

**Date**: 2025-12-28
**Feature**: Employee Compensation Management Service
**Related**: [plan.md](./plan.md) | [spec.md](./spec.md) | [research.md](./research.md)

## Overview

This document defines the complete data model for the Compensation Service, including entities, relationships, validation rules, state transitions, and database schema.

## Entity Relationship Diagram

```
┌─────────────────────┐         ┌──────────────────┐
│  CompensationRecord │◄────────│  SalaryHistory   │
│                     │         │                  │
│ - Id (PK)           │         │ - Id (PK)        │
│ - EmployeeId        │         │ - EmployeeId     │
│ - EffectiveDate     │         │ - CompRecordId   │
│ - BaseSalary (enc)  │         │ - PrevSalary (enc│
│ - Currency          │         │ - NewSalary (enc)│
│ - CompType          │         │ - ChangeAmount   │
│ - BonusPercentage   │         │ - ChangePercent  │
│ - CommissionRate    │         │ - EffectiveDate  │
│ - ChangeReason      │         │ - ChangeType     │
│ - ApprovedBy        │         │ - ChangedBy      │
│ - IsCurrent         │         └──────────────────┘
│ - CreatedDate       │
│ - ModifiedDate      │
└─────────────────────┘

┌─────────────────────┐
│      Benefit        │
│                     │
│ - Id (PK)           │
│ - Name              │
│ - Description       │
│ - BenefitType       │
│ - EmployerContrib   │
│ - EmployeeContrib   │
│ - IsActive          │
│ - CreatedDate       │
│ - ModifiedDate      │
└─────────────────────┘
          △
          │
          │ 1:N
          │
┌─────────────────────┐         ┌──────────────────┐
│ BenefitsEnrollment  │◄────────│    Dependent     │
│                     │         │                  │
│ - Id (PK)           │         │ - Id (PK)        │
│ - EmployeeId        │         │ - EnrollmentId   │
│ - BenefitId (FK)    │         │ - FirstName      │
│ - EnrollmentDate    │         │ - LastName       │
│ - TerminationDate   │         │ - Relationship   │
│ - Status            │         │ - DateOfBirth    │
│ - EmpContribution   │         │ - NationalId(enc)│
│ - CoverageLevel     │         │ - CreatedDate    │
│ - CreatedDate       │         │ - ModifiedDate   │
│ - ModifiedDate      │         └──────────────────┘
└─────────────────────┘

┌─────────────────────┐
│      BulkJob        │
│                     │
│ - Id (PK)           │
│ - JobType           │
│ - Status            │
│ - Parameters (JSON) │
│ - SuccessCount      │
│ - FailureCount      │
│ - ErrorDetails(JSON)│
│ - StartedBy         │
│ - StartedAt         │
│ - CompletedAt       │
└─────────────────────┘
```

## Core Entities

### 1. CompensationRecord

Represents an employee's compensation at a specific point in time.

**C# Entity**:
```csharp
public class CompensationRecord
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public DateTime EffectiveDate { get; set; }
    public decimal BaseSalary { get; set; }  // Encrypted via value converter
    public string Currency { get; set; } = "USD";
    public CompensationType CompensationType { get; set; }
    public decimal? BonusPercentage { get; set; }
    public decimal? CommissionRate { get; set; }
    public string? ChangeReason { get; set; }
    public Guid? ApprovedBy { get; set; }
    public bool IsCurrent { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }  // Optimistic locking

    // Navigation
    public ICollection<SalaryHistory> SalaryHistories { get; set; }
}
```

**Database Table**:
```sql
CREATE TABLE compensation_records (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    employee_id UUID NOT NULL,
    effective_date TIMESTAMP WITH TIME ZONE NOT NULL,
    base_salary_encrypted TEXT NOT NULL,
    currency VARCHAR(3) NOT NULL DEFAULT 'USD',
    compensation_type INTEGER NOT NULL DEFAULT 0,
    bonus_percentage DECIMAL(5,2),
    commission_rate DECIMAL(5,2),
    change_reason TEXT,
    approved_by UUID,
    is_current BOOLEAN NOT NULL DEFAULT TRUE,
    created_date TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    modified_date TIMESTAMP WITH TIME ZONE,
    row_version BYTEA NOT NULL
);

CREATE INDEX idx_comp_records_employee ON compensation_records(employee_id);
CREATE UNIQUE INDEX idx_comp_records_current ON compensation_records(employee_id, is_current) WHERE is_current = TRUE;
CREATE INDEX idx_comp_records_effective ON compensation_records(employee_id, effective_date DESC);
```

**Validation Rules**:
- `EmployeeId`: Required, must be valid UUID
- `EffectiveDate`: Required, must be today or future date
- `BaseSalary`: Required, must be > 0
- `Currency`: Required, must be 3-letter ISO 4217 code (e.g., USD, EUR, GBP)
- `CompensationType`: Required, valid enum value (0-3)
- `BonusPercentage`: Optional, must be 0-100 if provided
- `CommissionRate`: Optional, must be 0-100 if provided
- `ChangeReason`: Optional but recommended, max 500 characters
- `ApprovedBy`: Required for all changes
- `IsCurrent`: Only one record per employee_id can be is_current=true

**Business Rules**:
- When creating a new compensation record, mark previous record as `is_current = false`
- Changes > 25% increase trigger flag for additional approval
- Currency changes require special approval workflow
- Effective date cannot be backdated

**Indexes**:
- `employee_id`: For lookups by employee
- `(employee_id, is_current)`: Unique constraint + fast current compensation query
- `(employee_id, effective_date DESC)`: For compensation history queries

---

### 2. SalaryHistory

Represents a historical record of compensation changes.

**C# Entity**:
```csharp
public class SalaryHistory
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid CompensationRecordId { get; set; }
    public decimal PreviousSalary { get; set; }  // Encrypted via value converter
    public decimal NewSalary { get; set; }  // Encrypted via value converter
    public decimal ChangeAmount { get; set; }
    public decimal ChangePercentage { get; set; }
    public DateTime EffectiveDate { get; set; }
    public string ChangeType { get; set; }  // Promotion, Merit, Adjustment, Initial
    public Guid ChangedBy { get; set; }
    public DateTime CreatedDate { get; set; }

    // Navigation
    public CompensationRecord CompensationRecord { get; set; }
}
```

**Database Table**:
```sql
CREATE TABLE salary_histories (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    employee_id UUID NOT NULL,
    compensation_record_id UUID NOT NULL REFERENCES compensation_records(id) ON DELETE CASCADE,
    previous_salary_encrypted TEXT NOT NULL,
    new_salary_encrypted TEXT NOT NULL,
    change_amount DECIMAL(12,2) NOT NULL,
    change_percentage DECIMAL(5,2) NOT NULL,
    effective_date TIMESTAMP WITH TIME ZONE NOT NULL,
    change_type VARCHAR(50) NOT NULL,
    changed_by UUID NOT NULL,
    created_date TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_salary_hist_employee ON salary_histories(employee_id, effective_date DESC);
CREATE INDEX idx_salary_hist_comp_record ON salary_histories(compensation_record_id);
```

**Validation Rules**:
- `EmployeeId`: Required, must match compensation record
- `CompensationRecordId`: Required, must exist in compensation_records
- `PreviousSalary`: Required (0 for initial compensation)
- `NewSalary`: Required, must be > 0
- `ChangeAmount`: Calculated as NewSalary - PreviousSalary
- `ChangePercentage`: Calculated as ((NewSalary - PreviousSalary) / PreviousSalary) * 100
- `ChangeType`: Required, one of: "Promotion", "Merit", "Adjustment", "Initial", "Correction"
- `ChangedBy`: Required, user who approved the change

**Business Rules**:
- Automatically created when compensation record is created
- Cannot be modified or deleted (audit trail)
- Change calculations are server-side, not client-provided
- For initial compensation, previous_salary = 0, change_percentage = 0

**Indexes**:
- `(employee_id, effective_date DESC)`: For compensation history queries
- `compensation_record_id`: For joining with compensation records

---

### 3. Benefit

Represents an available benefits program offered by the organization.

**C# Entity**:
```csharp
public class Benefit
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public BenefitType BenefitType { get; set; }
    public decimal? EmployerContribution { get; set; }
    public decimal? EmployeeContribution { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // Navigation
    public ICollection<BenefitsEnrollment> Enrollments { get; set; }
}
```

**Database Table**:
```sql
CREATE TABLE benefits (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL,
    description TEXT,
    benefit_type INTEGER NOT NULL,
    employer_contribution DECIMAL(10,2),
    employee_contribution DECIMAL(10,2),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_date TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    modified_date TIMESTAMP WITH TIME ZONE
);

CREATE INDEX idx_benefits_active ON benefits(is_active) WHERE is_active = TRUE;
CREATE INDEX idx_benefits_type ON benefits(benefit_type);
```

**Validation Rules**:
- `Name`: Required, max 100 characters, unique
- `Description`: Optional, descriptive text
- `BenefitType`: Required, valid enum value (0-8)
- `EmployerContribution`: Optional, >= 0 if provided
- `EmployeeContribution`: Optional, >= 0 if provided
- `IsActive`: Default true, soft delete pattern

**Business Rules**:
- Benefits are never hard deleted (maintain historical integrity)
- Inactive benefits cannot be enrolled in
- Active benefits are cached in Redis (24h TTL)
- Contribution amounts are defaults, can be overridden at enrollment

**Indexes**:
- `is_active`: Filter for active benefits
- `benefit_type`: Group benefits by type

---

### 4. BenefitsEnrollment

Represents an employee's enrollment in a specific benefit program.

**C# Entity**:
```csharp
public class BenefitsEnrollment
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid BenefitId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public EnrollmentStatus Status { get; set; }
    public decimal? EmployeeContribution { get; set; }
    public string? CoverageLevel { get; set; }  // Individual, Family, Spouse, Spouse+Children
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // Navigation
    public Benefit Benefit { get; set; }
    public ICollection<Dependent> Dependents { get; set; }
}
```

**Database Table**:
```sql
CREATE TABLE benefits_enrollments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    employee_id UUID NOT NULL,
    benefit_id UUID NOT NULL REFERENCES benefits(id),
    enrollment_date TIMESTAMP WITH TIME ZONE NOT NULL,
    termination_date TIMESTAMP WITH TIME ZONE,
    status INTEGER NOT NULL DEFAULT 0,
    employee_contribution DECIMAL(10,2),
    coverage_level VARCHAR(50),
    created_date TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    modified_date TIMESTAMP WITH TIME ZONE,
    UNIQUE (employee_id, benefit_id, status) WHERE status = 0  -- Only one active enrollment per benefit
);

CREATE INDEX idx_benefits_enroll_employee ON benefits_enrollments(employee_id);
CREATE INDEX idx_benefits_enroll_status ON benefits_enrollments(employee_id, status);
CREATE INDEX idx_benefits_enroll_benefit ON benefits_enrollments(benefit_id);
```

**Validation Rules**:
- `EmployeeId`: Required, must be active employee
- `BenefitId`: Required, must exist and be active at enrollment time
- `EnrollmentDate`: Required, typically today or start of open enrollment period
- `TerminationDate`: Optional, must be >= EnrollmentDate if provided
- `Status`: Required, valid enum value (0-3)
- `EmployeeContribution`: Optional, must be >= 0 if provided
- `CoverageLevel`: Optional for some benefits, required for health/dental/vision

**Business Rules**:
- Only one active enrollment per employee+benefit combination
- Waiting period (90 days) for some benefits
- Open enrollment period restrictions (except qualifying life events)
- Termination automatically occurs on employee termination
- Pending enrollments automatically rejected on employee termination

**State Transitions**:
```
Pending → Active (waiting period expires)
Pending → Terminated (employee terminated, benefit cancelled)
Active → Terminated (employee terminated, voluntary cancellation)
Active → OnHold (payment issue, compliance issue)
OnHold → Active (issue resolved)
OnHold → Terminated (issue not resolved)
```

**Indexes**:
- `employee_id`: Lookup enrollments by employee
- `(employee_id, status)`: Filter active/pending enrollments
- `benefit_id`: Impact analysis for benefit changes

---

### 5. Dependent

Represents a family member covered under an employee's benefit enrollment.

**C# Entity**:
```csharp
public class Dependent
{
    public Guid Id { get; set; }
    public Guid BenefitsEnrollmentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DependentRelationship Relationship { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? NationalId { get; set; }  // Encrypted via value converter
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // Navigation
    public BenefitsEnrollment Enrollment { get; set; }
}
```

**Database Table**:
```sql
CREATE TABLE dependents (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    benefits_enrollment_id UUID NOT NULL REFERENCES benefits_enrollments(id) ON DELETE CASCADE,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    relationship INTEGER NOT NULL,
    date_of_birth DATE NOT NULL,
    national_id_encrypted TEXT,
    created_date TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    modified_date TIMESTAMP WITH TIME ZONE
);

CREATE INDEX idx_dependents_enrollment ON dependents(benefits_enrollment_id);
```

**Validation Rules**:
- `BenefitsEnrollmentId`: Required, must exist
- `FirstName`: Required, max 100 characters
- `LastName`: Required, max 100 characters
- `Relationship`: Required, valid enum value (0-4)
- `DateOfBirth`: Required, must be in the past
- `NationalId`: Optional, encrypted if provided

**Business Rules**:
- Cascade delete when enrollment is deleted
- Relationship must be valid for benefit type (e.g., only Spouse/Child for health insurance)
- Age verification for dependent children (typically <26 years)
- National ID required for some benefit types (e.g., life insurance)

**Indexes**:
- `benefits_enrollment_id`: Cascade deletes, lookup dependents by enrollment

---

### 6. BulkJob

Tracks asynchronous bulk operations.

**C# Entity**:
```csharp
public class BulkJob
{
    public Guid Id { get; set; }
    public string JobType { get; set; }  // SalaryIncrease, BonusAdjustment, etc.
    public BulkJobStatus Status { get; set; }
    public string Parameters { get; set; }  // JSON: {departmentId, increasePercentage, ...}
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public string? ErrorDetails { get; set; }  // JSON: [{employeeId, error}, ...]
    public Guid StartedBy { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? WebhookUrl { get; set; }  // Optional callback URL
}
```

**Database Table**:
```sql
CREATE TABLE bulk_jobs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    job_type VARCHAR(50) NOT NULL,
    status INTEGER NOT NULL DEFAULT 0,
    parameters JSONB NOT NULL,
    success_count INTEGER NOT NULL DEFAULT 0,
    failure_count INTEGER NOT NULL DEFAULT 0,
    error_details JSONB,
    started_by UUID NOT NULL,
    started_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    completed_at TIMESTAMP WITH TIME ZONE,
    webhook_url TEXT
);

CREATE INDEX idx_bulk_jobs_status ON bulk_jobs(status);
CREATE INDEX idx_bulk_jobs_started_by ON bulk_jobs(started_by);
CREATE INDEX idx_bulk_jobs_started_at ON bulk_jobs(started_at DESC);
```

**Validation Rules**:
- `JobType`: Required, one of: "SalaryIncrease", "BonusAdjustment", "BenefitsEnrollment"
- `Status`: Required, valid enum value (0-3)
- `Parameters`: Required, valid JSON matching job type schema
- `StartedBy`: Required, user who initiated the job
- `WebhookUrl`: Optional, must be valid HTTPS URL if provided

**State Transitions**:
```
Pending → InProgress (job processing starts)
InProgress → Completed (all employees processed)
InProgress → Failed (unrecoverable error)
InProgress → PartiallyCompleted (some employees failed)
```

**Indexes**:
- `status`: Filter pending/in-progress jobs
- `started_by`: User's job history
- `started_at DESC`: Recent jobs first

---

## Enumerations

### CompensationType
```csharp
public enum CompensationType
{
    Salary = 0,      // Fixed annual salary
    Hourly = 1,      // Hourly wage
    Contract = 2,    // Contract-based
    Commission = 3   // Commission-based
}
```

### BenefitType
```csharp
public enum BenefitType
{
    HealthInsurance = 0,
    DentalInsurance = 1,
    VisionInsurance = 2,
    LifeInsurance = 3,
    Retirement401k = 4,
    StockOptions = 5,
    PaidTimeOff = 6,
    WellnessProgram = 7,
    EducationAssistance = 8
}
```

### EnrollmentStatus
```csharp
public enum EnrollmentStatus
{
    Active = 0,      // Currently enrolled and active
    Pending = 1,     // Waiting period not yet complete
    Terminated = 2,  // Enrollment ended
    OnHold = 3       // Temporarily suspended
}
```

### DependentRelationship
```csharp
public enum DependentRelationship
{
    Spouse = 0,
    Child = 1,
    DomesticPartner = 2,
    Parent = 3,
    Other = 4
}
```

### BulkJobStatus
```csharp
public enum BulkJobStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Failed = 3,
    PartiallyCompleted = 4
}
```

---

## Encryption Implementation

### Encrypted Fields
- `CompensationRecord.BaseSalary`
- `SalaryHistory.PreviousSalary`
- `SalaryHistory.NewSalary`
- `Dependent.NationalId`

### Value Converter Configuration
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    var encryptionService = new EncryptionService(configuration["Encryption:Key"]);
    var converter = new EncryptionValueConverter(encryptionService);

    // CompensationRecord.BaseSalary
    modelBuilder.Entity<CompensationRecord>()
        .Property(e => e.BaseSalary)
        .HasColumnName("base_salary_encrypted")
        .HasConversion(converter);

    // SalaryHistory encrypted fields
    modelBuilder.Entity<SalaryHistory>()
        .Property(e => e.PreviousSalary)
        .HasColumnName("previous_salary_encrypted")
        .HasConversion(converter);

    modelBuilder.Entity<SalaryHistory>()
        .Property(e => e.NewSalary)
        .HasColumnName("new_salary_encrypted")
        .HasConversion(converter);

    // Dependent.NationalId
    modelBuilder.Entity<Dependent>()
        .Property(e => e.NationalId)
        .HasColumnName("national_id_encrypted")
        .HasConversion(converter);
}
```

---

## Relationships Summary

| Parent Entity | Child Entity | Relationship | Cascade |
|---------------|--------------|--------------|---------|
| CompensationRecord | SalaryHistory | 1:N | Delete |
| Benefit | BenefitsEnrollment | 1:N | Restrict (don't delete active benefits) |
| BenefitsEnrollment | Dependent | 1:N | Delete |

---

## Data Retention & Audit

### Retention Policies
- **CompensationRecords**: Indefinite retention for compliance
- **SalaryHistories**: Indefinite retention for audit trail
- **BenefitsEnrollments**: Indefinite retention for compliance
- **Dependents**: Indefinite retention (tied to enrollments)
- **BulkJobs**: Retain for 2 years (operational history)

### Soft Delete Pattern
- **Benefits**: Use `IsActive` flag (never hard delete)
- **All others**: Hard delete not allowed for audit reasons

### Audit Fields
All entities include:
- `CreatedDate`: Timestamp of record creation
- `ModifiedDate`: Timestamp of last modification
- `RowVersion`: For optimistic locking (CompensationRecord only)

---

## Database Migrations

### Initial Migration
- Create all 6 tables
- Create all indexes and constraints
- Seed default benefits (Health, Dental, Vision, Life, 401k)

### Future Migrations
- Add columns with defaults (backward compatible)
- Never drop columns (soft deprecation)
- Use database views for complex queries
- Maintain all audit trails

---

**Next**: See [contracts/](./contracts/) for API endpoint definitions and event schemas.
