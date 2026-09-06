using System.Security.Claims;
using DefNotEbay_API.Extensions;

using Xunit;

namespace DefNotEbay_API.Tests;

public class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void GetUserId_ReturnsValue_FromSubClaim()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("sub", "42")
        }, "Bearer"));

        var userId = principal.GetUserId();

        Assert.Equal(42, userId);
    }

    [Fact]
    public void GetUserId_ReturnsNull_WhenNoSupportedClaimExists()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, "user@example.com")
        }, "Bearer"));

        var userId = principal.GetUserId();

        Assert.Null(userId);
    }
}
