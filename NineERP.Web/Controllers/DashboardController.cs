using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Domain.Enums;

namespace NineERP.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class DashboardController : BaseController
    {
        [Authorize(Policy = $"Permission:{PermissionValue.Dashboard.View}")]
        public IActionResult Index()
        {
            ViewBag.PageTitle = "Dashboard";
            return View();
        }
    }
}
