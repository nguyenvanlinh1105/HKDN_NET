using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Dtos.User;
using NineERP.Application.Features.UsersFeature.Commands;
using NineERP.Application.Features.UsersFeature.Queries;
using NineERP.Domain.Enums;

namespace NineERP.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class UsersController(IMediator mediator) : BaseController
    {
        [Authorize(Policy = $"Permission:{PermissionValue.Users.View}")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
       [Authorize(Policy = $"Permission:{PermissionValue.Users.View}")]
        public async Task<IActionResult> GetAllUserPaging([FromQuery] UserRequest request)
        {
            var result = await mediator.Send(new GetUsersPaginationQuery(request));
            return Json(result);
        }

        [HttpGet]
       [Authorize(Policy = $"Permission:{PermissionValue.Users.View}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await mediator.Send(new GetUserByIdQuery(id));
            return Json(result);
        }

        [HttpPost]
        [Authorize(Policy = $"Permission:{PermissionValue.Users.Update}")]
        public async Task<IActionResult> ChangeUserStatus(string id)
        {
            var result = await mediator.Send(new ChangeUserStatusCommand(id));
            return Json(result);
        }

        [HttpPost]
        [Authorize(Policy = $"Permission:{PermissionValue.Users.Delete}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await mediator.Send(new DeleteUserCommand(id));
            return Json(result);
        }

        [HttpPost]
        [Authorize(Policy = $"Permission:{PermissionValue.Users.Update}")]
        public async Task<IActionResult> UpdateUser([FromBody] UserDetailDto request)
        {
            var result = await mediator.Send(new UpdateUserCommand(request));
            return Json(result);
        }

        [HttpPost]
        [Authorize(Policy = $"Permission:{PermissionValue.Users.Add}")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            return Json(await mediator.Send(command));
        }

    }
}