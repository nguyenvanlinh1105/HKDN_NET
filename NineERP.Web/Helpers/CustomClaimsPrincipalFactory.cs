using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NineERP.Domain.Entities.Identity;

namespace NineERP.Web.Helpers
{
    public class CustomClaimsPrincipalFactory(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IOptions<IdentityOptions> options)
        : UserClaimsPrincipalFactory<AppUser, AppRole>(userManager, roleManager, options)
    {
        private readonly UserManager<AppUser> _userManger = userManager;

        public override async Task<ClaimsPrincipal> CreateAsync(AppUser user)
        {
            var principal = await base.CreateAsync(user);
            var roles = await _userManger.GetRolesAsync(user);
            ((ClaimsIdentity)principal.Identity!).AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName ?? ""),
                new Claim("Email",user.Email ?? ""),
                new Claim("FullName",user.FullName),
                new Claim("PhoneNumber",user.PhoneNumber ?? ""),
                new Claim("Avatar",user.AvatarUrl ?? string.Empty),
                new Claim("Roles",string.Join(";",roles)),
                new Claim("UserId",user.Id),
                new Claim("UserName",user.UserName ?? string.Empty),
            });
            return principal;
        }
    }
}