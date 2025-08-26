using Microsoft.AspNetCore.Authorization;

namespace NineERP.Infrastructure.Authorization
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        const string PolicyPrefix = "Permission:";

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(PolicyPrefix))
            {
                var permission = policyName.Substring(PolicyPrefix.Length);
                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(permission))
                    .Build();

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            return Task.FromResult<AuthorizationPolicy?>(null);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
            => Task.FromResult(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
            => Task.FromResult<AuthorizationPolicy?>(null);
    }
}