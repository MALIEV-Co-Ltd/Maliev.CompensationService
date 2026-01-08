using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Application.Mappers;
using Maliev.CompensationService.Application.Common.Mediator;

namespace Maliev.CompensationService.Application.Queries.Handlers;

/// <summary>
/// Handler for the <see cref="GetCompensationDetailsQuery"/> query
/// </summary>
public class GetCompensationDetailsQueryHandler : IRequestHandler<GetCompensationDetailsQuery, CompensationRecordDto?>
{
    private readonly ICompensationRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCompensationDetailsQueryHandler"/> class
    /// </summary>
    /// <param name="repository">The compensation repository</param>
    public GetCompensationDetailsQueryHandler(ICompensationRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles the query to retrieve current compensation details
    /// </summary>
    /// <param name="request">The query containing employee identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The compensation record DTO, or null if not found</returns>
    public async Task<CompensationRecordDto?> Handle(GetCompensationDetailsQuery request, CancellationToken cancellationToken)
    {
        var record = await _repository.GetByEmployeeIdAsync(request.EmployeeId, cancellationToken);
        return record?.ToDto();
    }
}