using System.Security.Claims;

namespace NineERP.Web.Extensions
{
    public static class IdentityExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            var claim = ((ClaimsIdentity)claimsPrincipal.Identity!).Claims.Single(x => x.Type == "UserId");
            return Guid.Parse(claim.Value);
        }

        public static string GetUserName(this ClaimsPrincipal claimsPrincipal)
        {
            var claim = ((ClaimsIdentity)claimsPrincipal.Identity!).Claims.Single(x => x.Type == "UserName");
            return claim.Value;
        }

        public static string GetSpecificClaim(this ClaimsPrincipal claimsPrincipal, string claimType)
        {
            var claim = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == claimType);
            return claim != null ? claim.Value : string.Empty;
        }
    }
}
