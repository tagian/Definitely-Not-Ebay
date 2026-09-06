using System.Security.Claims;

namespace DefNotEbay_API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int? GetUserId(this ClaimsPrincipal principal)
    {
        if (principal == null)
        {
            throw new ArgumentNullException(nameof(principal));
        }

        var userIdString = principal.FindFirstValue("sub");
            

        if (string.IsNullOrWhiteSpace(userIdString))
        {
            return null;
        }

        return int.TryParse(userIdString, out var userId) ? userId : null;
    }
}
