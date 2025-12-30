using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Application.Mappers;
using MediatR;

namespace Maliev.CompensationService.Application.Queries.Handlers;

/// <summary>
/// Handler for the <see cref="GetCompensationHistoryQuery"/> query
/// </summary>
public class GetCompensationHistoryQueryHandler : IRequestHandler<GetCompensationHistoryQuery, IEnumerable<SalaryHistoryDto>>
{
    private readonly ISalaryHistoryRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCompensationHistoryQueryHandler"/> class
    /// </summary>
    /// <param name="repository">The salary history repository</param>
    public GetCompensationHistoryQueryHandler(ISalaryHistoryRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles the query to retrieve complete compensation history for an employee
    /// </summary>
    /// <param name="request">The query containing employee identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of salary history DTOs</returns>
    public async Task<IEnumerable<SalaryHistoryDto>> Handle(GetCompensationHistoryQuery request, CancellationToken cancellationToken)
    {
        var history = await _repository.GetByEmployeeIdAsync(request.EmployeeId, cancellationToken);
        return history.Select(h => h.ToDto());
    }
}
