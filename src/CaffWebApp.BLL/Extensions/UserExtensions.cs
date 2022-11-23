using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CaffWebApp.BLL.Extensions;

public static class UserExtensions
{
    public static string GetCurrentUserId(this IHttpContextAccessor httpContext)
    {
        return httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                    httpContext.HttpContext.User.FindFirstValue("sub");
    }
}
