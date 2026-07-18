using Maliev.CompensationService.Application.Common.Mediator;
using Maliev.CompensationService.Application.DTOs;

namespace Maliev.CompensationService.Application.Queries;

/// <summary>
/// Query to retrieve all available active benefits
/// </summary>
public record GetAvailableBenefitsQuery() : IRequest<IEnumerable<BenefitDto>>;
