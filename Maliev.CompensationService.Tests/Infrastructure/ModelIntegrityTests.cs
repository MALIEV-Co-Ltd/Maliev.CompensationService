using Maliev.CompensationService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Maliev.CompensationService.Tests.Infrastructure;

public class ModelIntegrityTests
{
    [Fact]
    public void Model_ShouldNotHavePendingChanges()
    {
        var options = new DbContextOptionsBuilder<CompensationDbContext>()
            .UseNpgsql("Host=localhost;Database=ModelCheck")
            .Options;

        using var context = new CompensationDbContext(options);
        var hasChanges = context.Database.HasPendingModelChanges();

        Assert.False(hasChanges, "Run 'dotnet ef migrations add <Name> --project Maliev.CompensationService.Infrastructure --startup-project Maliev.CompensationService.Api'");
    }
}
