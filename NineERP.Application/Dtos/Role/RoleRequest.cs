using NineERP.Application.Request;

namespace NineERP.Application.Dtos.Role
{
    public class RoleRequest : RequestParameter
    {
        public string? RoleName { get; set; }
    }
}