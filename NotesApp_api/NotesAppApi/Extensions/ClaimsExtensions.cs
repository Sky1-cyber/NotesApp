using System.Security.Claims;

namespace NotesAppApi.Extensions;

public static class ClaimsExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var id = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub") ?? throw new UnauthorizedAccessException("User id not found in token");
        return int.Parse(id);
    }
}