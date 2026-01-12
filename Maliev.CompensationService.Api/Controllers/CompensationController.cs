using Maliev.Aspire.ServiceDefaults.Authorization;
using Maliev.CompensationService.Application.Commands;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Queries;
using Maliev.CompensationService.Domain.Authorization;
using Maliev.CompensationService.Application.Common.Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maliev.CompensationService.Api.Controllers;

/// <summary>
/// Controller for managing employee compensation records, history, and benefits.
/// </summary>
[ApiController]
[Route("compensation/v1/employees")]
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
    /// Gets all benefit enrollments for a specific employee.
    /// </summary>
    /// <param name="employeeId">Unique identifier of the employee.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of benefit enrollments.</returns>
    [RequirePermission(CompensationPermissions.Read)]
    public async Task<ActionResult<IEnumerable<BenefitsEnrollmentDto>>> GetBenefits(Guid employeeId, CancellationToken cancellationToken)
    {
        var query = new GetEmployeeBenefitsQuery(employeeId);
        var result = await _mediator.Send(query, cancellationToken);

        if (!HasPermission(CompensationPermissions.ReadSensitive))
        {
            foreach (var enrollment in result)
            {
                foreach (var dependent in enrollment.Dependents)
                {
                    if (!string.IsNullOrEmpty(dependent.NationalId))
                    {
                        dependent.NationalId = "********";
                    }
                }
            }
        }

        return Ok(result);
    }

    /// <summary>
    /// Enrolls an employee in a benefit program.
    /// </summary>
    /// <param name="employeeId">Unique identifier of the employee.</param>
    /// <param name="benefitId">Unique identifier of the benefit program.</param>
    /// <param name="dto">Enrollment details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created benefit enrollment.</returns>
    [HttpPost("{employeeId:guid}/benefits/{benefitId:guid}/enroll")]
    [RequirePermission(CompensationPermissions.Update)]
    public async Task<ActionResult<BenefitsEnrollmentDto>> EnrollInBenefit(
        Guid employeeId,
        Guid benefitId,
        [FromBody] EnrollInBenefitDto dto,
        CancellationToken cancellationToken)
    {
        var command = new EnrollInBenefitsCommand(employeeId, benefitId, dto);
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetBenefits), new { employeeId }, result);
    }

    /// <summary>
    /// Updates an existing benefit enrollment.
    /// </summary>
    /// <param name="employeeId">Unique identifier of the employee.</param>
    /// <param name="enrollmentId">Unique identifier of the enrollment.</param>
    /// <param name="dto">Updated enrollment details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated benefit enrollment.</returns>
    [HttpPut("{employeeId:guid}/benefits/{enrollmentId:guid}")]
    [RequirePermission(CompensationPermissions.Update)]
    public async Task<ActionResult<BenefitsEnrollmentDto>> UpdateBenefitEnrollment(
        Guid employeeId,
        Guid enrollmentId,
        [FromBody] UpdateBenefitsEnrollmentDto dto,
        CancellationToken cancellationToken)
    {
        var command = new UpdateBenefitsEnrollmentCommand(enrollmentId, dto);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Adds a dependent to a benefit enrollment.
    /// </summary>
    /// <param name="employeeId">Unique identifier of the employee.</param>
    /// <param name="enrollmentId">Unique identifier of the enrollment.</param>
    /// <param name="dto">Dependent details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created dependent.</returns>
    [HttpPost("{employeeId:guid}/benefits/{enrollmentId:guid}/dependents")]
    [RequirePermission(CompensationPermissions.Update)]
    public async Task<ActionResult<DependentDto>> AddDependent(
        Guid employeeId,
        Guid enrollmentId,
        [FromBody] DependentDto dto,
        CancellationToken cancellationToken)
    {
        var command = new AddDependentCommand(enrollmentId, dto);
        var result = await _mediator.Send(command, cancellationToken);
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

