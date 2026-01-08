using Maliev.Aspire.ServiceDefaults.Authorization;
using Maliev.CompensationService.Application.Commands;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Queries;
using Maliev.CompensationService.Domain.Authorization;
using Maliev.CompensationService.Application.Common.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Maliev.CompensationService.Api.Controllers;

/// <summary>
/// Controller for managing asynchronous bulk operations
/// </summary>
[ApiController]
[Route("compensation/v1/bulk")]
public class BulkOperationsController : ControllerBase
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
        var jobId = await _mediator.Send(command, cancellationToken);

        return AcceptedAtAction(nameof(GetBulkJobStatus), new { jobId }, new { jobId });
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

    private Guid GetCurrentUserId()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var subClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(subClaim, out var userId))
            {
                return userId;
            }
        }
        return Guid.Empty;
    }
}
