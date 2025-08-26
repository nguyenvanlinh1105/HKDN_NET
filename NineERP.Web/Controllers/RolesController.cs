using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Dtos.Role;
using NineERP.Application.Features.RolesFeature.Commands;
using NineERP.Application.Features.RolesFeature.Queries;
using NineERP.Domain.Entities.Identity;
using NineERP.Domain.Enums;
using NineERP.Infrastructure.Services.SecurityStamp;
using System.Security.Claims;

namespace NineERP.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class RolesController(
        IMediator mediator,
        RoleManager<AppRole> roleManager,
        UserManager<AppUser> userManager,
        SecurityStampCacheService securityStampCacheService
    ) : BaseController
    {
        [Authorize(Policy = $"Permission:{PermissionValue.Roles.View}")]
        public IActionResult Index() => View();

        [HttpGet]
        [Authorize(Policy = $"Permission:{PermissionValue.Roles.View}")]
        public async Task<IActionResult> GetAllPaging([FromQuery] RoleRequest request)
        {
            var result = await mediator.Send(new GetRolePaginationQuery(request));
            return Json(result);
        }

        [HttpGet]
        [Authorize(Policy = $"Permission:{PermissionValue.Roles.View}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await mediator.Send(new GetRoleByIdQuery(id));
            return Json(result);
        }

        [HttpPost]
        [Authorize(Policy = $"Permission:{PermissionValue.Roles.Add}")]
        public async Task<IActionResult> Add([FromBody] RoleDetailDto request)
        {
            var result = await mediator.Send(new AddRoleCommand(request));
            return Json(result);
        }

        [HttpPost]
        [Authorize(Policy = $"Permission:{PermissionValue.Roles.Update}")]
        public async Task<IActionResult> Update([FromBody] RoleDetailDto request)
        {
            var result = await mediator.Send(new UpdateRoleCommand(request));
            return Json(result);
        }

        [HttpPost]
        [Authorize(Policy = $"Permission:{PermissionValue.Roles.Delete}")]
        public async Task<IActionResult> Delete(string id)
        {
            var roleResult = await mediator.Send(new GetRoleByIdQuery(id));
            if (!roleResult.Succeeded)
                return Json(new { succeeded = false, messages = "RoleNotFound" });

            var usersInRole = await userManager.GetUsersInRoleAsync(roleResult.Data.Name);
            if (usersInRole.Any())
                return Json(new { succeeded = false, messages = "NotDeleteRole" });

            var result = await mediator.Send(new DeleteRoleCommand(id));
            return Json(result);
        }

        [HttpPost]
        [Authorize(Policy = $"Permission:{PermissionValue.Roles.Update}")]
        public async Task<IActionResult> SavePermission([FromForm] List<string>? listPermission, [FromForm] string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
                return Json(new { succeeded = false, messages = "RoleNotFound" });

            var newClaims = listPermission?.Select(p => new Claim(PermissionValue.PermissionType, p)).ToList() ?? new();

            await mediator.Send(new UpdateRoleClaimsCommand(role.Id, newClaims));

            var usersInRole = await userManager.GetUsersInRoleAsync(role.Name!);
            foreach (var user in usersInRole)
            {
                securityStampCacheService.RemoveSecurityStampFromCache(user.Id);
                await userManager.UpdateSecurityStampAsync(user);
            }

            return Json(new { succeeded = true, messages = "UpdateSuccess" });
        }

        [HttpGet]
        [Authorize(Policy = $"Permission:{PermissionValue.Roles.View}")]
        public async Task<IActionResult> PermissionWithRole([FromQuery] string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null) return Ok(new List<PermissionWithRoleDto>());

            var roleClaims = await roleManager.GetClaimsAsync(role);
            var result = roleClaims
                .Where(x => x.Type == PermissionValue.PermissionType)
                .Select(x => new PermissionWithRoleDto { Permission = x.Value })
                .ToList();

            return Ok(result);
        }

        [HttpGet("/data/all-permissions")]
        [Authorize(Policy = $"Permission:{PermissionValue.Roles.View}")]
        public IActionResult GetAllPermissions()
        {
            var permissionGroups = typeof(PermissionValue).GetNestedTypes()
                .Select(group =>
                {
                    var permissions = group
                        .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                        .Where(f => f.FieldType == typeof(string))
                        .Select(f => new
                        {
                            label = f.Name,
                            value = f.GetValue(null)?.ToString()
                        }).ToList();

                    return new
                    {
                        group = group.Name,
                        permissions
                    };
                }).ToList();

            return Ok(permissionGroups);
        }
    }
}
