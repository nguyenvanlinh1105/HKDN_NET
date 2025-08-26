using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using NineERP.Application.Constants.Role;
using NineERP.Infrastructure.Services.SecurityStamp;

namespace NineERP.Web.Authorization
{
    /// <summary>
    /// Check if the working version of the user login is still valid with the system
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="userManager"></param>
    /// <param name="stampCache"></param>
    public class SecurityStampValidationFilter<TUser>(
        UserManager<TUser> userManager,
        ISecurityStampCacheService stampCache)
        : IAsyncAuthorizationFilter
        where TUser : class
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Check if user is logged in
            if (user.Identity is { IsAuthenticated: true })
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var tokenStamp = user.FindFirst("AspNet.Identity.SecurityStamp")?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    // Get Security Stamp from cache or database
                    string cachedStamp = await stampCache.GetSecurityStampAsync(userId, async () =>
                    {
                        var dbUser = await userManager.FindByIdAsync(userId);
                        return dbUser == null ? "INVALID_STAMP" : await userManager.GetSecurityStampAsync(dbUser);
                    });

                    // If security stamp does not match → Sign out and redirect to Login page
                    if (cachedStamp != tokenStamp)
                    {
                        var httpContext = context.HttpContext;
                        if (user.IsInRole(RoleConstants.SuperAdmin) || user.IsInRole(RoleConstants.Admin))
                        {
                            await httpContext.SignOutAsync(SchemeConstants.Admin);
                            context.Result = new RedirectToPageResult("/Admin/Login/Index");
                        }
                        else
                        {
                            await httpContext.SignOutAsync(SchemeConstants.Member);
                            context.Result = new RedirectToPageResult("/Member/Login/Index");
                        }
                    }
                }
            }
        }
    }
}
