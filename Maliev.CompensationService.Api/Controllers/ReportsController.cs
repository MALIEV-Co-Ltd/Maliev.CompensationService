using Maliev.Aspire.ServiceDefaults.Authorization;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Queries;
using Maliev.CompensationService.Domain.Authorization;
using Maliev.CompensationService.Application.Common.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Maliev.CompensationService.Api.Controllers;

/// <summary>
/// Controller for generating compensation and budget reports
/// </summary>
[ApiController]
[Route("compensation/v1/reports")]
public class ReportsController : BaseController
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportsController"/> class
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Generates a compensation analysis report, optionally filtered by department
    /// </summary>
    /// <param name="departmentId">Optional department identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Compensation analysis statistics</returns>
    [HttpGet("compensation-analysis")]
    [RequirePermission(CompensationPermissions.Reports)]
    public async Task<ActionResult<CompensationAnalysisDto>> GetCompensationAnalysis(
        [FromQuery] Guid? departmentId,
        CancellationToken cancellationToken)
    {
        var query = new GetCompensationAnalysisQuery(departmentId);
        var result = await _mediator.Send(query, cancellationToken);

        if (!HasPermission(CompensationPermissions.ReadSensitive))
        {
            result.TotalAnnualBudget = 0;
            result.AverageSalary = 0;
            result.MinimumSalary = 0;
            result.MaximumSalary = 0;
        }

        return Ok(result);
    }

    /// <summary>
    /// Gets the total annual compensation budget
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A summary of the annual budget</returns>
    [HttpGet("compensation-budget")]
    [RequirePermission(CompensationPermissions.Reports)]
    public async Task<ActionResult<CompensationAnalysisDto>> GetCompensationBudget(CancellationToken cancellationToken)
    {
        var query = new GetCompensationAnalysisQuery();
        var result = await _mediator.Send(query, cancellationToken);

        if (!HasPermission(CompensationPermissions.ReadSensitive))
        {
            result.TotalAnnualBudget = 0;
            result.AverageSalary = 0;
            result.MinimumSalary = 0;
            result.MaximumSalary = 0;
        }

        return Ok(result);
    }
}
