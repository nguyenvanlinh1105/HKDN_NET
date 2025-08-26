using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NineERP.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public string CurrentUserName => User.Identity?.Name ?? "Unknown";

        public string? CurrentUserId =>
            User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        public List<string> CurrentUserPermissions =>
            User.Claims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .ToList();

        public bool HasPermission(string permission) =>
            User.HasClaim("permission", permission);
    }
}
