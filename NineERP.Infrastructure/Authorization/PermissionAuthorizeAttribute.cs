using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NineERP.Infrastructure.Authorization
{
    public class PermissionAuthorizeAttribute : TypeFilterAttribute
    {
        public PermissionAuthorizeAttribute(string permission)
            : base(typeof(PermissionAuthorizationFilter))
        {
            Arguments = new object[] { permission };
        }
    }

    public class PermissionAuthorizationFilter(IAuthorizationService authorizationService, string permission)
        : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var result = await authorizationService.AuthorizeAsync(context.HttpContext.User, null, new PermissionRequirement(permission));
            if (!result.Succeeded)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
