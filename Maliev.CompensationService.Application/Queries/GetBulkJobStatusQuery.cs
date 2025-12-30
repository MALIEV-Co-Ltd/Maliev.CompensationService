using Maliev.CompensationService.Application.DTOs;
using MediatR;

namespace Maliev.CompensationService.Application.Queries;

/// <summary>
/// Query to retrieve the current status of a bulk job
/// </summary>
/// <param name="JobId">Unique identifier of the job</param>
public record GetBulkJobStatusQuery(Guid JobId) : IRequest<BulkJobStatusDto?>;
