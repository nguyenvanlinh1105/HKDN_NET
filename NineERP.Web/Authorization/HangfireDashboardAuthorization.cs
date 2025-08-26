using Hangfire.Dashboard;

namespace NineERP.Web.Authorization;

public class HangfireDashboardAuthorization : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true; // Cho phép truy cập tạm thời để kiểm tra
        //var httpContext = context.GetHttpContext();

        //    // Kiểm tra null trước khi truy cập User.Identity
        //    if (httpContext?.User?.Identity == null || !httpContext.User.Identity.IsAuthenticated)
        //    {
        //        return false;
        //    }

        //    // Chỉ cho phép Admin hoặc SuperAdmin truy cập
        //    var allowedRoles = new[] { "Admin", "SuperAdmin" };
        //    return httpContext.User.Claims
        //        .Where(c => c.Type == ClaimTypes.Role)
        //        .Any(c => allowedRoles.Contains(c.Value));
    }
}