using Maliev.CompensationService.Application.DTOs;
using MediatR;

namespace Maliev.CompensationService.Application.Queries;

/// <summary>
/// Query to generate compensation analysis report
/// </summary>
/// <param name="DepartmentId">Optional department identifier to filter the analysis</param>
public record GetCompensationAnalysisQuery(Guid? DepartmentId = null) : IRequest<CompensationAnalysisDto>;
