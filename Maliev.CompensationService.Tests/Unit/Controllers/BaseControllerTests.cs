using System.Security.Claims;
using Maliev.CompensationService.Api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Maliev.CompensationService.Tests.Unit.Controllers;

public class BaseControllerTests
{
    private class TestableBaseController : BaseController
    {
        public new Guid GetCurrentUserId() => base.GetCurrentUserId();
        public new bool HasPermission(string permission) => base.HasPermission(permission);
    }

    [Fact]
    public void GetCurrentUserId_WhenNotAuthenticated_ReturnsEmptyGuid()
    {
        var controller = new TestableBaseController();
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var result = controller.GetCurrentUserId();

        Assert.Equal(Guid.Empty, result);
    }

    [Fact]
    public void GetCurrentUserId_WhenAuthenticatedWithValidGuid_ReturnsUserId()
    {
        var controller = new TestableBaseController();
        var userId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var result = controller.GetCurrentUserId();

        Assert.Equal(userId, result);
    }

    [Fact]
    public void GetCurrentUserId_WhenAuthenticatedWithInvalidGuid_ReturnsEmptyGuid()
    {
        var controller = new TestableBaseController();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "invalid-guid")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var result = controller.GetCurrentUserId();

        Assert.Equal(Guid.Empty, result);
    }

    [Fact]
    public void HasPermission_WhenUserHasPermission_ReturnsTrue()
    {
        var controller = new TestableBaseController();
        var claims = new List<Claim>
        {
            new Claim("permission", "read")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var result = controller.HasPermission("read");

        Assert.True(result);
    }

    [Fact]
    public void HasPermission_WhenUserDoesNotHavePermission_ReturnsFalse()
    {
        var controller = new TestableBaseController();
        var claims = new List<Claim>
        {
            new Claim("permission", "read")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var result = controller.HasPermission("admin");

        Assert.False(result);
    }

    [Fact]
    public void HasPermission_WhenUserHasNoClaims_ReturnsFalse()
    {
        var controller = new TestableBaseController();
        var identity = new ClaimsIdentity("TestAuth");
        var principal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var result = controller.HasPermission("read");

        Assert.False(result);
    }
}
