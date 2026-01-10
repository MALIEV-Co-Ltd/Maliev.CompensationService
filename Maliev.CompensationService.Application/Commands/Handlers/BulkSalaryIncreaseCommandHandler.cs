using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Domain.IntegrationEvents;
using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Domain.Enums;
using MassTransit;
using Maliev.CompensationService.Application.Common.Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Maliev.CompensationService.Domain.Events;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Commands;

namespace Maliev.CompensationService.Application.Commands.Handlers;

/// <summary>
/// Handler for BulkSalaryIncreaseCommand.
/// </summary>
public class BulkSalaryIncreaseCommandHandler : IRequestHandler<BulkSalaryIncreaseCommand, BulkSalaryIncreaseResultDto>
{
    private readonly ICompensationRepository _compRepository;
    private readonly ISalaryHistoryRepository _historyRepository;
    private readonly IBulkJobRepository _bulkJobRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<BulkSalaryIncreaseCommandHandler> _logger;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="BulkSalaryIncreaseCommandHandler"/> class.
    /// </summary>
    public BulkSalaryIncreaseCommandHandler(
        ICompensationRepository compRepository,
        ISalaryHistoryRepository historyRepository,
        IBulkJobRepository bulkJobRepository,
        IPublishEndpoint publishEndpoint,
        ILogger<BulkSalaryIncreaseCommandHandler> logger,
        IConfiguration configuration)
    {
        _compRepository = compRepository;
        _historyRepository = historyRepository;
        _bulkJobRepository = bulkJobRepository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
        _configuration = configuration;
    }

    /// <inheritdoc/>
    public async Task<BulkSalaryIncreaseResultDto> Handle(BulkSalaryIncreaseCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing bulk salary increase of {Percentage}% for department {DeptId}",
            request.PercentageIncrease, request.DepartmentId);

        // 1. Create Job Record
        var jobId = Guid.NewGuid();
        var job = new BulkJob
        {
            Id = jobId,
            JobType = "SalaryIncrease",
            Status = BulkJobStatus.InProgress,
            Parameters = JsonSerializer.Serialize(new { request.DepartmentId, request.PercentageIncrease, request.EffectiveDate }),
            StartedBy = request.InitiatedByUserId,
            StartedAt = DateTime.UtcNow,
            SuccessCount = 0,
            FailureCount = 0
        };
        await _bulkJobRepository.AddAsync(job, cancellationToken);

        var threshold = _configuration.GetValue<decimal>("CompensationSettings:HighIncreaseThreshold", 25);
        var previewRecords = await _compRepository.GetAllCurrentAsync(request.DepartmentId, cancellationToken);
        var defaultCurrency = previewRecords.FirstOrDefault()?.Currency ?? "N/A";

        if (request.PreviewOnly)
        {
            // Just count eligibility and return
            job.Status = BulkJobStatus.Completed;
            job.SuccessCount = previewRecords.Count(); // In preview, success count is just matching records
            job.CompletedAt = DateTime.UtcNow;
            await _bulkJobRepository.UpdateAsync(job, cancellationToken);

            return new BulkSalaryIncreaseResultDto
            {
                JobId = jobId,
                TotalEmployeesProcessed = job.SuccessCount,
                TotalIncreaseAmount = previewRecords.Sum(r => r.BaseSalary * (request.PercentageIncrease / 100)),
                Currency = defaultCurrency
            };
        }

        // 2. Fetch Eligible Employees
        var eligibleRecords = previewRecords;
        var processedCount = 0;
        var failedCount = 0;
        decimal totalIncrease = 0;
        var errors = new List<object>();

        // 3. Process Each Employee in batches/transactions
        foreach (var record in eligibleRecords)
        {
            try
            {
                var oldSalary = record.BaseSalary;
                var increaseAmount = oldSalary * (request.PercentageIncrease / 100);
                var newSalary = oldSalary + increaseAmount;

                                // Create new record

                                var newRecord = new CompensationRecord

                                {

                                    Id = Guid.NewGuid(),

                                    EmployeeId = record.EmployeeId,

                
                    DepartmentId = record.DepartmentId,
                    BaseSalary = newSalary,
                    Currency = record.Currency,
                    CompensationType = record.CompensationType,
                    BonusPercentage = record.BonusPercentage,
                    CommissionRate = record.CommissionRate,
                    EffectiveDate = request.EffectiveDate,
                    ChangeReason = request.Reason,
                    ApprovedBy = request.InitiatedByUserId,
                    IsCurrent = true,
                    CreatedDate = DateTime.UtcNow
                };
                await _compRepository.AddAsync(newRecord, cancellationToken);

                // History
                var history = new SalaryHistory
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = record.EmployeeId,
                    CompensationRecordId = newRecord.Id,
                    PreviousSalary = oldSalary,
                    NewSalary = newSalary,
                    ChangeAmount = increaseAmount,
                    ChangePercentage = request.PercentageIncrease,
                    IsHighIncrease = request.PercentageIncrease > threshold,
                    EffectiveDate = request.EffectiveDate,
                    ChangeType = "BulkIncrease",
                    ChangedBy = request.InitiatedByUserId,
                    CreatedDate = DateTime.UtcNow
                };
                await _historyRepository.AddAsync(history, cancellationToken);

                // Publish Event
                await _publishEndpoint.Publish(new SalaryChangedEvent(
                    record.EmployeeId,
                    newRecord.Id,
                    newSalary,
                    oldSalary,
                    request.PercentageIncrease,
                    request.EffectiveDate,
                    request.Reason
                ), cancellationToken);

                processedCount++;
                totalIncrease += increaseAmount;
            }
            catch (Exception ex)
            {
                failedCount++;
                errors.Add(new { EmployeeId = record.EmployeeId, Error = ex.Message });
                _logger.LogError(ex, "Failed to process bulk increase for employee {EmployeeId}", record.EmployeeId);
            }
        }

        // 4. Update Job Status
        job.SuccessCount = processedCount;
        job.FailureCount = failedCount;
        job.CompletedAt = DateTime.UtcNow;
        job.Status = failedCount > 0 ? BulkJobStatus.PartiallyCompleted : BulkJobStatus.Completed;
        if (failedCount == eligibleRecords.Count() && eligibleRecords.Any()) job.Status = BulkJobStatus.Failed;
        if (errors.Any()) job.ErrorDetails = JsonSerializer.Serialize(errors);

        await _bulkJobRepository.UpdateAsync(job, cancellationToken);

        // 5. Publish Completion Event
        await _publishEndpoint.Publish(new BulkSalaryIncreaseCompletedEvent(
            job.Id,
            processedCount,
            failedCount,
            totalIncrease
        ), cancellationToken);

        return new BulkSalaryIncreaseResultDto
        {
            JobId = jobId,
            TotalEmployeesProcessed = processedCount,
            TotalIncreaseAmount = totalIncrease,
            Currency = defaultCurrency
        };
    }
}