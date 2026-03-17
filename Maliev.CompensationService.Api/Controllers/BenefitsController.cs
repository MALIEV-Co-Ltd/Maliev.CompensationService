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
/// Controller for managing benefit enrollments
/// </summary>
[ApiController]
[ApiVersion("1")]
[Route("compensation/v{version:apiVersion}/employees")]
public class BenefitsController : BaseController
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="BenefitsController"/> class
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    public BenefitsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all active benefits available for enrollment
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of available benefits</returns>
    [HttpGet("/compensation/v{version:apiVersion}/benefits")]
    [RequirePermission(CompensationPermissions.Read)]
    public async Task<ActionResult<IEnumerable<BenefitDto>>> GetAvailableBenefits(CancellationToken cancellationToken)
    {
        var query = new GetAvailableBenefitsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets all benefit enrollments for a specific employee
    /// </summary>
    /// <param name="employeeId">Unique identifier of the employee</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of the employee's benefit enrollments</returns>
    [HttpGet("{employeeId:guid}/benefits")]
    [RequirePermission(CompensationPermissions.Read)]
    public async Task<ActionResult<IEnumerable<BenefitsEnrollmentDto>>> GetEmployeeBenefits(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var query = new GetEmployeeBenefitsQuery(employeeId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Enrolls an employee in a benefit program
    /// </summary>
    /// <param name="employeeId">Unique identifier of the employee</param>
    /// <param name="data">Enrollment details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The newly created enrollment record</returns>
    [HttpPost("{employeeId:guid}/benefits/enrollments")]
    [RequirePermission(CompensationPermissions.Update)]
    public async Task<ActionResult<BenefitsEnrollmentDto>> EnrollInBenefit(
        Guid employeeId,
        [FromBody] EnrollInBenefitDto data,
        CancellationToken cancellationToken)
    {
        var command = new EnrollInBenefitsCommand(employeeId, data.BenefitId, data);
        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(EnrollInBenefit), new { employeeId }, result);
    }

    /// <summary>
    /// Updates an existing benefit enrollment for an employee
    /// </summary>
    /// <param name="employeeId">Unique identifier of the employee</param>
    /// <param name="enrollmentId">Unique identifier of the enrollment</param>
    /// <param name="data">Updated enrollment details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated enrollment record</returns>
    [HttpPut("{employeeId:guid}/benefits/enrollments/{enrollmentId:guid}")]
    [RequirePermission(CompensationPermissions.Update)]
    public async Task<ActionResult<BenefitsEnrollmentDto>> UpdateBenefitsEnrollment(
        Guid employeeId,
        Guid enrollmentId,
        [FromBody] UpdateBenefitsEnrollmentDto data,
        CancellationToken cancellationToken)
    {
        var command = new UpdateBenefitsEnrollmentCommand(enrollmentId, data);
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Terminates an active benefit enrollment for an employee
    /// </summary>
    /// <param name="employeeId">Unique identifier of the employee</param>
    /// <param name="enrollmentId">Unique identifier of the enrollment</param>
    /// <param name="terminationDate">Optional termination date (defaults to today)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpDelete("{employeeId:guid}/benefits/enrollments/{enrollmentId:guid}")]
    [RequirePermission(CompensationPermissions.Update)]
    public async Task<IActionResult> TerminateBenefit(
        Guid employeeId,
        Guid enrollmentId,
        [FromQuery] DateTime? terminationDate,
        CancellationToken cancellationToken)
    {
        var command = new TerminateBenefitCommand(employeeId, enrollmentId, terminationDate ?? DateTime.UtcNow);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Adds a dependent to a benefit enrollment
    /// </summary>
    /// <param name="employeeId">Unique identifier of the employee</param>
    /// <param name="enrollmentId">Unique identifier of the enrollment</param>
    /// <param name="dto">Dependent details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created dependent record</returns>
    [HttpPost("{employeeId:guid}/benefits/enrollments/{enrollmentId:guid}/dependents")]
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
}


