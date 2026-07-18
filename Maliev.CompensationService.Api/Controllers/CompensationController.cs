using Asp.Versioning;
using Maliev.Aspire.ServiceDefaults.Authorization;
using Maliev.CompensationService.Application.Commands;
using Maliev.CompensationService.Application.Common.Mediator;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Queries;
using Maliev.CompensationService.Domain.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maliev.CompensationService.Api.Controllers;

/// <summary>
/// Controller for managing employee compensation records, history, and benefits.
/// </summary>
[ApiController]
[ApiVersion("1")]
[Route("compensation/v{version:apiVersion}/employees")]
public class CompensationController : BaseController
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompensationController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance.</param>
    public CompensationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets the current active compensation details for a specific employee.
    /// </summary>
    /// <param name="employeeId">Unique identifier of the employee.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The current compensation details.</returns>
    [HttpGet("{employeeId:guid}/compensation")]
    [RequirePermission(CompensationPermissions.Read)]
    public async Task<ActionResult<CompensationRecordDto>> GetCompensationDetails(Guid employeeId, CancellationToken cancellationToken)
    {
        var query = new GetCompensationDetailsQuery(employeeId);
        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound();
        }

        if (!HasPermission(CompensationPermissions.ReadSensitive))
        {
            result.BaseSalary = 0;
        }

        return Ok(result);
    }

    /// <summary>
    /// Records a change in an employee's compensation.
    /// </summary>
    /// <param name="employeeId">Unique identifier of the employee.</param>
    /// <param name="data">The compensation change details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created salary history record.</returns>
    [HttpPost("{employeeId:guid}/compensation")]
    [RequirePermission(CompensationPermissions.Update)]
    public async Task<ActionResult<SalaryHistoryDto>> RecordCompensationChange(
        Guid employeeId,
        [FromBody] RecordCompensationChangeDto data,
        CancellationToken cancellationToken)
    {
        try
        {
            var changedBy = GetCurrentUserId();
            var command = new RecordCompensationChangeCommand(employeeId, data, changedBy);
            var result = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetCompensationDetails), new { employeeId }, result);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new { message = "The record was modified by another user." });
        }
    }

    /// <summary>
    /// Gets the complete compensation history for a specific employee.
    /// </summary>
    /// <param name="employeeId">Unique identifier of the employee.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of historical salary records.</returns>
    [HttpGet("{employeeId:guid}/history")]
    [RequirePermission(CompensationPermissions.Read)]
    public async Task<ActionResult<IEnumerable<SalaryHistoryDto>>> GetCompensationHistory(Guid employeeId, CancellationToken cancellationToken)
    {
        var query = new GetCompensationHistoryQuery(employeeId);
        var result = await _mediator.Send(query, cancellationToken);

        if (!HasPermission(CompensationPermissions.ReadSensitive))
        {
            foreach (var history in result)
            {
                history.PreviousSalary = 0;
                history.NewSalary = 0;
            }
        }

        return Ok(result);
    }

    /// <summary>
    /// Applies a bulk salary increase.
    /// </summary>

    /// <param name="request">Bulk increase request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the bulk operation.</returns>
    [HttpPost("bulk/salary-increase")]
    [RequirePermission(CompensationPermissions.Admin)]
    public async Task<ActionResult<BulkSalaryIncreaseResultDto>> BulkSalaryIncrease(
        [FromBody] BulkSalaryIncreaseCommand request,
        CancellationToken cancellationToken)
    {
        request.InitiatedByUserId = GetCurrentUserId();
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
}

