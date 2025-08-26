using Microsoft.AspNetCore.Authorization;

namespace NineERP.Infrastructure.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var user = context.User;
            if (!user.Identity?.IsAuthenticated == true)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var hasClaim = user.Claims.Any(c =>
                c.Type == "permission" &&
                c.Value.Equals(requirement.Permission, StringComparison.OrdinalIgnoreCase));

            if (hasClaim)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}