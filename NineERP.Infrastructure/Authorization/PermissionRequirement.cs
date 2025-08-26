using Microsoft.AspNetCore.Authorization;

namespace NineERP.Infrastructure.Authorization
{
    public class PermissionRequirement(string permission) : IAuthorizationRequirement
    {
        public string Permission { get; } = permission;
    }
}