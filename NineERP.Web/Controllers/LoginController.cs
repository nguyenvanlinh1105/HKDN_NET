using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NineERP.Application.Interfaces.Common;
using NineERP.Domain.Entities.Identity;
using NineERP.Infrastructure.Helpers;
using NineERP.Web.Models.Login;
using Serilog;
using System.Security.Claims;
using UAParser;
using NineERP.Application.Dtos.AuditLog;

namespace NineERP.Web.Controllers
{
    public class LoginController(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IAuditLogService auditLogService,
        IStringLocalizer<LoginController> localizer)
        : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var authResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (authResult.Succeeded && authResult.Principal != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(LoginViewModel model, string? returnUrl = null)
        {
            var user = await userManager.FindByNameAsync(model.UserName);

            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            {
                Log.Warning("Login failed for user: {User}", model.UserName);
                return Json(new { succeeded = false, messages = localizer["UserNameOrPasswordInvalid"] });
            }

            if (user.LockoutEnabled)
            {
                Log.Warning("Login blocked for locked user: {User}", model.UserName);
                return Json(new { succeeded = false, messages = localizer["LockUser"] });
            }

            var roleNames = await userManager.GetRolesAsync(user);

            var permissionClaims = new List<Claim>();
            foreach (var roleName in roleNames)
            {
                var role = await roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var claimsList = await roleManager.GetClaimsAsync(role);
                    permissionClaims.AddRange(claimsList.Where(c => c.Type == "permission"));
                }
            }

            if (!permissionClaims.Any())
            {
                Log.Warning("Login rejected for user without permission: {User}", model.UserName);
                return Json(new { succeeded = false, messages = localizer["AccountAccess"] });
            }

            var securityStamp = await userManager.GetSecurityStampAsync(user);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new("FullName", user.FullName),
                new("Avatar", user.AvatarUrl ?? string.Empty),
                new("AspNet.Identity.SecurityStamp", securityStamp)
            };

            foreach (var role in roleNames)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            claims.AddRange(permissionClaims);

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe
                        ? DateTimeOffset.UtcNow.AddDays(14)   // nếu tick Remember Me
                        : DateTimeOffset.UtcNow.AddHours(2)   // nếu không
                });

            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.UserName ?? "");
            HttpContext.Session.SetString("FullName", user.FullName);

            Log.Information("User logged in: {User}", user.UserName);
            var userAgent = Request.Headers["User-Agent"].ToString();
            var parser = Parser.GetDefault();
            var clientInfo = parser.Parse(userAgent);

            // Create an audit log entry
            var auditEntry = new AuditLogDto
            {
                TableName = "Users",
                ActionType = "Login",
                KeyValues = $"UserId: {user.Id}",
                NewValues = $"UserName: {user.UserName}, FullName: {user.FullName}, IsPersistent: {model.RememberMe}, UserAgent: {userAgent}, Browser: {clientInfo.UA.ToString()}, OS: {clientInfo.OS.ToString()}, Device: {clientInfo.Device.ToString()}",
                ActionTimestamp = DateTime.UtcNow,
                UserId = user.Id,
                UserName = user.UserName,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
            };

            await auditLogService.LogAsync(auditEntry);

            return Json(new
            {
                succeeded = true,
                redirectUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Action("Index", "Dashboard")
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}
