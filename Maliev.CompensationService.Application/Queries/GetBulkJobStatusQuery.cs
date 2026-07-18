using Maliev.CompensationService.Application.Common.Mediator;
using Maliev.CompensationService.Application.DTOs;

namespace Maliev.CompensationService.Application.Queries;

/// <summary>
/// Query to retrieve the current status of a bulk job
/// </summary>
/// <param name="JobId">Unique identifier of the job</param>
public record GetBulkJobStatusQuery(Guid JobId) : IRequest<BulkJobStatusDto?>;
