using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NineERP.Application.Interfaces.Common;

namespace NineERP.Infrastructure.Services.Identity
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;

            UserId = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            UserName = user?.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
            Email = user?.FindFirstValue("Email") ?? string.Empty;
            FullName = user?.FindFirstValue("FullName") ?? string.Empty;
            RoleName = user?.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            Phone = user?.FindFirstValue("PhoneNumber");
            EmployeeNo = user?.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
            Claims = user?.Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value)).ToList() ?? [];
            Origin = httpContextAccessor.HttpContext?.Request.Headers["Origin"].ToString() ?? string.Empty;
            IpAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        }

        public string UserId { get; }
        public string UserName { get; }
        public string EmployeeNo { get; }
        public string Email { get; }
        public string FullName { get; }
        public string RoleName { get; }
        public string Origin { get; }
        public string? Phone { get; }
        public string? IpAddress { get; }

        public List<KeyValuePair<string, string>> Claims { get; set; }
    }
}
