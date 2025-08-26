using NineERP.Application.Request;

namespace NineERP.Application.Dtos.User
{
    public class UserRequest : RequestParameter
    {
        public string? RoleName { get; set; }
    }
}