namespace NineERP.Application.Interfaces.Common
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string EmployeeNo { get; }
        string UserName { get; }
        string Email { get; }
        string FullName { get; }
        string RoleName { get; }
        string Origin { get; }
        string? Phone { get; }
        string? IpAddress { get; }
    }
}
