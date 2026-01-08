using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Application.Common.Mediator;

namespace Maliev.CompensationService.Application.Queries.Handlers;

/// <summary>
/// Handler for the <see cref="GetCompensationAnalysisQuery"/> query
/// </summary>
public class GetCompensationAnalysisQueryHandler : IRequestHandler<GetCompensationAnalysisQuery, CompensationAnalysisDto>
{
    private readonly ICompensationRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCompensationAnalysisQueryHandler"/> class
    /// </summary>
    /// <param name="repository">The compensation repository</param>
    public GetCompensationAnalysisQueryHandler(ICompensationRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<CompensationAnalysisDto> Handle(GetCompensationAnalysisQuery request, CancellationToken cancellationToken)
    {
        var records = await _repository.GetAllCurrentAsync(request.DepartmentId, cancellationToken);
        var recordList = records.ToList();

        if (!recordList.Any())
        {
            return new CompensationAnalysisDto();
        }

        return new CompensationAnalysisDto
        {
            TotalEmployees = recordList.Count,
            TotalAnnualBudget = recordList.Sum(r => r.BaseSalary),
            AverageSalary = Math.Round(recordList.Average(r => r.BaseSalary), 2),
            MinimumSalary = recordList.Min(r => r.BaseSalary),
            MaximumSalary = recordList.Max(r => r.BaseSalary)
        };
    }
}
