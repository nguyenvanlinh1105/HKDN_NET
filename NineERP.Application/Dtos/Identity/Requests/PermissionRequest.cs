namespace NineERP.Application.Dtos.Identity.Requests
{
    public class PermissionRequest
    {
        public string RoleId { get; set; } = default!;
        public IList<RoleClaimRequest> RoleClaims { get; set; } = new List<RoleClaimRequest>();
    }
}