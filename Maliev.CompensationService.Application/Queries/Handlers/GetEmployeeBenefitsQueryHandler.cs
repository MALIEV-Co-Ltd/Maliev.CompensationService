using Maliev.CompensationService.Application.Common.Mediator;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Application.Mappers;

namespace Maliev.CompensationService.Application.Queries.Handlers;

/// <summary>
/// Handler for the <see cref="GetEmployeeBenefitsQuery"/> query
/// </summary>
public class GetEmployeeBenefitsQueryHandler : IRequestHandler<GetEmployeeBenefitsQuery, IEnumerable<BenefitsEnrollmentDto>>
{
    private readonly IBenefitsRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetEmployeeBenefitsQueryHandler"/> class
    /// </summary>
    /// <param name="repository">The benefits repository</param>
    public GetEmployeeBenefitsQueryHandler(IBenefitsRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<BenefitsEnrollmentDto>> Handle(GetEmployeeBenefitsQuery request, CancellationToken cancellationToken)
    {
        var enrollments = await _repository.GetEnrollmentsByEmployeeIdAsync(request.EmployeeId, cancellationToken);
        return enrollments.Select(e => e.ToDto());
    }
}
