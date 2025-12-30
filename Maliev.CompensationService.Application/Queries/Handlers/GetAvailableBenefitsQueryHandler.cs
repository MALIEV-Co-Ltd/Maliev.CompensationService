using System.Text.Json;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Application.Mappers;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Maliev.CompensationService.Application.Queries.Handlers;

/// <summary>
/// Handler for the <see cref="GetAvailableBenefitsQuery"/> query
/// </summary>
public class GetAvailableBenefitsQueryHandler : IRequestHandler<GetAvailableBenefitsQuery, IEnumerable<BenefitDto>>
{
    private readonly IBenefitsRepository _repository;
    private readonly IDistributedCache _cache;
    private const string CacheKey = "available_benefits";

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAvailableBenefitsQueryHandler"/> class
    /// </summary>
    /// <param name="repository">The benefits repository</param>
    /// <param name="cache">The distributed cache</param>
    public GetAvailableBenefitsQueryHandler(IBenefitsRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<BenefitDto>> Handle(GetAvailableBenefitsQuery request, CancellationToken cancellationToken)
    {
        var cachedData = await _cache.GetStringAsync(CacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<IEnumerable<BenefitDto>>(cachedData) ?? Enumerable.Empty<BenefitDto>();
        }

        var benefits = await _repository.GetAllActiveAsync(cancellationToken);
        var dtos = benefits.Select(b => b.ToDto()).ToList();

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
        };

        await _cache.SetStringAsync(CacheKey, JsonSerializer.Serialize(dtos), options, cancellationToken);

        return dtos;
    }
}
