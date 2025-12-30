using Maliev.CompensationService.Application.DTOs;
using MediatR;

namespace Maliev.CompensationService.Application.Queries;

/// <summary>
/// Query to retrieve all available active benefits
/// </summary>
public record GetAvailableBenefitsQuery() : IRequest<IEnumerable<BenefitDto>>;
