using System.Text.Json;
using Maliev.CompensationService.Application.DTOs;
using Maliev.CompensationService.Application.Interfaces;
using Maliev.CompensationService.Application.Queries;
using Maliev.CompensationService.Application.Queries.Handlers;
using Maliev.CompensationService.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

namespace Maliev.CompensationService.Tests.Unit.Queries;

public class GetAvailableBenefitsQueryHandlerTests
{
    private readonly Mock<IBenefitsRepository> _repositoryMock;
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly GetAvailableBenefitsQueryHandler _handler;

    public GetAvailableBenefitsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IBenefitsRepository>();
        _cacheMock = new Mock<IDistributedCache>();
        _handler = new GetAvailableBenefitsQueryHandler(_repositoryMock.Object, _cacheMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFromCache_WhenCacheExists()
    {
        // Arrange
        var dtos = new List<BenefitDto> { new BenefitDto { Name = "Cached Benefit" } };
        var cachedData = JsonSerializer.Serialize(dtos);
        
        _cacheMock.Setup(c => c.GetAsync("available_benefits", It.IsAny<CancellationToken>()))
            .ReturnsAsync(System.Text.Encoding.UTF8.GetBytes(cachedData));

        var query = new GetAvailableBenefitsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("Cached Benefit", result.First().Name);
        _repositoryMock.Verify(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFromRepository_WhenCacheEmpty()
    {
        // Arrange
        _cacheMock.Setup(c => c.GetAsync("available_benefits", It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        var benefits = new List<Benefit> { new Benefit { Name = "Repo Benefit" } };
        _repositoryMock.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(benefits);

        var query = new GetAvailableBenefitsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("Repo Benefit", result.First().Name);
        _cacheMock.Verify(c => c.SetAsync(
            "available_benefits", 
            It.IsAny<byte[]>(), 
            It.IsAny<DistributedCacheEntryOptions>(), 
            It.IsAny<CancellationToken>()), Times.Once);
    }
}