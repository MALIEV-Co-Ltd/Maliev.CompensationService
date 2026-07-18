using Asp.Versioning;
using Maliev.Aspire.ServiceDefaults.Authorization;
using Maliev.CompensationService.Application.Commands;
using Maliev.CompensationService.Application.Common.Mediator;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Queries;
using Maliev.CompensationService.Domain.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maliev.CompensationService.Api.Controllers;

/// <summary>
/// Controller for managing asynchronous bulk operations
/// </summary>
[ApiController]
[ApiVersion("1")]
[Route("compensation/v{version:apiVersion}/bulk")]
public class BulkOperationsController : BaseController
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="BulkOperationsController"/> class
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    public BulkOperationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Initiates a bulk salary increase operation
    /// </summary>
    /// <param name="data">The bulk increase parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A 202 Accepted response with the job identifier</returns>
    [HttpPost("salary-increases")]
    [RequirePermission(CompensationPermissions.Admin)]
    public async Task<IActionResult> BulkSalaryIncrease(
        [FromBody] BulkSalaryIncreaseDto data,
        CancellationToken cancellationToken)
    {
        var startedBy = GetCurrentUserId();
        var command = new BulkSalaryIncreaseCommand
        {
            DepartmentId = data.DepartmentId,
            PercentageIncrease = data.PercentageIncrease,
            Reason = data.Reason,
            EffectiveDate = data.EffectiveDate,
            PreviewOnly = data.PreviewOnly,
            InitiatedByUserId = startedBy
        };
        var result = await _mediator.Send(command, cancellationToken);

        return AcceptedAtAction(nameof(GetBulkJobStatus), new { jobId = result.JobId }, result);
    }

    /// <summary>
    /// Gets the current status of a bulk job
    /// </summary>
    /// <param name="jobId">Unique identifier of the job</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The current job status and progress</returns>
    [HttpGet("jobs/{jobId:guid}")]
    [RequirePermission(CompensationPermissions.Admin)]
    public async Task<ActionResult<BulkJobStatusDto>> GetBulkJobStatus(
        Guid jobId,
        CancellationToken cancellationToken)
    {
        var query = new GetBulkJobStatusQuery(jobId);
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}

