using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Application.Mappers;
using Maliev.CompensationService.Domain.Entities;
using Maliev.CompensationService.Domain.Events;
using MassTransit;
using MediatR;

namespace Maliev.CompensationService.Application.Commands.Handlers;

/// <summary>
/// Handler for the <see cref="RecordCompensationChangeCommand"/> command
/// </summary>
public class RecordCompensationChangeCommandHandler : IRequestHandler<RecordCompensationChangeCommand, SalaryHistoryDto>
{
    private readonly ICompensationRepository _compRepository;
    private readonly ISalaryHistoryRepository _historyRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordCompensationChangeCommandHandler"/> class
    /// </summary>
    /// <param name="compRepository">The compensation repository</param>
    /// <param name="historyRepository">The salary history repository</param>
    /// <param name="publishEndpoint">The event publishing endpoint</param>
    public RecordCompensationChangeCommandHandler(
        ICompensationRepository compRepository,
        ISalaryHistoryRepository historyRepository,
        IPublishEndpoint publishEndpoint)
    {
        _compRepository = compRepository;
        _historyRepository = historyRepository;
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    /// Handles the compensation change command
    /// </summary>
    /// <param name="request">The command containing change details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created salary history DTO</returns>
    public async Task<SalaryHistoryDto> Handle(RecordCompensationChangeCommand request, CancellationToken cancellationToken)
    {
        var currentRecord = await _compRepository.GetByEmployeeIdAsync(request.EmployeeId, cancellationToken);
        decimal previousSalary = currentRecord?.BaseSalary ?? 0;

        if (currentRecord != null)
        {
            currentRecord.IsCurrent = false;
            currentRecord.ModifiedDate = DateTime.UtcNow;
            await _compRepository.UpdateAsync(currentRecord, cancellationToken);
        }

        var newRecord = new CompensationRecord
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            DepartmentId = currentRecord?.DepartmentId ?? Guid.Empty,
            BaseSalary = request.Data.NewBaseSalary,
            Currency = request.Data.Currency,
            CompensationType = request.Data.CompensationType,
            BonusPercentage = request.Data.BonusPercentage,
            CommissionRate = request.Data.CommissionRate,
            EffectiveDate = request.Data.EffectiveDate,
            ChangeReason = request.Data.ChangeReason,
            ApprovedBy = request.ChangedBy,
            IsCurrent = true,
            CreatedDate = DateTime.UtcNow
        };

        await _compRepository.AddAsync(newRecord, cancellationToken);

        decimal changeAmount = request.Data.NewBaseSalary - previousSalary;
        decimal changePercentage = previousSalary > 0 
            ? (changeAmount / previousSalary) * 100 
            : 0;

        var history = new SalaryHistory
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            CompensationRecordId = newRecord.Id,
            PreviousSalary = previousSalary,
            NewSalary = request.Data.NewBaseSalary,
            ChangeAmount = changeAmount,
            ChangePercentage = Math.Round(changePercentage, 2),
            IsHighIncrease = changePercentage > 25,
            EffectiveDate = request.Data.EffectiveDate,
            ChangeType = request.Data.ChangeType,
            ChangedBy = request.ChangedBy,
            CreatedDate = DateTime.UtcNow
        };

        await _historyRepository.AddAsync(history, cancellationToken);

        await _publishEndpoint.Publish(new SalaryChangedEvent(
            request.EmployeeId,
            newRecord.Id,
            request.Data.NewBaseSalary,
            previousSalary,
            Math.Round(changePercentage, 2),
            request.Data.EffectiveDate,
            request.Data.ChangeReason ?? string.Empty
        ), cancellationToken);

        return history.ToDto();
    }
}