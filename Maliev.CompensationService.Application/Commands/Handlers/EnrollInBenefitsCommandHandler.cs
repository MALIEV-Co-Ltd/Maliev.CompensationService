using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using MediatR;

namespace Maliev.CompensationService.Application.Commands.Handlers;

/// <summary>
/// Handler for EnrollInBenefitsCommand.
/// </summary>
public class EnrollInBenefitsCommandHandler : IRequestHandler<EnrollInBenefitsCommand, BenefitsEnrollmentDto>
{
    private readonly IBenefitsRepository _benefitsRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnrollInBenefitsCommandHandler"/> class.
    /// </summary>
    /// <param name="benefitsRepository">The benefits repository.</param>
    public EnrollInBenefitsCommandHandler(IBenefitsRepository benefitsRepository)
    {
        _benefitsRepository = benefitsRepository;
    }

    /// <inheritdoc/>
    public async Task<BenefitsEnrollmentDto> Handle(EnrollInBenefitsCommand request, CancellationToken cancellationToken)
    {
        // Implementation logic
        return new BenefitsEnrollmentDto();
    }
}